using System;
using System.Collections.Generic;
using UnityEngine;

public class InkTagRouter : MonoBehaviour
{
    public static InkTagRouter Instance;
    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 由 #spawn:<id> 记录，供 #goto 使用
    string pendingSpawnPoint;

    public void ProcessTags(List<string> tags)
    {
        if (tags == null || tags.Count == 0) return;

        foreach (var raw in tags)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;

            // 统一成 key:value
            var s = raw.Trim();
            if (s[0] == '#') s = s.Substring(1);                  // 去掉 '#'
            int colon = s.IndexOf(':');
            string tagKey = (colon >= 0 ? s.Substring(0, colon) : s).Trim().ToLower();
            string val    = (colon >= 0 ? s.Substring(colon + 1) : "").Trim();

            switch (tagKey)
            {
                // --- 背景 ---
                case "bg":
                    if (val.Equals("clear", StringComparison.OrdinalIgnoreCase))
                        DialogueVisuals.Instance?.ClearBackground();
                    else
                        DialogueVisuals.Instance?.SetBackground(val); // "bg1"
                    break;

                // --- 说话人：#speaker:名字 或 #speaker:left:名字 ---
                case "speaker":
                {
                    string pos  = null;
                    string name = val;

                    var p = val.Split(':'); // e.g. "left: 日记本"
                    if (p.Length >= 2)
                    {
                        var maybePos = p[0].Trim().ToLower();
                        if (maybePos == "left" || maybePos == "right" || maybePos == "center")
                        {
                            pos  = maybePos;
                            name = val.Substring(val.IndexOf(':') + 1).Trim();
                        }
                    }
                    DialogueVisuals.Instance?.SetSpeaker(name, pos);
                    break;
                }

                // --- 立绘：#ch:left:rin.neutral ---
                case "ch":
                {
                    var p = val.Split(':'); // [pos, spriteKey]
                    if (p.Length >= 2)
                    {
                        var pos       = p[0].Trim();
                        var spriteKey = p[1].Trim();
                        DialogueVisuals.Instance?.SetPortrait(pos, spriteKey);
                    }
                    break;
                }

                // --- 清空立绘 ---
                case "clearportraits":
                    DialogueVisuals.Instance?.ClearPortraits();
                    break;

                // --- 道具 ---
                case "give":
                    InventoryManager.Instance?.AddItemById(val, 1);
                    break;

                case "take":
                {
                    var p = val.Split(':'); // "ItemId" 或 "ItemId:3"
                    var id  = p[0].Trim();
                    int cnt = (p.Length > 1 && int.TryParse(p[1], out var v)) ? v : 1;
                    if (!string.IsNullOrEmpty(id))
                        InventoryManager.Instance?.RemoveItem(id, cnt);
                    break;
                }

                // --- 标记位 ---
                case "flag":
                {
                    var p = val.Split(':'); // ["on","Key"] 或 ["off","Key"]
                    if (p.Length == 2)
                    {
                        bool on = p[0].Trim().ToLower() == "on";
                        StoryFlags.Instance?.Set(p[1].Trim(), on);
                    }
                    break;
                }

                // --- 数值 ---
                case "trust":  ApplyInt(ref GameStats.Instance.trust,  val); break;
                case "favor":  ApplyInt(ref GameStats.Instance.favor,  val); break;
                case "sanity": ApplyInt(ref GameStats.Instance.sanity, val); break;
                case "hp":     ApplyInt(ref GameStats.Instance.hp,     val); break;
                case "evil":   ApplyInt(ref GameStats.Instance.evil,   val); break;

                // --- 出生点 / 跳转 ---
                case "spawn":
                    pendingSpawnPoint = val;
                    break;

                case "goto":
                {
                    var sceneId = val;
                    var spawn = string.IsNullOrEmpty(pendingSpawnPoint) ? "Spawn_Default" : pendingSpawnPoint;
                    pendingSpawnPoint = null;
                    MapService.Instance?.LoadMapAdditive(sceneId, spawn);
                    break;
                }

                // --- 音频：SFX ---
                case "sfx":
                {
                    var parsed = ParseVal(val); // id;vol=0.8
                    float vol = TryGetFloat(parsed.kv, "vol", 1f);
                    AudioHub.Instance?.PlaySFX(parsed.id, vol);
                    break;
                }

                // --- 音频：VOICE ---
                case "voice":
                {
                    if (val.Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        AudioHub.Instance?.StopVoice();
                    }
                    else
                    {
                        var parsed = ParseVal(val); // id;vol=0.9
                        float vol = TryGetFloat(parsed.kv, "vol", 1f);
                        bool duck = AudioHub.Instance && AudioHub.Instance.duckDuringVoice;
                        AudioHub.Instance?.PlayVoice(parsed.id, vol, duck);
                    }
                    break;
                }

                // --- 音频：BGM ---
                case "bgm":
                {
                    if (val.Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        AudioHub.Instance?.StopBGM();
                        break;
                    }
                    var parsed = ParseVal(val); // id;vol=1;fade=0.8;loop=1
                    float vol  = TryGetFloat(parsed.kv, "vol",  1f);
                    float fade = TryGetFloat(parsed.kv, "fade", 0.8f);
                    bool  loop = TryGetBool (parsed.kv, "loop", true);
                    AudioHub.Instance?.PlayBGM(parsed.id, vol, fade, loop);
                    break;
                }

                case "stop:bgm":
                    AudioHub.Instance?.StopBGM();
                    break;

                // --- duck ---
                case "duck":
                {
                    bool on = val.Trim().ToLower() == "on";
                    AudioHub.Instance?.SetDuck(on);
                    break;
                }

                default:
                    Debug.Log($"[InkTagRouter] 未识别标签: {raw}");
                    break;
            }
        }
    }

    // ========== 工具函数（全部放在类内，避免找不到） ==========

    // 支持纯数字或 +N/-N
    void ApplyInt(ref int field, string deltaText)
    {
        if (int.TryParse(deltaText, out var v)) { field += v; return; }
        if ((deltaText.StartsWith("+") || deltaText.StartsWith("-")) &&
            int.TryParse(deltaText, out var d))
        {
            field += d;
        }
    }

    // 解析 "id;vol=0.8;fade=1.0;loop=1"
    Parsed ParseVal(string raw)
    {
        var result = new Parsed { id = raw, kv = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase) };
        if (string.IsNullOrWhiteSpace(raw)) return result;

        int semi = raw.IndexOf(';');
        if (semi >= 0)
        {
            result.id = raw.Substring(0, semi).Trim();
            var rest = raw.Substring(semi + 1);
            foreach (var seg in rest.Split(';'))
            {
                var s = seg.Trim();
                if (string.IsNullOrEmpty(s)) continue;
                var eq = s.IndexOf('=');
                if (eq > 0)
                {
                    var k = s.Substring(0, eq).Trim();
                    var v = s.Substring(eq + 1).Trim();
                    result.kv[k] = v;
                }
            }
        }
        return result;
    }

    float TryGetFloat(Dictionary<string,string> kv, string k, float dflt)
        => (kv!=null && kv.TryGetValue(k, out var v) && float.TryParse(v, out var f)) ? f : dflt;

    bool  TryGetBool(Dictionary<string,string> kv, string k, bool dflt)
    {
        if (kv != null && kv.TryGetValue(k, out var v))
        {
            v = v.Trim().ToLower();
            return v == "1" || v == "true" || v == "on";
        }
        return dflt;
    }

    struct Parsed
    {
        public string id;
        public Dictionary<string,string> kv;
    }
}

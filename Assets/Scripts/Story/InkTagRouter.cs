using System.Collections;
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
                if (val.Equals("clear", System.StringComparison.OrdinalIgnoreCase))
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

            default:
                Debug.Log($"[InkTagRouter] 未识别标签: {raw}");
                break;
        }
    }
}


// 和你之前一致：支持纯数字或 +N/-N
void ApplyInt(ref int field, string deltaText)
{
    if (int.TryParse(deltaText, out var v)) { field += v; return; }
    if ((deltaText.StartsWith("+") || deltaText.StartsWith("-")) &&
        int.TryParse(deltaText, out var d))
    {
        field += d;
    }
}


    // void ApplyInt(ref int field, string deltaText)
    // {
    //     // 支持纯数字 / 带 +-
    //     if (int.TryParse(deltaText, out var v))
    //     {
    //         field += v;
    //         return;
    //     }
    //     if ((deltaText.StartsWith("+") || deltaText.StartsWith("-")) &&
    //         int.TryParse(deltaText, out var d))
    //     {
    //         field += d;
    //     }
    // }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueVisuals : MonoBehaviour
{
    public static DialogueVisuals Instance;

    [System.Serializable]
    public class NamedSprite
    {
        public string key;     // 例如 "bg1" 或 "rin.neutral"
        public Sprite sprite;
    }

    [Header("UI Refs")]
    public Image bgImage;            // 对话 Panel 里的背景图
    public TMP_Text speakerText;     // 说话人名字

    [Header("Portrait Slots")]
    public Image portraitLeft;
    public Image portraitCenter;
    public Image portraitRight;

    [Header("背景图映射（名字->Sprite）")]
    public List<NamedSprite> backgrounds = new();

    [Header("立绘映射（key = id 或 id.pose）")]
    public List<NamedSprite> portraitSprites = new();

    // 内部查表
    Dictionary<string, Sprite> _bgMap;
    Dictionary<string, Sprite> _portraitMap;

    [Header("立绘高亮颜色")]
    public Color highlightColor = Color.white;                      // 说话人
    public Color dimColor       = new Color(0.72f, 0.72f, 0.72f, 1);// 非说话人

    // 当前各槽 id（可选，方便需要时查询）
    string currentLeftId, currentCenterId, currentRightId;

    void Awake()
    {
        Instance = this;

        _bgMap = new Dictionary<string, Sprite>();
        foreach (var x in backgrounds)
            if (x != null && !string.IsNullOrEmpty(x.key) && x.sprite)
                _bgMap[x.key.Trim().ToLower()] = x.sprite;

        _portraitMap = new Dictionary<string, Sprite>();
        foreach (var x in portraitSprites)
            if (x != null && !string.IsNullOrEmpty(x.key) && x.sprite)
                _portraitMap[x.key.Trim().ToLower()] = x.sprite;

        // 默认隐藏立绘槽
        SetPortraitVisible(portraitLeft,   false);
        SetPortraitVisible(portraitCenter, false);
        SetPortraitVisible(portraitRight,  false);
    }

    // 背景
    public void SetBackground(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) { ClearBackground(); return; }

        var k = key.Trim().ToLower();
        if (_bgMap != null && _bgMap.TryGetValue(k, out var sp) && sp)
        {
            if (bgImage) { bgImage.enabled = true; bgImage.sprite = sp; }
            return;
        }

        // 兜底：Resources/DialogBG/<key>.png
        var res = Resources.Load<Sprite>($"DialogBG/{key}");
        if (res && bgImage)
        {
            bgImage.enabled = true;
            bgImage.sprite = res;
        }
        else
        {
            Debug.LogWarning($"[DialogueVisuals] 未找到背景 '{key}'。请配置 backgrounds 或放到 Resources/DialogBG/{key}.png");
        }
    }

    public void ClearBackground()
    {
        if (!bgImage) return;
        bgImage.enabled = false;
        bgImage.sprite = null;
    }

    // 说话人（带可选位置）
    public void SetSpeaker(string name, string pos = null)
    {
        if (speakerText) speakerText.text = name ?? "";

        if (!string.IsNullOrEmpty(pos))
        {
            HighlightSlot(pos.Trim().ToLower());
        }
        // 若没给 pos：保持当前高亮不变（只改名字）
    }

    // 立绘：#ch:left:rin.neutral
    public void SetPortrait(string pos, string portraitKey)
    {
        if (string.IsNullOrWhiteSpace(pos) || string.IsNullOrWhiteSpace(portraitKey)) return;

        var k = portraitKey.Trim().ToLower();
        if (!_portraitMap.TryGetValue(k, out var sp) || !sp)
        {
            Debug.LogWarning($"[DialogueVisuals] 未找到立绘 '{portraitKey}'");
            return;
        }

        Image slot = null;
        switch (pos.Trim().ToLower())
        {
            case "left":   slot = portraitLeft;   currentLeftId   = portraitKey; break;
            case "center": slot = portraitCenter; currentCenterId = portraitKey; break;
            case "right":  slot = portraitRight;  currentRightId  = portraitKey; break;
            default:
                Debug.LogWarning($"[DialogueVisuals] 未知立绘槽位 '{pos}'（应为 left/center/right）");
                return;
        }

        if (!slot) return;
        slot.enabled = true;
        slot.sprite  = sp;

        // 设置为“非高亮”基色，等待 #speaker:left/right/center 再高亮
        slot.color = dimColor;
    }

    public void ClearPortraits()
    {
        ClearPortraitSlot(portraitLeft);   currentLeftId   = null;
        ClearPortraitSlot(portraitCenter); currentCenterId = null;
        ClearPortraitSlot(portraitRight);  currentRightId  = null;
    }

    void ClearPortraitSlot(Image img)
    {
        if (!img) return;
        img.sprite  = null;
        img.enabled = false;
    }

    void SetPortraitVisible(Image img, bool on)
    {
        if (!img) return;
        img.enabled = on;
        if (!on) img.sprite = null;
    }

    // 高亮槽位（改 tint，不改透明度）
    public void HighlightSlot(string pos)
    {
        Tint(portraitLeft,   dimColor);
        Tint(portraitCenter, dimColor);
        Tint(portraitRight,  dimColor);

        switch (pos)
        {
            case "left":   Tint(portraitLeft,   highlightColor); break;
            case "center": Tint(portraitCenter, highlightColor); break;
            case "right":  Tint(portraitRight,  highlightColor); break;
        }
    }

    void Tint(Image img, Color c)
    {
        if (img == null || !img.enabled) return;
        img.color = c;
    }
}
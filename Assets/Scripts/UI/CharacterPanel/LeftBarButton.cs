using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftBarButton : MonoBehaviour
{
    [Header("Wiring")]
     public RectTransform moveTarget;   // 指到视觉那层（Button的子物体或Button本身）
    public Button button;
    public Image bg;                   // 按钮底图（可选）

    [HideInInspector] public CharacterData data;

    LeftBarController _owner;
    float _t;
    bool _selected;
    Vector2 _basePos;
    LayoutElement _le;

    public void Init(LeftBarController owner, CharacterData cd)
    {
        _owner = owner;
        data = cd;

        if (bg) bg.sprite = cd.leftBarSprite;     // 根据角色设置不同底图
        if (button) button.onClick.RemoveAllListeners();
        if (button) button.onClick.AddListener(() => _owner.OnButtonClicked(this));

        if (moveTarget) _basePos = moveTarget.anchoredPosition;

        // 关键：把根的 LayoutElement 高度设置为视觉高度，避免 VLG 看到高度=0
        _le = GetComponent<LayoutElement>();
        if (_le && moveTarget)
        {
            var h = moveTarget.rect.height;
            if (h <= 0f) h = (moveTarget.sizeDelta.y != 0) ? moveTarget.sizeDelta.y : 120f; // 兜底
            _le.preferredHeight = h;
        }

        SetSelected(false, true);
    }

    public void SetSelected(bool on, bool instant = false)
    {
        _selected = on;
        if (instant) _t = on ? 1f : 0f;
    }

    void Update()
    {
        // 防守
        if (_owner == null || moveTarget == null) return;

        float target = _selected ? 1f : 0f;
        _t = Mathf.MoveTowards(_t, target, Time.unscaledDeltaTime * _owner.selectLerpSpeed);

        float shift = Mathf.Lerp(0f, _owner.selectedShiftX, _t); // 负值向左
        moveTarget.anchoredPosition = _basePos + new Vector2(shift, 0f);
    }

}

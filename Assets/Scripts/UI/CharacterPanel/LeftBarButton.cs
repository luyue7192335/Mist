using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftBarButton : MonoBehaviour
{
    [Header("Wiring")]
    public RectTransform moveTarget;  // 指向子物体 Button 的 RectTransform
    public Button button;
    public TMP_Text label;

    [HideInInspector] public CharacterData data;

    LeftBarController _owner;
    float _t;
    bool _selected;
    Vector2 _basePos;

    public void Init(LeftBarController owner, CharacterData cd, string title)
    {
        _owner = owner;
        data = cd;
        if (label)  label.text = string.IsNullOrEmpty(title) ? cd.displayName : title;
        if (button) button.onClick.AddListener(() => _owner.OnButtonClicked(this));

        _basePos = moveTarget ? moveTarget.anchoredPosition : Vector2.zero;
        SetSelected(false, true);
    }

    public void SetSelected(bool on, bool instant = false)
    {
        _selected = on;
        if (instant) _t = on ? 1f : 0f;
    }

    void Update()
    {
        float target = _selected ? 1f : 0f;
        _t = Mathf.MoveTowards(_t, target, Time.unscaledDeltaTime * _owner.selectLerpSpeed);

        if (moveTarget)
        {
            float shift = Mathf.Lerp(0f, _owner.selectedShiftX, _t); // 负值向左
            moveTarget.anchoredPosition = _basePos + new Vector2(shift, 0f);
        }
    }
}

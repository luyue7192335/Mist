using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarController : MonoBehaviour
{
    [Header("Build")]
    public Transform content;                  // 列表的 Content（带 VLG + CSF）
    public LeftBarButton buttonPrefab;         // 上面的按钮预制体
    public List<CharacterData> characters;     // 把所有角色数据拖进来

    [Header("Selection Anim")]
    public float selectedShiftX = -20f;        // 选中时向左偏移（负值）
    public float selectLerpSpeed = 10f;

    [Header("Target Panel")]
    public CharacterPanelController panel;     // 右侧面板控制器

    LeftBarButton _current;
    readonly List<LeftBarButton> _buttons = new();

    void Start()
    {
        Build();
        // 默认选中第一个
        if (_buttons.Count > 0) OnButtonClicked(_buttons[0], true);
    }

    void Build()
    {
        // 清空旧
        foreach (Transform t in content) Destroy(t.gameObject);
        _buttons.Clear();

        foreach (var cd in characters)
        {
            var b = Instantiate(buttonPrefab, content);
            b.Init(this, cd, cd.displayName);
            _buttons.Add(b);
        }
    }

    public void OnButtonClicked(LeftBarButton b, bool instant = false)
    {
        _current = b;
        foreach (var x in _buttons) x.SetSelected(x == b, instant);
        if (panel) panel.ShowCharacter(b.data);
    }
}


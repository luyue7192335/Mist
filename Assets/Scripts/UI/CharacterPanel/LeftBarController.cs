using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarController : MonoBehaviour
{
    [Header("Build")]
    public Transform content;                 // 列表 Content（带 VerticalLayoutGroup + ContentSizeFitter）
    public GameObject buttonPrefab;           // 你的“空物体”为根、Button 在子物体里的预制体
    public List<CharacterData> characters;    // 把所有角色数据拖进来

    [Header("Selection Anim")]
    public float selectedShiftX = -20f;       // 选中时向左偏移（负值向左）
    public float selectLerpSpeed = 10f;       // 插值速度（Time.unscaledDeltaTime）

    [Header("Target Panel (右侧信息区域脚本)")]
    public CharacterPanelController panel;

    LeftBarButton _current;
    readonly List<LeftBarButton> _buttons = new();

    // void Start()
    // {
    //     Build();
    //     // 默认选中第一个
    //     if (_buttons.Count > 0)
    //         OnButtonClicked(_buttons[0], true);
    // }

    // /// <summary>重建左侧按钮列</summary>
    // public void Build()
    // {
    //     // 清空旧
    //     if (content)
    //     {
    //         for (int i = content.childCount - 1; i >= 0; i--)
    //             Destroy(content.GetChild(i).gameObject);
    //     }
    //     _buttons.Clear();

    //     if (buttonPrefab == null || content == null || characters == null)
    //     {
    //         Debug.LogWarning("[LeftBar] 缺少必要引用：buttonPrefab/content/characters。");
    //         return;
    //     }

    //     foreach (var cd in characters)
    //     {
    //         if (cd == null) continue;

    //         // 实例化你的“条目根”预制体（根不是 Button）
    //         var go = Instantiate(buttonPrefab, content);

    //         // 在根或子物体里找到 LeftBarButton（你已经把这个脚本绑在条目根上了，也可绑在子物体上）
    //         var lb = go.GetComponent<LeftBarButton>();
    //         if (!lb) lb = go.GetComponentInChildren<LeftBarButton>(true);

    //         if (!lb)
    //         {
    //             Debug.LogError("[LeftBar] 预制体里找不到 LeftBarButton 组件。请把脚本绑在条目根或它的子物体上。");
    //             continue;
    //         }

    //         // 把“左移距离/插值速度”喂给条目（让它能访问）
    //         lb.Init(this, cd, cd.displayName);

    //         _buttons.Add(lb);
    //     }
    // }

    // /// <summary>条目按钮回调</summary>
    // public void OnButtonClicked(LeftBarButton b, bool instant = false)
    // {
    //     _current = b;
    //     foreach (var x in _buttons)
    //         x.SetSelected(x == b, instant);

    //     if (panel && b != null)
    //         panel.ShowCharacter(b.data);
    // }

    // // 让 LeftBarButton 能读取到动画参数
    // public float GetShiftX() => selectedShiftX;
    // public float GetLerpSpeed() => selectLerpSpeed;
    void Start()
    {
        Build();
        if (_buttons.Count > 0) OnButtonClicked(_buttons[0], true);
    }

    void Build()
    {
        foreach (Transform t in content) Destroy(t.gameObject);
        _buttons.Clear();

        foreach (var cd in characters)
        {
            // 实例化空物体根预制体
            var go = Instantiate(buttonPrefab, content);

            // 在根或子物体上拿 LeftBarButton 组件
            var b = go.GetComponent<LeftBarButton>() ?? go.GetComponentInChildren<LeftBarButton>(true);
            if (b == null)
            {
                Debug.LogError("[LeftBar] 预制体里找不到 LeftBarButton 组件");
                continue;
            }

            b.Init(this, cd);     // ← 现在传的是组件，不是 GameObject
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

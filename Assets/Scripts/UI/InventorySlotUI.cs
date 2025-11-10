using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class InventorySlotUI : MonoBehaviour
{
    [Header("Slot visuals")]
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;     // 不要可以留空
    [SerializeField] TMP_Text countText;    // 不要可以留空

    [Header("Inline action panel (child of slot)")]
    [SerializeField] RectTransform actionPanel; // 拖到 ActionPanel
    [SerializeField] Button btnUse;
    [SerializeField] Button btnCombine;
    [SerializeField] Button btnInspect;

    InventorySlot slot;
    RectTransform self;

    // 供 InventoryUI 传入的“选中回调”
    System.Action<InventorySlot> onSelected;

    // 记录当前展开的那个格子，保证同一时间只展开一个
    static InventorySlotUI s_open;

    void Awake()
    {
        self = (RectTransform)transform;
        HidePanel();
    }

    public void Setup(InventorySlot s, System.Action<InventorySlot> onSelected)  
    {
        slot = s;
        this.onSelected = onSelected;

        if (s == null || s.item == null)
        {
            Clear();
            return;
        }

        if (icon) { icon.enabled = true; icon.sprite = s.item.icon; }
        if (nameText)  nameText.text  = ""; // 你说现在不显示
        if (countText) countText.text = "";

        // 面板按钮只做占位
        if (btnUse)
        {
            btnUse.onClick.RemoveAllListeners();
            btnUse.onClick.AddListener(() => {
                Debug.Log($"[Bag] 使用：{slot.item.itemName}");
                HidePanel();
            });
        }
        if (btnCombine)
        {
            btnCombine.onClick.RemoveAllListeners();
            bool show = slot.item.combineThreshold > 0;
            btnCombine.gameObject.SetActive(show);
            btnCombine.onClick.AddListener(() => {
                Debug.Log($"[Bag] 合成：{slot.item.itemName}");
                HidePanel();
            });
        }
        if (btnInspect)
        {
            btnInspect.onClick.RemoveAllListeners();
            btnInspect.onClick.AddListener(() => {
                Debug.Log($"[Bag] 询问：{slot.item.itemName}");
                HidePanel();
            });
        }

        HidePanel(); // 刷新时先收起
    }

    public void OnClick()  // 把 Slot 的 Button.onClick 绑到它
    {
        if (slot == null || slot.item == null) return;

        onSelected?.Invoke(slot);

        // 已有别的格子展开，则先收起
        if (s_open && s_open != this) s_open.HidePanel();

        // 自己切换显示
        if (actionPanel && !actionPanel.gameObject.activeSelf)
        {
            ShowPanel();
            s_open = this;
        }
        else
        {
            HidePanel();
            if (s_open == this) s_open = null;
        }
    }

    void ShowPanel()
    {
        if (!actionPanel) return;

        // 作为自己的子物体，并贴在右侧
        // actionPanel.SetParent(self, false);
        // actionPanel.pivot = new Vector2(0f, 0.5f);
        // actionPanel.anchorMin = new Vector2(0f, 0.5f);
        // actionPanel.anchorMax = new Vector2(0f, 0.5f);
        // actionPanel.anchoredPosition = new Vector2(self.rect.width + 8f, 0f); // 右侧 8px

        actionPanel.gameObject.SetActive(true);
        actionPanel.SetAsLastSibling(); // 不被 Icon 遮
    }

    void HidePanel()
    {
        if (actionPanel) actionPanel.gameObject.SetActive(false);
    }

    public void Clear()
    {
        if (nameText)  nameText.text  = "";
        if (countText) countText.text = "";
        if (icon) { icon.sprite = null; icon.enabled = false; }
        HidePanel();
        if (s_open == this) s_open = null;
        slot = null;
    }
}
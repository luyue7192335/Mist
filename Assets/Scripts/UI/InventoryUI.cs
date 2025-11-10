using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject inventoryPanel;   // bagPanel 自己

    [Header("Grid")]
    public Transform slotContainer;     // 放格子的容器（GridLayoutGroup）
    public GameObject slotPrefab;       // 格子预制体（含 InventorySlotUI）
    public int pageSize = 16;

    [Header("Right Detail")]
    public Image detailIcon;            // 右侧图
    public TMP_Text nameText;           // 名称
    public TMP_Text descText;           // 描述
    public TMP_Text countText;          // 数量（可留空）
    public GameObject detailPanel;      // 整个右侧面板（可选，控制显隐）

    [Header("Paging UI ")]
    public TMP_Text pageText;            
    public Button btnPrev, btnNext;

    private readonly List<InventorySlotUI> uiSlots = new();
    private int pageIndex = 0;
    private ItemCategory? currentFilter = null; // null=全部；否则只显示某类


    void Start()
    {
        // 生成固定数量的格子（=pageSize）
        for (int i = 0; i < pageSize; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            var ui = go.GetComponent<InventorySlotUI>();
            uiSlots.Add(ui);
        }

        // if (inventoryPanel) inventoryPanel.SetActive(false);
        // if (detailPanel) detailPanel.SetActive(false);

        if (InventoryManager.Instance)
            InventoryManager.Instance.OnChanged += RefreshUI;

        // 翻页按钮
        if (btnPrev) btnPrev.onClick.AddListener(PrevPage);
        if (btnNext) btnNext.onClick.AddListener(NextPage);

        Filter_道具();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance)
            InventoryManager.Instance.OnChanged -= RefreshUI;
    }

    public void Toggle()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf) { pageIndex = 0; RefreshUI(); }
        else ClearDetail();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B)) Toggle();
    }


    public void Filter_All()      { currentFilter = null; pageIndex = 0; RefreshUI(); }
    public void Filter_道具()     { currentFilter = ItemCategory.道具; pageIndex = 0; RefreshUI(); }
    public void Filter_特殊()     { currentFilter = ItemCategory.特殊; pageIndex = 0; RefreshUI(); }
    public void Filter_消耗()     { currentFilter = ItemCategory.消耗; pageIndex = 0; RefreshUI(); }

    // public void RefreshUI()
    // {
    //     var data = InventoryManager.Instance.slots;

    //     for (int i = 0; i < slots.Count; i++)
    //     {
    //         if (i < data.Count)
    //             // 传“选中回调”给格子
    //             slots[i].Setup(data[i], OnSlotSelected);
    //         else
    //             slots[i].Clear();
    //     }

    //     // 打开背包时，默认清空右侧或选中第一个
    //     if (detailPanel) detailPanel.SetActive(false);
    //     ClearDetail();
    // }

    public void RefreshUI()
    {
        var all = InventoryManager.Instance.slots;

        // 过滤
        IEnumerable<InventorySlot> filtered = all;
        if (currentFilter.HasValue)
            filtered = all.Where(s => s.item && s.item.category == currentFilter.Value);

        var list = filtered.ToList();

        // 分页
        int totalPages = Mathf.Max(1, Mathf.CeilToInt(list.Count / (float)pageSize));
        pageIndex = Mathf.Clamp(pageIndex, 0, totalPages - 1);

        int start = pageIndex * pageSize;
        var pageItems = list.Skip(start).Take(pageSize).ToList();

        // 填充到 12 个格子
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < pageItems.Count)
                uiSlots[i].Setup(pageItems[i], OnSlotSelected); // ← 两参 Setup
            else
                uiSlots[i].Clear();
        }

        // 页码 UI
        if (pageText) pageText.text = $"{(totalPages == 0 ? 0 : pageIndex + 1)}/{totalPages}";
        if (btnPrev) btnPrev.interactable = (pageIndex > 0);
        if (btnNext) btnNext.interactable = (pageIndex < totalPages - 1);

        // 打开背包/切页/筛选时，先清空右侧详情
        if (detailPanel) detailPanel.SetActive(false);
        ClearDetail();
    }

    void NextPage() { pageIndex++; RefreshUI(); }
    void PrevPage() { pageIndex--; RefreshUI(); }

    void OnSlotSelected(InventorySlot s)
    {
        // 刷新右侧详情
        if (s == null || s.item == null) { ClearDetail(); return; }

        if (detailPanel) detailPanel.SetActive(true);

        if (detailIcon)
        {
            detailIcon.enabled = true;
            detailIcon.sprite = s.item.icon;
        }
        if (nameText)  nameText.text = s.item.itemName;
        if (descText)  descText.text = s.item.description;

        if (countText)
        {
            if (s.item.combineThreshold > 0)
                countText.text = $"{Mathf.Min(s.count, s.item.combineThreshold)} / {s.item.combineThreshold}";
            else
                countText.text = (s.count > 1) ? $"x{s.count}" : "x1";
        }
    }

    void ClearDetail()
    {
        if (detailIcon) { detailIcon.enabled = false; detailIcon.sprite = null; }
        if (nameText)  nameText.text = "";
        if (descText)  descText.text = "";
        if (countText) countText.text = "";
    }
    
    }
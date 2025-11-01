using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
public GameObject inventoryPanel;        // 背包主界面
    public Transform slotContainer;          // 格子容器
    public GameObject slotPrefab;            // 格子预制体

    //private List<InventorySlotUI> slots = new List<InventorySlotUI>();
    private List<InventorySlotUI> slots = new();
    private int slotCount = 12;

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotContainer);
            var ui = slotObj.GetComponent<InventorySlotUI>();
            slots.Add(ui);
        }
        inventoryPanel.SetActive(false);

        // 自动刷新（加/删物品时）
        if (InventoryManager.Instance)
            InventoryManager.Instance.OnChanged += RefreshUI;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance)
            InventoryManager.Instance.OnChanged -= RefreshUI;
    }

    // 你现在有 MainMenu 控制面板显示的话，这段 B 键可以保留或删除
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf) RefreshUI();
        }
    }

    public void RefreshUI()
    {
        var items = InventoryManager.Instance.slots;   // ★ 改：从 slots 取
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count) slots[i].Setup(items[i]);
            else                 slots[i].Clear();
        }
    }

    public void CloseInventory() => inventoryPanel.SetActive(false);
}
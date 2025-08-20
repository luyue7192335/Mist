using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
public GameObject inventoryPanel;        // 背包主界面
    public Transform slotContainer;          // 格子容器
    public GameObject slotPrefab;            // 格子预制体

    private List<InventorySlotUI> slots = new List<InventorySlotUI>();
    private int slotCount = 12;

    void Start()
    {
        // 初始化固定数量的槽位
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slots.Add(slotUI);
        }

        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("背包");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
                RefreshUI();
        }
    }

    public void RefreshUI()
    {
        var items = InventoryManager.Instance.inventoryItems;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
                slots[i].Setup(items[i]); // 显示物品
            else
                slots[i].Clear();         // 显示为空格子
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }
}

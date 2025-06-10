using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> inventoryItems = new List<ItemData>();

    void Awake() => Instance = this;

    public void AddItem(ItemData item)
    {
        if (!inventoryItems.Contains(item))
        {
            inventoryItems.Add(item);
            Debug.Log($"加入背包：{item.itemName}");
        }
    }

    public void UseItem(ItemData item)
    {
        if (!inventoryItems.Contains(item)) return;

        if (item.functions.Contains(ItemFunction.UsableInInventory))
        {
            // 自定义用途逻辑
            Debug.Log($"使用了：{item.itemName}");
        }
    }

    public void DeliverItem(ItemData item)
    {
        if (!inventoryItems.Contains(item)) return;

        if (item.functions.Contains(ItemFunction.Deliverable))
        {
            inventoryItems.Remove(item);
            Debug.Log($"交付了：{item.itemName}");
        }
    }

    public bool HasItem(string itemId)
    {
        return inventoryItems.Exists(item => item.itemId == itemId);
    }
    public void RemoveItem(string id)
    {
        ItemData itemToRemove = inventoryItems.FirstOrDefault(item => item.itemId == id);
        if (itemToRemove != null)
        {
            inventoryItems.Remove(itemToRemove);
        }
    }


}

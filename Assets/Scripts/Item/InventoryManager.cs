using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class InventoryManager : MonoBehaviour
{
public static InventoryManager Instance;

    public List<ItemData> inventoryItems = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ItemData item)
    {
        if (item == null) return;
        if (!inventoryItems.Contains(item))
            inventoryItems.Add(item);
        Debug.Log($"加入背包：{item.itemName}");
    }

    public void UseItem(ItemData item)
    {
        if (item == null || !inventoryItems.Contains(item)) return;
        // 你的具体逻辑……
        Debug.Log($"使用了：{item.itemName}");
    }

    public void DeliverItem(ItemData item)
    {
        if (item == null || !inventoryItems.Contains(item)) return;
        inventoryItems.Remove(item);
        Debug.Log($"交付了：{item.itemName}");
    }

    public bool HasItem(string itemId) =>
        inventoryItems.Any(it => it && it.itemId == itemId);

    public void RemoveItem(string id)
    {
        var it = inventoryItems.FirstOrDefault(x => x && x.itemId == id);
        if (it != null) inventoryItems.Remove(it);
    }

    public List<string> GetAllItemIds()
    {
        var ids = new List<string>();
        foreach (var it in inventoryItems)
            if (it && !string.IsNullOrEmpty(it.itemId))
                ids.Add(it.itemId);
        return ids;
    }

    public void SetByItemIds(List<string> ids)
    {
        inventoryItems.Clear();
        if (ids == null) return;
        foreach (var id in ids)
        {
            var item = ItemDatabase.Instance.GetItemById(id);
            if (item != null) inventoryItems.Add(item);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("物品数据表（拖 ScriptableObject）")]
    public ItemDatabase database;

    [Header("当前背包")]
    public List<InventorySlot> slots = new();

    public event Action OnChanged; // UI 刷新订阅

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!database) database =FindObjectOfType<ItemDatabase>();
    }

    // ========== 查询 ==========
    public bool HasItem(string itemId, int needCount = 1)
    {
        int have = slots.Where(s => s.item && s.item.itemId == itemId).Sum(s => s.count);
        return have >= needCount;
    }
    public int GetCount(string itemId) =>
        slots.Where(s => s.item && s.item.itemId == itemId).Sum(s => s.count);

    // —— 添加 —— 
    public void AddItem(ItemData item, int count = 1)
    {
        if (!item || count <= 0) return;

        if (item.stackable)
        {
            var s = slots.FirstOrDefault(x => x.item == item);
            if (s == null) { s = new InventorySlot { item = item, count = 0 }; slots.Add(s); }
            s.count = Mathf.Clamp(s.count + count, 0, item.maxStack);
        }
        else
        {
            for (int i = 0; i < count; i++)
                slots.Add(new InventorySlot { item = item, count = 1 });
        }

        TryAutoCombineIfNeeded(item);   // 留出合成可能
        OnChanged?.Invoke();
    }
    public void AddItemById(string id, int count = 1)
    {
        var it = database ? database.GetItemById(id) : null;
        if (it) AddItem(it, count);
        else Debug.LogWarning($"[Inventory] 未在数据库中找到物品ID：{id}");

    }

    // —— 移除/交付 —— 
    public void RemoveItem(string itemId, int count = 1)
    {
        if (count <= 0) return;
        for (int i = slots.Count - 1; i >= 0 && count > 0; i--)
        {
            var s = slots[i];
            if (!s.item || s.item.itemId != itemId) continue;
            int take = Mathf.Min(s.count, count);
            s.count -= take;
            count   -= take;
            if (s.count <= 0) slots.RemoveAt(i);
        }
        OnChanged?.Invoke();
    }

    // —— 兼容你以前的调用 —— 
    public void AddItem(ItemData item) => AddItem(item, 1);
    public bool HasItem(ItemData item)  => item && HasItem(item.itemId, 1);
    public void UseItem(ItemData item)  // 供你旧的 UI 调用
    {
        if (!item || !item.usableInBag) return;
        Debug.Log($"使用了：{item.itemName}");
        RemoveItem(item.itemId, 1);
        // TODO：在这里触发具体效果/Ink/数值变化
    }

    // —— 预留：自动合成（有阈值时可自动触发） —— 
    void TryAutoCombineIfNeeded(ItemData source)
    {
        if (source == null || source.combineThreshold <= 0 || string.IsNullOrEmpty(source.combineResultId)) return;
        var s = slots.FirstOrDefault(x => x.item == source);
        if (s == null || s.count < source.combineThreshold) return;

        int times = s.count / source.combineThreshold;
        s.count -= times * source.combineThreshold;
        if (s.count <= 0) slots.Remove(s);

        var result = database ? database.GetItemById(source.combineResultId) : null;
        if (result) AddItem(result, times); // 会再次触发 OnChanged
    }

    [Serializable] public class SaveEntry { public string id; public int count; }
    public List<SaveEntry> ExportSave() =>
        slots.Where(s => s.item).Select(s => new SaveEntry { id = s.item.itemId, count = s.count }).ToList();

    public void ImportSave(List<SaveEntry> data)
    {
        slots.Clear();
        if (data != null)
            foreach (var e in data)
            {
                var it = database ? database.GetItemById(e.id) : null;
                if (it != null) slots.Add(new InventorySlot { item = it, count = e.count });
            }
        OnChanged?.Invoke();
    }

    // ====== 兼容你 SaveLoadManager 旧接口（ID 列表）======
    // 旧的 GetAllItemIds：返回“按数量重复的ID列表”
    public List<string> GetAllItemIds()
    {
        var ids = new List<string>();
        foreach (var s in slots)
            if (s.item != null)
                for (int i = 0; i < Mathf.Max(0, s.count); i++)
                    ids.Add(s.item.itemId);
        return ids;
    }

    // 旧的 SetByItemIds：根据ID列表重建（会自动聚合为数量）
    public void SetByItemIds(List<string> ids)
    {
        slots.Clear();
        if (ids != null)
            foreach (var id in ids)
            {
                var it = database ? database.GetItemById(id) : null;
                if (it != null) AddItem(it, 1);
            }
        OnChanged?.Invoke();
    }

    // 分类取
    public IEnumerable<InventorySlot> GetByCategory(ItemCategory cat) =>
        slots.Where(s => s.item && s.item.category == cat);
}
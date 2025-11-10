using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<ItemData> allItems;

    Dictionary<string, ItemData> map;

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        map = new Dictionary<string, ItemData>();
        foreach (var it in allItems) if (it != null && !string.IsNullOrEmpty(it.itemId))
            map[it.itemId] = it;
    }

    public ItemData GetItemById(string id)
        => (id != null && map.TryGetValue(id, out var it)) ? it : null;
}

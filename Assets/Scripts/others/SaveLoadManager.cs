using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [Header("References (手动拖拽)")]
    public InkManager inkManager;                 // 你的对话场景用的
    public InkManager_Explore inkExploreManager;  // 你的探索场景用的（有就拖）
    //public PlayerStats playerStats;               // 玩家数值
    public InventoryManager inventoryManager;     // 背包
    public MapManager mapManager;                 // 地图页管理
    public MapConditionManager mapCondition;      // 地图解锁条件

    // 路径：persistentDataPath/Saves/slotX.json
    string Root => Path.Combine(Application.persistentDataPath, "Saves");
    string SlotPath(int slotIndex) => Path.Combine(Root, $"slot{slotIndex}.json");
    void EnsureFolder() { if (!Directory.Exists(Root)) Directory.CreateDirectory(Root); }

    // ---------- 保存 ----------
    public void SaveGame(int slot)
    {
        EnsureFolder();
        var data = new SaveData();

        // 1) Ink 状态：哪个在用就存哪个（两者都可能为空，只要二选一就行）
        var story = TryGetStory();
        if (story != null)
            data.inkJSONState = story.state.ToJson();
        else
            data.inkJSONState = ""; // 没在对话也没关系

        // 2) 玩家数值
        // if (playerStats != null)
        // {
        //     data.hp         = playerStats.hp;
        //     data.sanity     = playerStats.sanity;
        //     data.trustValue = playerStats.trustValue;
        //     data.evilValue  = playerStats.evilValue;
        // }

        // 3) 物品（存ID）
        if (inventoryManager != null)
            data.inventoryItemIds = inventoryManager.GetAllItemIds();
        else
            data.inventoryItemIds = new List<string>();

        // 4) 地图
        if (mapManager != null)
            data.currentMapIndex = mapManager.currentMapIndex;

        if (mapCondition != null)
            data.unlockedMapIndices = mapCondition.GetUnlockedList();
        else
            data.unlockedMapIndices = new List<int>();

        // 5) 其他信息
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        data.saveTime  = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SlotPath(slot), json);
        Debug.Log($"保存成功：{SlotPath(slot)}\n{json}");
    }

    // ---------- 读取 ----------
    public void LoadGame(int slot)
    {
        var path = SlotPath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"存档不存在：{path}");
            return;
        }

        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);

        // 1) Ink
        var story = TryGetStory();
        if (story != null && !string.IsNullOrEmpty(data.inkJSONState))
            story.state.LoadJson(data.inkJSONState);

        // 2) 玩家数值
        // if (playerStats != null)
        // {
        //     playerStats.hp         = data.hp;
        //     playerStats.sanity     = data.sanity;
        //     playerStats.trustValue = data.trustValue;
        //     playerStats.evilValue  = data.evilValue;

        //     // 如果你有 UI 刷新函数，这里调用一下
        //     // playerStats.Apply();
        // }

        // 3) 物品
        if (inventoryManager != null)
            inventoryManager.SetByItemIds(data.inventoryItemIds);

        // 4) 地图
        if (mapCondition != null)
            mapCondition.RestoreUnlocked(data.unlockedMapIndices);

        if (mapManager != null)
            mapManager.ShowMap(data.currentMapIndex);

        Debug.Log($"读档成功：{path}");
    }

    // 从两个 Ink 管理器里拿 Story（哪个活跃用哪个）
    Ink.Runtime.Story TryGetStory()
    {
        if (inkManager != null && inkManager.story != null) return inkManager.story;
        if (inkExploreManager != null && inkExploreManager.story != null) return inkExploreManager.story;
        return null;
    }
}

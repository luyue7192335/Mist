using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{

    public string sceneId;
    public string spawnId;
    public float playerX, playerY;

    public int trust, favor, sanity, hp, evil;
    public Dictionary<string, bool> flags = new();
    public List<InvEntry> inventory = new();

    // Ink
    public string inkStateJson = null;

    public string note;
    public long   unixTime;

    
    // 1) Ink 状态（SaveLoadManager 用的是这个名字）
    public string inkJSONState = "";              // story.state.ToJson()

    // 2) 背包（SaveLoadManager 目前只存 ID 列表）
    public List<string> inventoryItemIds = new(); // InventoryManager.GetAllItemIds()

    // 3) 地图/解锁
    public int currentMapIndex = 0;
    public List<int> unlockedMapIndices = new();

    // 4) 额外信息
    public string sceneName = "";
    public string saveTime  = "";                 // "yyyy-MM-dd HH:mm:ss"
}

[Serializable]
public class InvEntry { public string id; public int count; }

public static class SaveSystem
{
    public const int MaxSlots = 15;

    static string Dir => Path.Combine(Application.persistentDataPath, "saves");
    static string SlotPath(int idx) => Path.Combine(Dir, $"save_{idx:D2}.json");

    public static bool Exists(int idx) => File.Exists(SlotPath(idx));

    public static void Delete(int idx)
    {
        var p = SlotPath(idx);
        if (File.Exists(p)) File.Delete(p);
    }

    public static void Save(int idx, SaveData data)
    {
        if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
        data.unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SlotPath(idx), json);
    }

    public static SaveData Load(int idx)
    {
        var p = SlotPath(idx);
        if (!File.Exists(p)) return null;
        var json = File.ReadAllText(p);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static (bool has, string timeStr, string note) Summary(int idx)
    {
        var d = Load(idx);
        if (d == null) return (false, "", "");
        var local = DateTimeOffset.FromUnixTimeSeconds(d.unixTime).ToLocalTime().DateTime;
        return (true, local.ToString("dd.MM.yyyy. HH:mm"), d.note ?? "");
    }
}

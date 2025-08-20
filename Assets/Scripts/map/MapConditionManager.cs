using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConditionManager : MonoBehaviour
{
    public static MapConditionManager Instance;
    private HashSet<int> unlockedMaps = new();
    public List<int> GetUnlockedList() => new List<int>(unlockedMaps);

    void Awake(){ Instance = this; }

    public void MarkExplorationDone(int mapIndex)
    {
        unlockedMaps.Add(mapIndex);
        Debug.Log($"探索完成：地图 {mapIndex}");
    }

    public bool CanEnter(int mapIndex)
    {
        return unlockedMaps.Contains(mapIndex - 1);
    }

    // ★ 导出为列表
   
    // ★ 读档恢复
    public void RestoreUnlocked(List<int> list)
    {
        unlockedMaps = new HashSet<int>(list ?? new List<int>());
    }


}

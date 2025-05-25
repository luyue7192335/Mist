using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConditionManager : MonoBehaviour
{
    public static MapConditionManager Instance;

    private HashSet<int> unlockedMaps = new HashSet<int>();

    void Awake()
    {
        Instance = this;
    }

    public void MarkExplorationDone(int mapIndex)
    {
        unlockedMaps.Add(mapIndex);
        Debug.Log($"探索完成：地图 {mapIndex}");
    }

    public bool CanEnter(int mapIndex)
    {
        return unlockedMaps.Contains(mapIndex - 1); // 要完成前一张地图才能进
    }
}

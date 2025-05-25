using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public List<GameObject> maps;   // 所有地图块
    private int currentMapIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowMap(currentMapIndex); // 初始显示第一个地图
    }

    public void ShowMap(int index)
    {
        for (int i = 0; i < maps.Count; i++)
        {
            maps[i].SetActive(i == index);
        }

        currentMapIndex = index;
    }

    public void NextMap()
    {
        int nextIndex = currentMapIndex + 1;

        if (nextIndex < maps.Count)
            ShowMap(nextIndex);
    }

    public void PreviousMap()
    {
        int prevIndex = currentMapIndex - 1;

        if (prevIndex >= 0)
            ShowMap(prevIndex);
    }
}

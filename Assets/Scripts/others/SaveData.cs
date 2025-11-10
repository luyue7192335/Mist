using System;
using System.Collections.Generic;


[System.Serializable]
public class SaveData
{
    public string inkJSONState;

    public int hp;
    public int sanity;
    public int trustValue;
    public int evilValue;

    public List<string> inventoryItemIds = new();

    public int currentMapIndex;
    public List<int> unlockedMapIndices = new();

    public string sceneName;
    public string saveTime;
}
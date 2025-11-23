using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string charId;
    public string displayName;
    public Sprite portrait;
    [TextArea] public string intro;

    public List<CharacterStoryEntry> stories = new();
}

[Serializable]
public class CharacterStoryEntry
{
    public string entryId;
    [TextArea] public string lockedHint;
    [TextArea] public string content;
    public List<UnlockCondition> conditions = new();
}

public enum CondType { FlagOn, FlagOff, HasItem, StatGE, StatLE }

[Serializable]
public class UnlockCondition
{
    public CondType type;
    public string key;
    public int value;

    public bool IsMet()
    {
        switch (type)
        {
            case CondType.FlagOn:  return StoryFlags.Instance && StoryFlags.Instance.IsOn(key);
            case CondType.FlagOff: return StoryFlags.Instance && !StoryFlags.Instance.IsOn(key);
            case CondType.HasItem: return InventoryManager.Instance && InventoryManager.Instance.HasItem(key, 1);
            case CondType.StatGE:  return GetStat(key) >= value;
            case CondType.StatLE:  return GetStat(key) <= value;
            default: return false;
        }
    }

    int GetStat(string name)
    {
        var gs = GameStats.Instance;
        if (gs == null) return 0;
        switch (name.ToLower())
        {
            case "trust":  return gs.trust;
            case "favor":  return gs.favor;
            case "sanity": return gs.sanity;
            case "hp":     return gs.hp;
            case "evil":   return gs.evil;
            default:       return 0;
        }
    }
}


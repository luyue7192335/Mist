using UnityEngine;

public enum ItemFunction
{
    None,
    TriggerDialogue,
    CanBeStored,
    UsableInInventory,
    Deliverable
}

public enum ItemCategory { 道具, 特殊, 消耗 }

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;               // 唯一ID（英文/拼音）
    public string itemName;             // 显示名
    [TextArea] public string description;
    public Sprite icon;

    public ItemCategory category = ItemCategory.道具;

    // 数量与叠加
    public bool stackable = true;
    public int  maxStack = 99;

    // 可选：碎片式收集
    public int combineThreshold = 0;    // 0=不是碎片，>0=收集阈值
    public string combineResultId;      // 合成产物（可留空，先不实现合成）

    // 背包操作开关（先最小化）
    public bool usableInBag = false;
    public bool deliverable = true;
}
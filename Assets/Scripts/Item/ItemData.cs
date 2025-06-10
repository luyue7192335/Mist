using UnityEngine;

public enum ItemFunction
{
    None,
    TriggerDialogue,
    CanBeStored,
    UsableInInventory,
    Deliverable
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string itemName;
    public Sprite icon;
    public ItemFunction[] functions;
    public TextAsset dialogueFile;  // 若用于触发 Ink 对话
    public string requiredItemId;   // 若需要交付某物品
}
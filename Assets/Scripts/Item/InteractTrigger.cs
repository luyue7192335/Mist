using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractTrigger : MonoBehaviour
{
[Header("高亮（可选）")]
    public GameObject highlightVisual;

    [Header("剧情绑定")]
    public TextAsset dialogueFile;             // 正常剧情
    public TextAsset fallbackDialogueFile;     // 缺少物品时的替代剧情

    [Header("交互条件")]
    public string requiredItemId;              // 若需交付物品，写其ID
    public bool consumeRequiredItem = true;    // 是否交付后删除物品

    [Header("交互后给予物品")]
    public bool giveItemAfterInteraction = false;
    public ItemData itemToGive;

    [Header("交互后是否销毁物体")]
    public bool destroyAfterInteraction = false;

    [Header("地图探索推进")]
    public bool triggersMapProgress = false;
    public int mapIndex = 0; // 若以后改成场景key，可改成string

    [Header("点击距离限制")]
    public Transform interactionCenter;   // 若空则用 transform
    public float maxClickDistance = 1.6f;

    public void SetHighlighted(bool on)
    {
        if (highlightVisual) highlightVisual.SetActive(on);
    }

    public bool CanInteract(GameObject player)
    {
        if (!string.IsNullOrEmpty(requiredItemId))
        {
            if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(requiredItemId))
                return false;
        }
        return true;
    }

    public void Interact(GameObject player)
    {
        // 物品条件
        if (!string.IsNullOrEmpty(requiredItemId))
        {
            if (InventoryManager.Instance.HasItem(requiredItemId))
            {
                if (consumeRequiredItem)
                    InventoryManager.Instance.RemoveItem(requiredItemId);

                PlayStory(dialogueFile);
            }
            else
            {
                PlayStory(fallbackDialogueFile);
                return; // 缺物品则结束，不发奖励
            }
        }
        else
        {
            PlayStory(dialogueFile);
        }

        // 发奖励物品
        if (giveItemAfterInteraction && itemToGive != null)
            InventoryManager.Instance.AddItem(itemToGive);

        // 地图推进
        if (triggersMapProgress && MapConditionManager.Instance != null)
            MapConditionManager.Instance.MarkExplorationDone(mapIndex);

        // 自毁
        if (destroyAfterInteraction)
            Destroy(gameObject);
    }

    void PlayStory(TextAsset inkJson)
    {
        if (inkJson != null) InkManager_Explore.Instance.StartStory(inkJson);
        else Debug.LogWarning("未绑定Ink JSON文件！");
    }

    // — 鼠标点击（距离限制）—
    void OnMouseDown()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;
        Vector3 c = interactionCenter ? interactionCenter.position : transform.position;
        if (Vector2.Distance(player.transform.position, c) > maxClickDistance) return;

        if (CanInteract(player)) Interact(player);
    }
}

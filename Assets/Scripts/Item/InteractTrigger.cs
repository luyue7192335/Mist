using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractTrigger : MonoBehaviour
{
    [Header("剧情绑定")]
    public TextAsset dialogueFile;             // 正常剧情
    public TextAsset fallbackDialogueFile;     // 缺少物品时的替代剧情

    [Header("交互条件")]
    public string requiredItemId;              // 若需交付物品，写其ID
    public bool consumeRequiredItem = true;    // 是否交付后删除物品

    [Header("地图探索推进")]
    public bool triggersMapProgress = false;
    public int mapIndex = 0;

    [Header("UI 提示")]
    public GameObject hintImage;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Debug.Log("按下F，尝试互动");

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
                Debug.Log("缺少所需物品，播放备用剧情");
                PlayStory(fallbackDialogueFile);
            }
        }
        else
        {
            PlayStory(dialogueFile);
        }

        if (hintImage != null)
            hintImage.SetActive(false);

        if (triggersMapProgress)
            MapConditionManager.Instance.MarkExplorationDone(mapIndex);
    }

    private void PlayStory(TextAsset inkJson)
    {
        if (inkJson != null)
        {
            InkManager_Explore.Instance.StartStory(inkJson);
        }
        else
        {
            Debug.LogWarning("未绑定Ink JSON文件！");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (hintImage != null)
                hintImage.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (hintImage != null)
                hintImage.SetActive(false);
        }
    }
}

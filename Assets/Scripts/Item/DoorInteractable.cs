using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("目标场景")]
    public string targetScenePath = "Scenes/Map_RoomB";
    public string targetSpawnPoint = "Spawn_Default";

    [Header("进入条件（可选）")]
    public string requireItemId;          // 需要某道具（留空则不需要）
    public bool consumeItem = true;
    public string requireFlagKey;         // 需要某剧情开关（留空则不需要）

    [Header("高亮/描边（可选）")]
    public GameObject highlightVisual;

    // [Header("点击距离限制")]
    // public Transform interactionCenter;   // 若空则用 transform
    // public float maxClickDistance = 1.6f; // 防止隔屏点击

    public void OnInteract(GameObject player)
    {
        // 条件检查
        if (!string.IsNullOrEmpty(requireItemId))
        {
            if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(requireItemId))
            {
                Debug.Log("[Door] 缺少物品，不能进入");
                return;
            }
            if (consumeItem) InventoryManager.Instance.RemoveItem(requireItemId);
        }
        if (!string.IsNullOrEmpty(requireFlagKey))
        {
            if (StoryFlags.Instance == null || !StoryFlags.Instance.IsOn(requireFlagKey))
            {
                Debug.Log("[Door] 条件Flag未满足，不能进入");
                return;
            }
        }

        if (highlightVisual) highlightVisual.SetActive(false);
        Debug.Log("[Door] Interact -> Request transition");
        MapService.Instance.StartCoroutine(
            MapService.Instance.LoadMapAdditive(targetScenePath, targetSpawnPoint)
        );
    }

    IEnumerator DoTransition()
    {
        if (highlightVisual) highlightVisual.SetActive(false);
        yield return MapService.Instance.LoadMapAdditive(targetScenePath, targetSpawnPoint);
    }

    // —— 进入/离开范围时会调这个 —— 
    public void OnHighlight(bool on)
    {
        if (highlightVisual) highlightVisual.SetActive(on);
    }

    // （可选）鼠标点击也能触发
    void OnMouseDown()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player) OnInteract(player);
    }
}

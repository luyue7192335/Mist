using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;



public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    private ItemData itemData;
    public TextMeshProUGUI nameText;

    public void Setup(ItemData item)
    {
        itemData = item;
        icon.sprite = item.icon;
        nameText.text = item.itemName;
    }

    public void OnClickUse()
    {
        if (itemData.functions.Contains(ItemFunction.UsableInInventory))
        {
            InventoryManager.Instance.UseItem(itemData);
            Debug.Log("点击使用了物品：" + itemData.itemName);
        }
    }

        public void Clear()
    {
        nameText.text = "";
        icon.sprite = null;
        icon.enabled = false; // 或设置为灰色背景
    }
}

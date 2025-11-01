using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;



public class InventorySlotUI : MonoBehaviour
{
      public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;    // 新增：右下角显示数量或 N/阈值

    private InventorySlot slot;          // 保存引用，点击时用
    public void Setup(InventorySlot s)
    {
        slot = s;
        if (icon) { icon.enabled = true; icon.sprite = s.item.icon; }
        if (nameText) nameText.text = s.item.itemName;

        if (countText)
        {
            if (s.item.combineThreshold > 0)
                countText.text = $"{Mathf.Min(s.count, s.item.combineThreshold)}/{s.item.combineThreshold}";
            else
                countText.text = s.count > 1 ? s.count.ToString() : "";
        }
    }

    public void OnClickUse()
    {
        if (slot == null || slot.item == null) return;
        if (slot.item.usableInBag)
        {
            InventoryManager.Instance.UseItem(slot.item);
            Debug.Log("点击使用了物品：" + slot.item.itemName);
        }
        else
        {
            Debug.Log("该物品不能在背包中使用：" + slot.item.itemName);
        }
    }

    public void Clear()
    {
        if (nameText) nameText.text = "";
        if (countText) countText.text = "";
        if (icon) { icon.sprite = null; icon.enabled = false; }
        slot = null;
    }
}

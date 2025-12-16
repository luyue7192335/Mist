using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Button button;          // 覆盖整个槽位的按钮
    public TMP_Text timeText;      // 例：11.08.2025 23:37
    public TMP_Text noteText;      // 例：第一章·旧宅
    public GameObject selectedMask;// 被选中时显示
    
    public Image slotBg;           // 背景（别改 alpha）

    [HideInInspector] public int index;
    SaveSlotsPanel _owner;

    public void Init(int idx, SaveSlotsPanel owner)
    {
        index = idx;
        _owner = owner;
        if (button) button.onClick.AddListener(() => _owner.OnClickSlot(this));
        SetSelected(false);
    }

    public void SetData(bool hasData, string timeStr, string note)
    {
        if (timeText) timeText.text = hasData ? timeStr : "";
        if (noteText) noteText.text = hasData ? note : "";

        //if (emptyHint) emptyHint.SetActive(!hasData);
    }

    public void SetSelected(bool on)
    {
        if (selectedMask) selectedMask.SetActive(on);
    }
    }
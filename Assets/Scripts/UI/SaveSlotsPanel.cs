using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotsPanel : MonoBehaviour
{
    [Header("Build")]
    public Transform content;           // ScrollView/Viewport/Content
    public SaveSlotUI slotPrefab;       // 槽位预制体
    public int slotCount = 15;

    [Header("Confirm Dialog")]
    public GameObject confirmDialog;    // 图2的面板
    public TMP_Text confirmText;
    public Button btnOk, btnCancel;

    [Header("Right Buttons")]
    public Button btnSave, btnLoad, btnDelete;  // 右下角固定按钮

    [Header("Hook")]
    public SaveLoadManager saveLoad;

    private readonly List<SaveSlotUI> _slots = new();
    private int _selectedIndex = -1;
    private System.Action _pendingAction; // 用于确认框回调

    void Awake()
    {
        BuildSlots();
        WireRightButtons();
        HideConfirm();
        RefreshAll();
    }

    void BuildSlots()
    {
        // 清空旧
        foreach (Transform t in content) Destroy(t.gameObject);
        _slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            var s = Instantiate(slotPrefab, content);
            s.Init(i, this);
            _slots.Add(s);
        }
    }

    void WireRightButtons()
    {
        if (btnSave) btnSave.onClick.AddListener(() =>
        {
            if (_selectedIndex < 0) return;
            var has = SaveSystem.Exists(_selectedIndex);
            string msg = has ? "覆盖存档？" : "建立新存档？";
            ShowConfirm(msg, () => {
                saveLoad.SaveGame(_selectedIndex);
                RefreshAll();
            });
        });

        if (btnLoad) btnLoad.onClick.AddListener(() =>
        {
            if (_selectedIndex < 0 || !SaveSystem.Exists(_selectedIndex)) return;
            ShowConfirm("加载该存档并开始游戏？", () => {
                saveLoad.LoadGame(_selectedIndex);
            });
        });

        if (btnDelete) btnDelete.onClick.AddListener(() =>
        {
            if (_selectedIndex < 0 || !SaveSystem.Exists(_selectedIndex)) return;
            ShowConfirm("删除该存档？不可撤销。", () => {
                SaveSystem.Delete(_selectedIndex);
                RefreshAll();
            });
        });
    }

    public void OnClickSlot(SaveSlotUI slot)
    {
        _selectedIndex = slot.index;

        // 空槽位 → 弹确认保存
        if (!SaveSystem.Exists(_selectedIndex))
        {
            ShowConfirm("建立新存档？", () => {
                saveLoad.SaveGame(_selectedIndex);
                RefreshAll();
            });
        }
        else
        {
            // 已有存档 → 高亮槽位，让右下按钮处理
            RefreshHighlight();
        }
    }

    void RefreshAll()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var (has, timeStr, note) = SaveSystem.Summary(i);
            _slots[i].SetData(has, timeStr, note);
        }
        RefreshHighlight();
    }

    void RefreshHighlight()
    {
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].SetSelected(i == _selectedIndex);
    }

    // ===== 确认框 =====
    void ShowConfirm(string msg, System.Action ok)
    {
        _pendingAction = ok;
        if (confirmText) confirmText.text = msg;
        if (confirmDialog) confirmDialog.SetActive(true);

        if (btnOk)
        {
            btnOk.onClick.RemoveAllListeners();
            btnOk.onClick.AddListener(() => {
                _pendingAction?.Invoke();
                HideConfirm();
            });
        }
        if (btnCancel)
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(HideConfirm);
        }
    }

    void HideConfirm()
    {
        if (confirmDialog) confirmDialog.SetActive(false);
        _pendingAction = null;
    }
}

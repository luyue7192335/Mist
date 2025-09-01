using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
[Header("HUD / 主按钮条")]
    public GameObject hudBar;                 // MainCanvas/HUD/MainMenuBar（面板打开时可隐藏）

    [Header("按钮(点击打开)")]
    public Button btnInventory;
    public Button btnCharacter;
    public Button btnSettings;

    [Header("面板(初始隐藏) + 返回按钮")]
    public GameObject inventoryPanel;
    public Button    inventoryBack;
    public GameObject characterPanel;
    public Button    characterBack;
    public GameObject settingsPanel;
    public Button    settingsBack;

    [Header("面板打开时需禁用的脚本(可选)")]
    public MonoBehaviour[] playerInputToDisable; // 拖 PlayerController、InteractionController 等

    GameObject _openPanel;   // 当前打开的面板(为空表示都关闭)

    void Awake()
    {
        // 绑定点击事件
        if (btnInventory) btnInventory.onClick.AddListener(() => OpenExclusive(inventoryPanel));
        if (btnCharacter) btnCharacter.onClick.AddListener(() => OpenExclusive(characterPanel));
        if (btnSettings)  btnSettings .onClick.AddListener(() => OpenExclusive(settingsPanel));

        if (inventoryBack) inventoryBack.onClick.AddListener(CloseAll);
        if (characterBack) characterBack.onClick.AddListener(CloseAll);
        if (settingsBack)  settingsBack .onClick.AddListener(CloseAll);

        // 初始全部关闭
        CloseAll();
    }

    void Update()
    {
        // 仅 Esc 作为返回键
        if (_openPanel && Input.GetKeyDown(KeyCode.Escape))
            CloseAll();
    }

    void OpenExclusive(GameObject panel)
    {
        if (!panel) return;

        // 已经是这个面板 → 视为返回
        if (_openPanel == panel) { CloseAll(); return; }

        // 先关掉其它
        SetPanel(inventoryPanel, false);
        SetPanel(characterPanel, false);
        SetPanel(settingsPanel,  false);

        // 打开目标
        SetPanel(panel, true);
        _openPanel = panel;

        // 面板打开时：暂停 + 禁用玩家输入 + 隐藏 HUD（按需）
        Time.timeScale = 0f;
        SetPlayerInputs(false);
        if (hudBar) hudBar.SetActive(false);
    }

    public void CloseAll()
    {
        SetPanel(inventoryPanel, false);
        SetPanel(characterPanel, false);
        SetPanel(settingsPanel,  false);
        _openPanel = null;

        // 恢复
        Time.timeScale = 1f;
        SetPlayerInputs(true);
        if (hudBar) hudBar.SetActive(true);
    }

    void SetPanel(GameObject p, bool on)
    {
        if (p && p.activeSelf != on) p.SetActive(on);
    }

    void SetPlayerInputs(bool on)
    {
        if (playerInputToDisable == null) return;
        foreach (var mb in playerInputToDisable)
            if (mb) mb.enabled = on;
    }
}

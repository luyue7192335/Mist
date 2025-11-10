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

    public CanvasGroup dialogueGroup;

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
        if (_openPanel == panel) { CloseAll(); return; }

        SetPanel(inventoryPanel, false);
        SetPanel(characterPanel, false);
        SetPanel(settingsPanel,  false);

        SetPanel(panel, true);
        _openPanel = panel;

        Time.timeScale = 0f;
        SetPlayerInputs(false);
        if (hudBar) hudBar.SetActive(false);

        // ★ 关键：对话继续显示，但不拦鼠标/键盘射线
        if (dialogueGroup)
        {
            dialogueGroup.interactable   = false;
            dialogueGroup.blocksRaycasts = false;
            // 可选：半透明
            // dialogueGroup.alpha = 0.9f;
        }
    }

    public void CloseAll()
    {
        SetPanel(inventoryPanel, false);
        SetPanel(characterPanel, false);
        SetPanel(settingsPanel,  false);
        _openPanel = null;

        Time.timeScale = 1f;
        SetPlayerInputs(true);
        if (hudBar) hudBar.SetActive(true);

        // 恢复对话面板可交互
        if (dialogueGroup)
        {
            dialogueGroup.interactable   = true;
            dialogueGroup.blocksRaycasts = true;
            // dialogueGroup.alpha = 1f;
        }
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

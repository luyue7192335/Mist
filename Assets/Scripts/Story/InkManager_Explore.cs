using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems; 

public class InkManager_Explore : MonoBehaviour
{
     static readonly Regex ChoiceStyleRe = new Regex(@"\{style=(\w+)\}", RegexOptions.IgnoreCase);

     public static InkManager_Explore Instance;

    public TMP_Text dialogueText; // 对话文本
    //public Button choiceButtonPrefab; // 选项按钮预制体
    public Transform choicePanel; // 选项容器
    public GameObject dialogueUI; 
    public TMP_Text nameText;

    // 在类里加：把它们拖到 Inspector
    public Button choicePrefab_Default;
    public Button choicePrefab_Blue;
    public Button choicePrefab_Red;


    public Story story; // Ink 剧情数据
    public PlayerController playerController; // 玩家移动脚本

    public bool isStoryActive = false; // 判断是否正在进行剧情

    [SerializeField] UnityEngine.UI.Button clickButton;

    [Header("Dialogue Buttons")]
    [SerializeField] UnityEngine.UI.Button btnAuto;
    [SerializeField] UnityEngine.UI.Button btnSkip;
    [SerializeField] UnityEngine.UI.Image  imgAuto;
    [SerializeField] UnityEngine.UI.Image  imgSkip;

    [Header("Button Sprites")]
    [SerializeField] Sprite autoPlaySprite;   // 未播放：显示播放
    [SerializeField] Sprite autoPauseSprite;  // 播放中：显示暂停
    [SerializeField] Sprite skipIdleSprite;   // 跳过关闭

    [Header("Auto / Skip Params")]
    public bool  autoMode = false;
    public float autoDelay = 1.6f;

    public bool  skipMode = false;
    public float skipInterval = 0.02f;

    Coroutine _autoCo, _skipCo;

    // === 提供给按钮的回调 ===
    public void UI_OnClickAuto()
    {
        if (!isStoryActive) return;

        if (autoMode == true)
        {
            // 如果正在播放，点一下就明确停止
            StopAuto();
            RefreshButtonsVisual();
            Debug.Log("停止自动播放");
            return;
        }

        // 如果没在播放，先保证跳过已关，再开启自动
        StopSkip();
        autoMode = true;
        if (_autoCo != null) StopCoroutine(_autoCo);
        _autoCo = StartCoroutine(AutoLoop());
        Debug.Log("自动播放");
        RefreshButtonsVisual();
    }

    public void UI_OnClickSkip()
    {
        // 切换跳过；若正在自动播放，则先停自动
        if (!isStoryActive) return;
        skipMode = !skipMode;
        if (skipMode)
        {
            StopAuto();
            _skipCo = StartCoroutine(SkipLoop());
        }
        else
        {
            StopSkip();
        }
        RefreshButtonsVisual();
    }

    // === 协程：自动/跳过 ===
    IEnumerator AutoLoop()
    {
        while (isStoryActive && autoMode && story != null)
        {
            // 有选项就停（尊重玩家选择）
            if (story.currentChoices.Count > 0) { StopAuto(); break; }

            yield return new WaitForSeconds(autoDelay);

            if (!autoMode || story == null) break;
            if (story.currentChoices.Count == 0) DisplayNextLine();
            else { StopAuto(); break; }
        }
        RefreshButtonsVisual();
    }

    IEnumerator SkipLoop()
    {
        while (isStoryActive && skipMode && story != null)
        {
            // 有选项就停
            if (story.currentChoices.Count > 0) { StopSkip(); break; }

            DisplayNextLine();
            yield return new WaitForSeconds(skipInterval);
        }
        RefreshButtonsVisual();
    }

    void StopAuto()
    {
        if (_autoCo != null) StopCoroutine(_autoCo);
        _autoCo = null;
        autoMode = false;
    }

    void StopSkip()
    {
        if (_skipCo != null) StopCoroutine(_skipCo);
        _skipCo = null;
        skipMode = false;
    }

    // === 点击屏幕推进时，打断 Auto/Skip（玩家优先）===
 
  

    // === 你在 DisplayNextLine() 的最后，或 ShowChoices() 后面调用它 ===
    void RefreshButtonsVisual()
    {
        // 图标切换
        if (imgAuto)
            imgAuto.sprite = autoMode ? autoPauseSprite : autoPlaySprite;

        // if (imgSkip)
        //     imgSkip.sprite = (skipMode && skipActiveSprite) ? skipActiveSprite : skipIdleSprite;

        // 可点性：有选项时禁止自动/跳过按钮（也可选择只禁用跳过）
        bool hasChoices = (story != null && story.currentChoices.Count > 0);
        if (btnAuto) btnAuto.interactable = !hasChoices; // 自动播放遇到选项会停，这里也禁掉
        if (btnSkip) btnSkip.interactable = !hasChoices; // 跳过也应禁掉
    }

    void Awake()
    {
        Instance = this;
        if (clickButton == null)
        clickButton = transform.Find("window/clickButton")?.GetComponent<UnityEngine.UI.Button>();

    }

    void OnEnable()
{
    if (clickButton != null)
    {
        clickButton.onClick.RemoveListener(UI_OnClickNext); // 防重复
        clickButton.onClick.AddListener(UI_OnClickNext);
    }
}

void OnDisable()
{
    if (clickButton != null)
        clickButton.onClick.RemoveListener(UI_OnClickNext);
}

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        Time.timeScale = 1f;

        
    }

    void Update()
    {
        

        if (!isStoryActive) return;

        

        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //     return;

        //     // 只有在“没有选项”时，左键推进一行；同时停止自动/跳过
        //     if (story != null && story.currentChoices.Count == 0)
        //     {
        //         StopAuto();
        //         StopSkip();
        //         RefreshButtonsVisual();
        //         DisplayNextLine();
        //     }
        // }
    }

    public void UI_OnClickNext()
{
    if (!isStoryActive || story == null) return;

    // 有选项时不推进（避免误触）
    if (story.currentChoices != null && story.currentChoices.Count > 0) return;

    // 强制推进或结束
    if (story.canContinue) DisplayNextLine();
    else EndStory();

    Debug.Log("Click next");

    //（可选）任何手动点击都停止自动/跳过
    StopAuto();
    StopSkip();
    RefreshButtonsVisual();
}


    // 供 InteractTrigger 调用，开始播放剧情
    public void StartStory(TextAsset inkJSON)
    {
        Debug.Log("InkManager_Explore开始播放剧情");

        Time.timeScale = 1f;

        story = new Story(inkJSON.text);
        isStoryActive = true;

        // 禁止玩家移动
        playerController.enabled = false;
        dialogueUI.SetActive(true);

        RefreshButtonsVisual();

        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        // 清旧选项
        foreach (Transform child in choicePanel) Destroy(child.gameObject);

        string nextLineToShow = null;

        while (story != null && story.canContinue)
        {
            string line = story.Continue();

            // 处理这一行的 tags（即便这行没有正文）
            var tags = story.currentTags;
            if (InkTagRouter.Instance) InkTagRouter.Instance.ProcessTags(tags);

            // 文本清洗
            line = line.Replace("_", " ").Replace("<u>", "").Replace("</u>", "");

            // 如果这一行有可显示文本，就拿它；否则继续吃下一行
            if (!string.IsNullOrWhiteSpace(line))
            {
                nextLineToShow = line;
                break;
            }
        }

        if (nextLineToShow != null)
        {
            dialogueText.text = nextLineToShow;
            ShowChoices();
        }
        else
        {
            // 没内容了
            EndStory();
        }
    }

    
void ShowChoices()
{
    foreach (Choice choice in story.currentChoices)
    {
        // 1) 解析样式标记
        string raw = choice.text;
        string style = "default";

        var m = ChoiceStyleRe.Match(raw);
        if (m.Success)
        {
            style = m.Groups[1].Value.ToLower();     // e.g. "blue" / "red"
            raw   = ChoiceStyleRe.Replace(raw, "").Trim();  // 去掉 {style=...}
        }

        // 2) 选预制体
        Button prefab = choicePrefab_Default;
        if (style == "blue") prefab = choicePrefab_Blue;
        else if (style == "red") prefab = choicePrefab_Red;

        // 3) 生成按钮
        Button choiceButton = Instantiate(prefab, choicePanel);
        var label = choiceButton.GetComponentInChildren<TMPro.TMP_Text>();
        if (label) label.text = raw;                 // 显示“净化后”的文本

        // 4) 绑定点击
        int capturedIndex = choice.index;            // 闭包捕获
        choiceButton.onClick.AddListener(() =>
        {
            story.ChooseChoiceIndex(capturedIndex);

            // 如果你用了“选择后也处理 tags”，保留这行（可选）
            var tags = story.currentTags;
            if (InkTagRouter.Instance) InkTagRouter.Instance.ProcessTags(tags);

            DisplayNextLine();
        });

        RefreshButtonsVisual();
    }
}

    // void ShowChoices()
    // {
    //     foreach (Choice choice in story.currentChoices)
    //     {
    //         Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel);
    //         choiceButton.GetComponentInChildren<TMP_Text>().text = choice.text;

    //         choiceButton.onClick.AddListener(() =>
    //         {
    //             story.ChooseChoiceIndex(choice.index);
    //             DisplayNextLine();
    //         });
    //     }
    // }

    void EndStory()
    {
        story = null;
        isStoryActive = false;

        dialogueText.text = "";

        RefreshButtonsVisual();

        // 清除选项
        foreach (Transform child in choicePanel) Destroy(child.gameObject);

        // 恢复玩家移动
        playerController.enabled = true;
         dialogueUI.SetActive(false);

         StopAuto();
        StopSkip();
        //OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }
}

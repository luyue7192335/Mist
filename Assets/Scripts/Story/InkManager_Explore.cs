using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

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

    [Header("Auto / Skip")]
    public bool autoMode = false;
    public float autoDelay = 1.6f;

    public bool skipMode = false;
    public float skipInterval = 0.02f;   // 快进间隔（尽量短）
    Coroutine _autoCo, _skipCo;

    // === 跑剧情时调用它来刷新按钮状态（可选）===
    public System.Action<bool,bool> OnAutoSkipChanged;

    // === 绑定 UI 按钮 ===
    public void ToggleAuto()
    {
        autoMode = !autoMode;
        if (autoMode) { skipMode = false; StopSkip(); _autoCo = StartCoroutine(AutoLoop()); }
        else          { StopAuto(); }
        OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }
    public void ToggleSkip()
    {
        skipMode = !skipMode;
        if (skipMode) { autoMode = false; StopAuto(); _skipCo = StartCoroutine(SkipLoop()); }
        else          { StopSkip(); }
        OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }
    public void StopAuto() { if (_autoCo!=null) StopCoroutine(_autoCo); _autoCo=null; autoMode=false; }
    public void StopSkip() { if (_skipCo!=null) StopCoroutine(_skipCo); _skipCo=null; skipMode=false; }

    IEnumerator AutoLoop()
    {
        while (isStoryActive && autoMode && story != null)
        {
            // 若有选项就停
            if (story.currentChoices.Count > 0) { StopAuto(); OnAutoSkipChanged?.Invoke(autoMode, skipMode); yield break; }

            yield return new WaitForSeconds(autoDelay);

            // 再判断一次，避免等待期间出现选项
            if (!autoMode) yield break;
            if (story != null && story.currentChoices.Count == 0) DisplayNextLine();
            else { StopAuto(); OnAutoSkipChanged?.Invoke(autoMode, skipMode); yield break; }
        }
        StopAuto(); OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }

    IEnumerator SkipLoop()
    {
        while (isStoryActive && skipMode && story != null)
        {
            if (story.currentChoices.Count > 0) { StopSkip(); OnAutoSkipChanged?.Invoke(autoMode, skipMode); yield break; }

            DisplayNextLine();
            yield return new WaitForSeconds(skipInterval);
        }
        StopSkip(); OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // if (!isStoryActive) return;

        // // 鼠标点击推进对话（只有没有选项时才生效）
        // if (Input.GetMouseButtonDown(0) && story != null && story.currentChoices.Count == 0)
        // {
        //     DisplayNextLine();
        // }

        if (!isStoryActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (story != null && story.currentChoices.Count == 0)
            {
                StopAuto(); StopSkip();
                OnAutoSkipChanged?.Invoke(autoMode, skipMode);
                DisplayNextLine();
            }
        }
    }

    // 供 InteractTrigger 调用，开始播放剧情
    public void StartStory(TextAsset inkJSON)
    {
        Debug.Log("InkManager_Explore开始播放剧情");

        story = new Story(inkJSON.text);
        isStoryActive = true;

        // 禁止玩家移动
        playerController.enabled = false;
        dialogueUI.SetActive(true);

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

        // 清除选项
        foreach (Transform child in choicePanel) Destroy(child.gameObject);

        // 恢复玩家移动
        playerController.enabled = true;
         dialogueUI.SetActive(false);

         StopAuto();
        StopSkip();
        OnAutoSkipChanged?.Invoke(autoMode, skipMode);
    }
}

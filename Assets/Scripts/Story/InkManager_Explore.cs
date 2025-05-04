using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

public class InkManager_Explore : MonoBehaviour
{
     public static InkManager_Explore Instance;

    public TMP_Text dialogueText; // 对话文本
    public Button choiceButtonPrefab; // 选项按钮预制体
    public Transform choicePanel; // 选项容器
    public GameObject dialogueUI; 

    private Story story; // Ink 剧情数据
    private PlayerController playerController; // 玩家移动脚本

    private bool isStoryActive = false; // 判断是否正在进行剧情

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
        if (!isStoryActive) return;

        // 鼠标点击推进对话（只有没有选项时才生效）
        if (Input.GetMouseButtonDown(0) && story != null && story.currentChoices.Count == 0)
        {
            DisplayNextLine();
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
        // 清除旧选项
        foreach (Transform child in choicePanel) Destroy(child.gameObject);

        if (story.canContinue)
        {
            string nextLine = story.Continue();

            // 解决 TMP 下划线问题
            nextLine = nextLine.Replace("_", " ");
            nextLine = nextLine.Replace("<u>", "").Replace("</u>", "");

            dialogueText.text = nextLine;

            ShowChoices();
        }
        else
        {
            // 剧情结束
            EndStory();
        }
    }

    void ShowChoices()
    {
        foreach (Choice choice in story.currentChoices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choice.text;

            choiceButton.onClick.AddListener(() =>
            {
                story.ChooseChoiceIndex(choice.index);
                DisplayNextLine();
            });
        }
    }

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
    }
}

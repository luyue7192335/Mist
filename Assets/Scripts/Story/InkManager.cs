using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public class InkManager : MonoBehaviour
{
    public TMP_Text dialogueText; // 对话文本
    public Button choiceButtonPrefab; // 选项按钮预制体
    public Transform choicePanel; // 选项容器

    public Story story; // Ink 剧情数据

    [SerializeField] private string nextSceneName = "Scene2"; // 通过Inspector配置目标场景
    private bool isDialogueEnded = false;

    void Start()
    {
        

        TextAsset inkJson = Resources.Load<TextAsset>("test"); // 不带 .json
        story = new Story(inkJson.text);
        DisplayNextLine(); // 显示第一条对话
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && story.currentChoices.Count == 0) // 只有没有选项时才继续
        {
            DisplayNextLine();
        }
    }

    void DisplayNextLine()
    {
        // 清除旧选项
        foreach (Transform child in choicePanel) Destroy(child.gameObject);

        if (story.canContinue)
        {
            string nextLine = story.Continue();

            // 🔥 解决 TMP 下划线问题
            nextLine = nextLine.Replace("_", " ");  
            nextLine = nextLine.Replace("<u>", "").Replace("</u>", "");  

            dialogueText.text = nextLine; // ✅ 只赋值一次
            ShowChoices();
        }
        else
        {
            // 新增结束检测逻辑
            if (story.currentChoices.Count == 0 && !isDialogueEnded)
            {
                isDialogueEnded = true;
                StartCoroutine(LoadSceneAfterDelay(1f)); // 延迟1秒跳转
            }
        }
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        
        
        SceneManager.LoadScene(nextSceneName);
    }


    void ShowChoices()
    {
        // 如果有选项，生成按钮
        foreach (Choice choice in story.currentChoices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choice.text;
            choiceButton.onClick.AddListener(() => ChooseOption(choice.index));
        }
    }

    void ChooseOption(int index)
    {
        story.ChooseChoiceIndex(index); // 选择 Ink 选项
        DisplayNextLine(); // 继续显示下一行文本
    }
}

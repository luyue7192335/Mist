using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class startSceneController : MonoBehaviour
{
    public GameObject startPanel;   // 开始界面
    public GameObject chapterPanel; // 章节选择界面
    public string chapter1SceneName = "StoryText"; // 章节1的场景名称

    void Start()
    {
       

        // 确保游戏启动时只显示 startPanel
        startPanel.SetActive(true);
        chapterPanel.SetActive(false);
    }

    // 开始游戏（显示章节界面）
    public void OnStartButtonClicked()
    {
        startPanel.SetActive(false);
        chapterPanel.SetActive(true);
    }

    // 选择章节1并加载对应场景
    public void OnChapter1ButtonClicked()
    {
        SceneManager.LoadScene(chapter1SceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}

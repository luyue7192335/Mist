using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class startSceneController : MonoBehaviour
{
[Header("点击开始后要加载的场景名")]
    // 建议默认是 "Persistent"；如果你想先进序章文本，就改成 "StoryText"
    public string firstSceneName = "Persistent";

    [Header("（可选）把按钮直接拖到这里自动绑定")]
    public Button startButton;
    public Button quitButton;

    void Awake()
    {
        // 确保回到正常时间流
        Time.timeScale = 1f;
        // 主菜单通常显示鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 自动绑定按钮
        if (startButton) startButton.onClick.AddListener(StartGame);
        if (quitButton)  quitButton .onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(firstSceneName))
        {
            Debug.LogError("[StartMenu] firstSceneName 为空！");
            return;
        }
        SceneManager.LoadScene(firstSceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    void Update()
    {
        // 回车/空格 = 开始；ESC = 退出
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            StartGame();
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
        public TextAsset inkJSON; // 绑定剧情文件
    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("按下了E，准备触发Ink剧情");  // ← 这里加一个debug，确认按键按了

            if (inkJSON != null)
            {
                InkManager_Explore.Instance.StartStory(inkJSON);
                Debug.Log("已调用InkManager_Explore播放剧情");  // ← 确认调用成功
            }
            else
            {
                Debug.Log("没有绑定Ink JSON文件！");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("玩家进入互动区域");  // ← 进入触发区
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("玩家离开互动区域");  // ← 离开触发区
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoStartInkOnLoad : MonoBehaviour
{
    public TextAsset inkJSON;             // 拖 Prologue 的 JSON
    
    bool done;
    // void Start() {
    //     var mgr = FindObjectOfType<InkManager_Explore>(includeInactive:true);
    //     if (!mgr || !inkJSON) { Debug.LogWarning("找不到 InkManager 或没绑JSON"); return; }

    //     // 可选：跳到特定 knot
    //     mgr.StartStory(inkJSON);
    // //     if (!string.IsNullOrEmpty(startKnot))
    // //         mgr.JumpToKnot(startKnot); // 在 InkManager_Explore 里加一个封装：_story.ChoosePathString(...)
    //  }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (done || inkJSON == null) return;
        StartCoroutine(Boot());
    }

    IEnumerator Boot(){
        // 等一帧，保证地图/对话 UI 都创建好
        yield return null;
        InkManager_Explore.Instance?.StartStory(inkJSON);
        done = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartInkOnLoad : MonoBehaviour
{
public TextAsset inkJSON;             // 拖 Prologue 的 JSON
    void Start() {
        var mgr = FindObjectOfType<InkManager_Explore>(includeInactive:true);
        if (!mgr || !inkJSON) { Debug.LogWarning("找不到 InkManager 或没绑JSON"); return; }

        // 可选：跳到特定 knot
        mgr.StartStory(inkJSON);
    //     if (!string.IsNullOrEmpty(startKnot))
    //         mgr.JumpToKnot(startKnot); // 在 InkManager_Explore 里加一个封装：_story.ChoosePathString(...)
     }
}

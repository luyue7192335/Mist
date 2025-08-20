using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
[SerializeField] string firstMapScenePath = "Scenes/Map_RoomA"; // 改成你的房间场景路径

    IEnumerator Start()
    {
        yield return null; // 等 MapService Awake 完成
        if (!string.IsNullOrEmpty(firstMapScenePath))
            yield return MapService.Instance.LoadMapAdditive(firstMapScenePath);
    }
}

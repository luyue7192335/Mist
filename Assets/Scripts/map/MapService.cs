using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;

public class MapService : MonoBehaviour
{
public static MapService Instance { get; private set; }

    private Scene _currentScene;          // 当前已加载的地图场景
    private Transform _currentMapRoot;    // 地图根（名字含 "Map_"）

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadMapAdditive(string sceneId, string spawnPoint = "Spawn_Default")
    {
        Debug.Log($"[MapService] Request load: {sceneId}");

        // 1) 卸载旧地图
        if (_currentScene.IsValid())
        {
            Debug.Log($"[MapService] Unload: {_currentScene.name}");
            yield return SceneManager.UnloadSceneAsync(_currentScene);
            _currentScene = default;
            _currentMapRoot = null;
        }

        // 2) 记录加载前场景数量
        int before = SceneManager.sceneCount;

        // 3) 加载新地图（sceneId 可以是 Build Settings 里的“场景名”或“Assets/.../xxx.unity”）
        var op = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        if (op == null)
        {
            Debug.LogError($"[MapService] LoadSceneAsync 返回空：{sceneId}。请确认该场景已加入 Build Settings。");
            yield break;
        }
        yield return op;

        // 4) 取“最后一个已加载的场景”——就是刚刚加进来的那个
        int after = SceneManager.sceneCount;
        if (after <= before)
        {
            Debug.LogError($"[MapService] 场景数量未增加。加载失败？传入：{sceneId}");
            yield break;
        }
        _currentScene = SceneManager.GetSceneAt(after - 1);
        Debug.Log($"[MapService] Loaded scene: index={after - 1}, name={_currentScene.name}, path={_currentScene.path}");

        if (!_currentScene.IsValid() || !_currentScene.isLoaded)
        {
            Debug.LogError("[MapService] 新场景无效或未加载完成。");
            yield break;
        }

        // 5) 设为 Active（这一步之前报错的地方）
        bool ok = SceneManager.SetActiveScene(_currentScene);
        if (!ok)
        {
            Debug.LogError("[MapService] SetActiveScene 失败。");
            yield break;
        }

        // 6) 找地图根（名字包含 "Map_"）
        foreach (var go in _currentScene.GetRootGameObjects())
            if (go.name.Contains("Map_")) { _currentMapRoot = go.transform; break; }
        if (_currentMapRoot == null)
            Debug.LogWarning("[MapService] 当前场景未找到名含 'Map_' 的根对象。请确认你的地图根命名。");

        // 7) 放置玩家到出生点
        PlacePlayerAtSpawn(spawnPoint);

        // // 8) 绑定相机边界（CameraBounds 上的 PolygonCollider2D）
        // var vcam = FindFirstObjectByType<CinemachineVirtualCamera>();
        // var conf = vcam ? vcam.GetComponent<CinemachineConfiner2D>() : null;
        // var boundsGO = GameObject.Find("CameraBounds");
        // if (conf && boundsGO)
        // {
        //     conf.m_BoundingShape2D = boundsGO.GetComponent<PolygonCollider2D>();
        //     conf.InvalidateCache();
        //     Debug.Log("[MapService] Confiner2D 已绑定 CameraBounds。");
        // }
        // else
        // {
        //     Debug.LogWarning("[MapService] 未绑定 Confiner2D（vcam/conf/bounds 缺一）。");
        // }
    }

    public void PlacePlayerAtSpawn(string spawnName)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player || _currentMapRoot == null) return;

        var spawn = _currentMapRoot.Find($"Characters_Spawn/{spawnName}")
                 ?? _currentMapRoot.Find("Characters_Spawn/Spawn_Default");

        if (spawn) player.transform.position = spawn.position;
        else Debug.LogWarning($"[MapService] 没找到出生点：{spawnName}");
    }
}

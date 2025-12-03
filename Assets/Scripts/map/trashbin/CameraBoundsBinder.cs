using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraBoundsBinder : MonoBehaviour
{
    [SerializeField] string boundsTag = "CamBounds";

    CinemachineVirtualCamera _vcam;
    CinemachineConfiner2D _conf;

    void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _conf = _vcam.GetComponent<CinemachineConfiner2D>();
        if (_conf == null) _conf = _vcam.gameObject.AddComponent<CinemachineConfiner2D>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshBinding(); // 进场先绑一次
    }
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    void OnSceneLoaded(Scene s, LoadSceneMode m) => StartCoroutine(BindAfterFrames(2)); // 给加载一点点时间

    [ContextMenu("Refresh Cam Bounds")]
    public void RefreshBinding() => StartCoroutine(BindAfterFrames(0));

    IEnumerator BindAfterFrames(int waitFrames)
    {
        for (int i = 0; i < waitFrames; i++) yield return null;

        var go = FindByTagIncludingInactive(boundsTag);
        if (!go)
        {
            Debug.LogWarning($"[CamBinder] 没找到 Tag={boundsTag} 的物体（会再次尝试）。");
            // 再等一帧再试一次，避免加载时机问题
            yield return null;
            go = FindByTagIncludingInactive(boundsTag);
            if (!go)
            {
                Debug.LogWarning($"[CamBinder] 仍未找到 Tag={boundsTag} 的物体。");
                yield break;
            }
        }

        // 优先 PolygonCollider2D，其次 CompositeCollider2D
        var poly = go.GetComponent<PolygonCollider2D>();
        var comp = go.GetComponent<CompositeCollider2D>();

        if (poly)
        {
            // 等待顶点准备好（有些情况下 Additive 场景一开始 pathCount=0）
            int guard = 0;
            while (poly.pathCount == 0 && guard++ < 10) yield return null;

            _conf.m_BoundingShape2D = poly;
            InvalidateConfinerCacheSafe(_conf);
            Debug.Log($"[CamBinder] 绑定 Polygon: {go.name} (scene: {go.scene.name})");
        }
        else if (comp)
        {
            _conf.m_BoundingShape2D = comp;
            InvalidateConfinerCacheSafe(_conf);
            Debug.Log($"[CamBinder] 绑定 Composite: {go.name} (scene: {go.scene.name})");
        }
        else
        {
            Debug.LogWarning($"[CamBinder] 物体 {go.name} 上没有 Polygon/Composite Collider2D。");
        }
    }

    static GameObject FindByTagIncludingInactive(string tag)
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.isLoaded) continue;

            foreach (var root in s.GetRootGameObjects())
            {
                var found = FindInChildrenRecursive(root.transform, tag);
                if (found) return found;
            }
        }
        return null;
    }
    static GameObject FindInChildrenRecursive(Transform t, string tag)
    {
        if (t.CompareTag(tag)) return t.gameObject;
        for (int i = 0; i < t.childCount; i++)
        {
            var r = FindInChildrenRecursive(t.GetChild(i), tag);
            if (r) return r;
        }
        return null;
    }

    static void InvalidateConfinerCacheSafe(CinemachineConfiner2D conf)
    {
        var m = conf.GetType().GetMethod("InvalidateCache",
                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? conf.GetType().GetMethod("InvalidatePathCache",
                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m != null) m.Invoke(conf, null);
    }
}

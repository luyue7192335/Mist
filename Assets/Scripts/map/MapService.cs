using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapService : MonoBehaviour
{
public static MapService Instance { get; private set; }
    string currentScenePath;
    Transform currentMapRoot;

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadMapAdditive(string scenePath, string spawnPoint = "Spawn_Default")
    {
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            yield return SceneManager.UnloadSceneAsync(currentScenePath);
            currentScenePath = null;
            currentMapRoot = null;
        }

        var op = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        yield return op;

        currentScenePath = scenePath;

        var loaded = SceneManager.GetSceneByPath(scenePath);
        SceneManager.SetActiveScene(loaded);

        foreach (var go in loaded.GetRootGameObjects())
            if (go.name.Contains("Map_")) { currentMapRoot = go.transform; break; }

        PlacePlayerAtSpawn(spawnPoint);
    }

    public void PlacePlayerAtSpawn(string spawnName)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || currentMapRoot == null) return;

        var spawn = currentMapRoot.Find($"Characters_Spawn/{spawnName}")
                   ?? currentMapRoot.Find("Characters_Spawn/Spawn_Default");

        if (spawn) player.transform.position = spawn.position;
    }
}

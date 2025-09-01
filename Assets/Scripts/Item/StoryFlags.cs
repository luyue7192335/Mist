using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlags : MonoBehaviour
{
    // Start is called before the first frame update
    public static StoryFlags Instance;
    HashSet<string> flags = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Set(string key, bool value = true)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (value) flags.Add(key); else flags.Remove(key);
    }

    public bool IsOn(string key) => !string.IsNullOrEmpty(key) && flags.Contains(key);

}

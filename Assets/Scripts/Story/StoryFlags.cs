using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlags : MonoBehaviour
{
    // Start is called before the first frame update
    public static StoryFlags Instance;
    HashSet<string> on = new();

   void Awake(){ if (Instance && Instance!=this){Destroy(gameObject);return;} Instance=this; DontDestroyOnLoad(gameObject); }


    public void Set(string key, bool value){ if (string.IsNullOrEmpty(key)) return; if (value) on.Add(key); else on.Remove(key); }
    public bool IsOn(string key)=> !string.IsNullOrEmpty(key) && on.Contains(key);

    [System.Serializable] public class Save { public List<string> keys = new(); }
    public Save Export(){ return new Save{ keys = new List<string>(on) }; }
    public void Import(Save s){ on = new HashSet<string>(s?.keys ?? new List<string>()); }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;
    public int trust, favor, sanity=100, hp=100, evil;

    void Awake(){ if (Instance && Instance!=this){Destroy(gameObject);return;} Instance=this; DontDestroyOnLoad(gameObject); }
}
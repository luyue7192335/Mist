using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float moveSpeed = 2f;
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        Act();
    }

    // 留给子类实现的行为
    public abstract void Act();

}

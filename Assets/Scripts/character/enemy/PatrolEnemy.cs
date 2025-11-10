using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : EnemyBase
{
    public float chaseRange = 5f;

    public override void Act()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= chaseRange)
        {
            // 向玩家移动
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
}

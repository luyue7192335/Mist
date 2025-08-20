using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewBridge : MonoBehaviour
{
    public Transform visualPivot;            // 指向子物体 VisualPivot
    public Animator visualAnimator;          // 绑定占位用 Animator
    public SpriteRenderer placeholderSR;     // 绑定占位的 SpriteRenderer

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 动画速度驱动
        float speed = Mathf.Abs(rb.velocity.x);
        if (visualAnimator) visualAnimator.SetFloat("Speed", speed);

        // 左右翻转（占位阶段用 localScale；将来 Spine 也可复用这招）
        if (speed > 0.01f)
        {
            float dir = Mathf.Sign(rb.velocity.x);
            var s = visualPivot.localScale;
            s.x = Mathf.Abs(s.x) * (dir >= 0 ? 1f : -1f);
            visualPivot.localScale = s;
        }
    }
}

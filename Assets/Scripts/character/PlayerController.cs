using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] float runMultiplier = 1.6f;

    // Optional: if you later put visuals under a child (e.g., VisualPivot)
    [Header("Optional")]
    [SerializeField] Transform visualPivot;   // leave null if you want to use SpriteRenderer.flipX
    [SerializeField] Animator animator;       // idle/walk blend by speed (optional)

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Physics tip: freeze Z rotation in Rigidbody2D to avoid tipping over.
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip facing
        if (visualPivot) {
            if (Mathf.Abs(moveInput) > 0.01f) {
                var s = visualPivot.localScale;
                s.x = Mathf.Sign(moveInput) >= 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
                visualPivot.localScale = s;
            }
        } else {
            if (moveInput > 0) spriteRenderer.flipX = false;
            else if (moveInput < 0) spriteRenderer.flipX = true;
        }

        // Drive optional animator by horizontal speed
        if (animator) animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void FixedUpdate()
    {
        float speed = Input.GetKey(runKey) ? moveSpeed * runMultiplier : moveSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }
}
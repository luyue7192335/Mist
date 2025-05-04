using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
     public Transform target; // 角色（跟随目标）

    public float smoothSpeed = 0.125f; // 平滑程度
    public Vector3 offset; // 摄像机和角色之间的偏移（一般是 0, 0, -10）

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(
        Mathf.Round(smoothedPosition.x * 100) / 100,
        Mathf.Round(smoothedPosition.y * 100) / 100,
        smoothedPosition.z
        );

    }
}

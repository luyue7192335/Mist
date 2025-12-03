using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBounds : MonoBehaviour
{
    [Tooltip("相机允许活动的世界坐标 X 范围")]
    public float minX = -10f;
    public float maxX =  10f;

    [Tooltip("可选：相机目标固定的 Y（留空用玩家的 Y）")]
    public bool lockY = true;
    public float fixedY = 0f;

    void OnEnable()
    {
        // 地图激活时，把范围推给 CamTargetClamp
        var clamp = FindObjectOfType<CamTargetClamp>(true);
        if (clamp) clamp.SetRange(this);
    }

    void OnDisable()
    {
        // 地图卸载时，如果还指着我，就清空
        var clamp = FindObjectOfType<CamTargetClamp>(true);
        if (clamp && clamp.CurrentRange == this) clamp.SetRange(null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var y = lockY ? fixedY : 0f;
        Gizmos.DrawLine(new Vector3(minX, y - 100, 0), new Vector3(minX, y + 100, 0));
        Gizmos.DrawLine(new Vector3(maxX, y - 100, 0), new Vector3(maxX, y + 100, 0));
    }
}

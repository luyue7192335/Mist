using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamTargetClamp : MonoBehaviour
{
[Header("必填")]
    public Transform player;                  // 玩家
    public CinemachineVirtualCamera vcam;     // 你的虚拟相机

    MapBounds _range;
    public MapBounds CurrentRange => _range;


    public void SetRange(MapBounds r) => _range = r;

    void LateUpdate()
    {
        if (!player || !vcam || _range == null) return;

        // 计算半屏宽：orthoSize * 屏幕宽高比
        float halfWidth = vcam.m_Lens.OrthographicSize * ((float)Screen.width / Screen.height);

        // 防止范围比半屏还小
        float min = Mathf.Min(_range.minX, _range.maxX);
        float max = Mathf.Max(_range.minX, _range.maxX);
        if (max - min < halfWidth * 2f) { float mid=(min+max)*0.5f; min=mid-halfWidth; max=mid+halfWidth; }

        float x = Mathf.Clamp(player.position.x, min + halfWidth, max - halfWidth);
        float y = _range.lockY ? _range.fixedY : player.position.y;

        var p = transform.position;
        transform.position = new Vector3(x, y, p.z);  // z 保持不变
    }
}

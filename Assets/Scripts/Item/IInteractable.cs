using UnityEngine;

public interface IInteractable
{
    // Start is called before the first frame update
     // 是否满足互动（距离/条件等），用于E键和点击二次确认
    bool CanInteract(GameObject player);

    // 执行互动
    void Interact(GameObject player);

    // 进入/离开交互范围时由控制器调用，用来开关描边/边框等
    void SetHighlighted(bool on);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // Start is called before the first frame update
//    [SerializeField] KeyCode interactKey = KeyCode.E;

//     readonly List<IInteractable> _candidates = new();
//     IInteractable _current;

//     void Update()
//     {
//         if (_current == null && _candidates.Count > 0)
//             _current = _candidates[_candidates.Count - 1];

//          if (Input.GetKeyDown(interactKey))
//         Debug.Log($"[Interact] E pressed. current={_current?.GetType().Name ?? "null"}");

//         if (_current != null)
//         {
//             bool can = _current.CanInteract(gameObject);
//             if (can && Input.GetKeyDown(interactKey))
//                 _current.Interact(gameObject);
//             else if (!can && Input.GetKeyDown(interactKey))
//                 Debug.Log("[Interact] E pressed but CanInteract=false");
//         }
//     }

//     void OnTriggerEnter2D(Collider2D other)
//     {
//         var it = other.GetComponent<IInteractable>() ?? other.GetComponentInParent<IInteractable>();

//         // 进入候选
//         Debug.Log($"[Interact] Enter trigger: {other.name}");


//         if (it != null)
//         {
//             if (!_candidates.Contains(it)) _candidates.Add(it);
//             it.SetHighlighted(true);
//             _current = it;
//         }
//     }

//     void OnTriggerExit2D(Collider2D other)
//     {
//         var it = other.GetComponent<IInteractable>() ?? other.GetComponentInParent<IInteractable>();
//         if (it != null)
//         {
//             it.SetHighlighted(false);
//             _candidates.Remove(it);
//             if (_current == it) _current = null;
//         }
//     }

[SerializeField] KeyCode interactKey = KeyCode.E;
    GameObject current;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log($"[InteractEZ] E pressed. current={(current?current.name:"null")}");
            if (current)
                current.SendMessage("OnInteract", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        current = other.gameObject; // 撞到谁就选谁
        Debug.Log($"[InteractEZ] Enter: {other.name}");
        other.gameObject.SendMessage("OnHighlight", true, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 出去时把高亮关掉；如果是当前目标，顺便清空
        other.gameObject.SendMessage("OnHighlight", false, SendMessageOptions.DontRequireReceiver);
        if (other.gameObject == current)
        {
            current = null;
            Debug.Log("[InteractEZ] Exit & clear current");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 防抖：如果某些情况下没触发 Enter，也能在 Stay 里重新拿到
        if (!current) { current = other.gameObject; }
    }
}

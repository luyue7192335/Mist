using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastSpy : MonoBehaviour
{
    readonly List<RaycastResult> hits = new();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var es = EventSystem.current;
            if (es == null) { Debug.Log("No EventSystem"); return; }

            var ped = new PointerEventData(es) { position = Input.mousePosition };
            hits.Clear();
            es.RaycastAll(ped, hits);

            Debug.Log("---- UI hits (top to bottom) ----");
            for (int i = 0; i < hits.Count; i++)
                Debug.Log($"{i}. {hits[i].gameObject.name}  (module={hits[i].module?.GetType().Name})");

            Debug.Log($"currentSelected = {es.currentSelectedGameObject?.name ?? "null"}");
            Debug.Log($"Time.timeScale = {Time.timeScale}");
        }
    }
}

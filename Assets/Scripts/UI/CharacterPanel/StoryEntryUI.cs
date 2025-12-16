using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryEntryUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text titleText;
    public TMP_Text bodyText;          // 解锁时显示正文
    public GameObject lockedGroup;     // 未解锁时显示的整组（里头放锁图/提示字）
    public TMP_Text lockedHintText;    // 未解锁提示（可选）
    public LayoutElement layoutElement; // 可选：限制最小高度

    /// <summary>
    /// 统一签名：标题 / 正文 / 是否解锁 / 未解锁提示
    /// </summary>
    public void Bind(string title, string body, bool unlocked, string lockedHint)
    {
        if (titleText)     titleText.text = title ?? "";
        bool showBody = unlocked && !string.IsNullOrWhiteSpace(body);

        if (bodyText)
        {
            bodyText.gameObject.SetActive(showBody);
            bodyText.text = showBody ? body : "";
        }

        if (lockedGroup)
            lockedGroup.SetActive(!showBody);

        if (!showBody && lockedHintText)
            lockedHintText.text = string.IsNullOrEmpty(lockedHint) ? "未解锁" : lockedHint;

        // 强制重建，保证 ContentSizeFitter 正确撑高
        var rt = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}


using UnityEngine.UI;
using TMPro;

using UnityEngine;

public class CharacterPanelController : MonoBehaviour
{
    [Header("Header")]
    public Image portrait;
    public TMP_Text nameText;
    public TMP_Text introText;

    [Header("Story Scroll")]
    public Transform content;               // ScrollView/Content（带 VLG+CSF）
    public StoryEntryUI storyEntryPrefab; // 你的故事条目预制体脚本

    public void ShowCharacter(CharacterData data)
    {
        if (!data) return;

        if (portrait)  portrait.sprite = data.portrait;
        if (nameText)  nameText.text   = data.displayName;
        if (introText) introText.text  = data.intro;

        foreach (Transform t in content) Destroy(t.gameObject);

        foreach (var s in data.stories)
        {
            bool unlocked = true;
            foreach (var c in s.conditions)
                unlocked &= c.IsMet();

            var entry = Instantiate(storyEntryPrefab, content);
            // 你在 StoryEntryView 里实现 Bind(title, body, unlocked)
            entry.Bind(s.entryId, s.content, unlocked, s.lockedHint);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }
}


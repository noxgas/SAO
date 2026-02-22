using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single quest item in quest log.
/// </summary>
public class VRQuestItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button questButton;

    public void Initialize(string questName, string progress)
    {
        questNameText.text = questName;
        progressText.text = progress;

        if (questButton == null)
            questButton = GetComponent<Button>();

        questButton.onClick.AddListener(OnQuestSelected);
    }

    private void OnQuestSelected()
    {
        Debug.Log($"Selected quest: {questNameText.text}");
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays current combo status in VR HUD.
/// Shows combo count, precision, and timer.
/// </summary>
public class ComboUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboCountText;
    [SerializeField] private TextMeshProUGUI comboNameText;
    [SerializeField] private Image comboProgressBar;
    [SerializeField] private Image precisionIndicator;

    private ComboSystem comboSystem;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        comboSystem = player.GetComponent<ComboSystem>();
    }

    private void Update()
    {
        if (comboSystem == null || !comboSystem.IsComboActive())
        {
            comboCountText.text = "";
            return;
        }

        int currentHits = comboSystem.GetCurrentComboCount();
        float progress = comboSystem.GetComboProgress();

        comboCountText.text = $"COMBO: {currentHits}/5";
        comboProgressBar.fillAmount = progress;

        // Update precision indicator
        Color precisionColor = progress > 0.7f ? Color.green : Color.yellow;
        precisionIndicator.color = precisionColor;
    }
}
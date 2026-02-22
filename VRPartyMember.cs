using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single party member display.
/// </summary>
public class VRPartyMember : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI memberNameText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI statusEffectsText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Image roleIcon;

    public void Initialize(string name, float hpPercent, string role)
    {
        memberNameText.text = name;
        hpBar.fillAmount = hpPercent;
        distanceText.text = "5.2m";
        // Load role icon
    }
}
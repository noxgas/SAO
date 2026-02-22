using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single equipment slot UI.
/// </summary>
public class VREquipmentSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI equippedItemText;
    [SerializeField] private Button slotButton;

    private string slotName;

    public void Initialize(string name)
    {
        slotName = name;
        slotNameText.text = slotName;
        equippedItemText.text = "Empty";

        if (slotButton == null)
            slotButton = GetComponent<Button>();

        slotButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        Debug.Log($"Clicked equipment slot: {slotName}");
    }
}
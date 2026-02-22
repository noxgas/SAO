using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single inventory item in grid.
/// </summary>
public class VRInventoryItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Button itemButton;
    [SerializeField] private Image rarityBorder;

    private Equipment.EquipmentItem itemData;
    private VRInventoryPanel parentPanel;

    public void Initialize(Equipment.EquipmentItem item, VRInventoryPanel parent)
    {
        itemData = item;
        parentPanel = parent;

        if (itemButton == null)
            itemButton = GetComponent<Button>();

        itemNameText.text = item.itemName;
        itemButton.onClick.AddListener(OnItemClicked);

        // Set rarity border color
        Color rarityColor = parentPanel.GetRarityColor(item.rarity);
        if (rarityBorder != null)
            rarityBorder.color = rarityColor;
    }

    private void OnItemClicked()
    {
        parentPanel.OnItemSelected(itemData);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Inventory panel showing player's items and equipment.
/// </summary>
public class InventoryPanel : UIPanel
{
    [Header("Equipment Slots")]
    [SerializeField] private Image rightHandSlot;
    [SerializeField] private Image leftHandSlot;
    [SerializeField] private TextMeshProUGUI rightHandLabel;
    [SerializeField] private TextMeshProUGUI leftHandLabel;

    [Header("Inventory Grid")]
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Button inventoryItemPrefab;
    [SerializeField] private int gridColumns = 5;

    [Header("Item Details")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private Player currentPlayer;
    private Equipment playerEquipment;
    private List<Button> inventoryButtons = new List<Button>();

    private void Start()
    {
        titleText.text = "🎒 INVENTORY 🎒";
        titleText.color = UIManager.Instance.GetAccentColor(AccentType.Blue);
    }

    protected override void OnShow()
    {
        currentPlayer = FindObjectOfType<Player>();
        playerEquipment = currentPlayer.GetComponent<Equipment>();
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        // Clear existing buttons
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();

        // Update equipment slots
        if (playerEquipment != null)
        {
            if (playerEquipment.RightHandWeapon != null)
            {
                rightHandLabel.text = playerEquipment.RightHandWeapon.itemName;
            }

            if (playerEquipment.LeftHandWeapon != null)
            {
                leftHandLabel.text = playerEquipment.LeftHandWeapon.itemName;
            }

            // Populate inventory grid
            var inventory = playerEquipment.GetInventory();
            for (int i = 0; i < inventory.Count; i++)
            {
                Button itemButton = Instantiate(inventoryItemPrefab, inventoryGrid);
                itemButton.GetComponentInChildren<TextMeshProUGUI>().text = inventory[i].itemName;
                
                int index = i;
                itemButton.onClick.AddListener(() => OnItemSelected(inventory[index]));
                inventoryButtons.Add(itemButton);
            }
        }
    }

    private void OnItemSelected(Equipment.EquipmentItem item)
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        itemStatsText.text = $"DMG: {item.damage}\nRarity: {item.rarity}\n" +
                            (item.isDualWieldSword ? "✨ DUAL WIELD CAPABLE ✨" : "");
    }
}
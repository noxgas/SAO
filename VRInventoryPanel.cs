using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Inventory panel - displays items with categories.
/// Left: Category tabs
/// Center: Item grid (5xN layout)
/// Right: Item details
/// </summary>
public class VRInventoryPanel : VRMenuPanel
{
    [Header("Category Tabs")]
    [SerializeField] private Transform categoryTabsContainer;
    [SerializeField] private VRMenuButton categoryTabPrefab;

    [Header("Item Grid")]
    [SerializeField] private Transform itemGridContainer;
    [SerializeField] private VRInventoryItem itemPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private int gridColumns = 5;

    [Header("Item Details Panel")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI sellValueText;

    [Header("Action Buttons")]
    [SerializeField] private Transform actionButtonsContainer;
    [SerializeField] private VRMenuButton actionButtonPrefab;

    private Equipment playerEquipment;
    private Equipment.EquipmentItem selectedItem;
    private string currentCategory = "Weapons";

    private string[] categories = new string[]
    {
        "Weapons",
        "Armor",
        "Accessories",
        "Consumables",
        "Materials",
        "Quest Items"
    };

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 1200f;
        panelHeight = 700f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        
        Player player = system.GetCurrentPlayer();
        playerEquipment = player.GetComponent<Equipment>();

        CreateCategoryTabs();
        RefreshInventory();
    }

    private void CreateCategoryTabs()
    {
        foreach (string category in categories)
        {
            VRMenuButton tab = Instantiate(categoryTabPrefab, categoryTabsContainer);
            tab.Initialize(category, null);
            // Add click listener to filter inventory
        }
    }

    private void RefreshInventory()
    {
        // Clear existing items
        foreach (Transform child in itemGridContainer)
        {
            Destroy(child.gameObject);
        }

        // Add items from current category
        var inventory = playerEquipment.GetInventory();
        for (int i = 0; i < inventory.Count; i++)
        {
            VRInventoryItem item = Instantiate(itemPrefab, itemGridContainer);
            item.Initialize(inventory[i], this);
        }
    }

    public void OnItemSelected(Equipment.EquipmentItem item)
    {
        selectedItem = item;

        // Update details panel
        itemNameText.text = item.itemName;
        rarityText.text = $"Rarity: {item.rarity}";
        rarityText.color = GetRarityColor(item.rarity);
        statsText.text = $"Damage: {item.damage}";
        descriptionText.text = item.description;
        sellValueText.text = $"Sell Value: {item.damage * 100} Col";
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => new Color(0.7f, 0.7f, 0.7f),
            ItemRarity.Uncommon => new Color(0, 1, 0.5f),
            ItemRarity.Rare => new Color(0.227f, 0.627f, 1f),
            ItemRarity.Epic => new Color(0.75f, 0, 1f),
            ItemRarity.Legendary => new Color(1, 0.8f, 0),
            _ => Color.white
        };
    }
}
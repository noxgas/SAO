using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Equipment panel - shows 3D character and equipment slots.
/// Left: 3D character model
/// Right: Equipment slots
/// </summary>
public class VREquipmentPanel : VRMenuPanel
{
    [Header("3D Character")]
    [SerializeField] private RawImage characterDisplayImage;
    [SerializeField] private RenderTexture characterRenderTexture;

    [Header("Equipment Slots")]
    [SerializeField] private Transform equipmentSlotsContainer;
    [SerializeField] private VREquipmentSlot equipmentSlotPrefab;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private string[] equipmentSlots = new string[]
    {
        "Main Hand",
        "Off Hand",
        "Head",
        "Chest",
        "Arms",
        "Legs",
        "Boots",
        "Accessory 1",
        "Accessory 2",
        "Accessory 3",
        "Accessory 4"
    };

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 900f;
        panelHeight = 800f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        CreateEquipmentSlots();
    }

    private void CreateEquipmentSlots()
    {
        foreach (string slot in equipmentSlots)
        {
            VREquipmentSlot slotUI = Instantiate(equipmentSlotPrefab, equipmentSlotsContainer);
            slotUI.Initialize(slot);
        }
    }

    private void Start()
    {
        titleText.text = "👕 EQUIPMENT 👕";
        titleText.color = VRMenuSystem.Instance.GetAccentColor(AccentType.Green);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Map panel - shows current floor with markers.
/// </summary>
public class VRMapPanel : VRMenuPanel
{
    [Header("Map Display")]
    [SerializeField] private RawImage mapImage;
    [SerializeField] private Transform mapMarkersContainer;

    [Header("Zoom Controls")]
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private TextMeshProUGUI zoomLevelText;

    [Header("Floor Info")]
    [SerializeField] private TextMeshProUGUI floorNameText;
    [SerializeField] private TextMeshProUGUI floorDescriptionText;

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 800f;
        panelHeight = 800f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        RefreshMap();

        if (zoomSlider != null)
            zoomSlider.onValueChanged.AddListener(OnZoomChanged);
    }

    private void RefreshMap()
    {
        // TODO: Load map for current floor
        floorNameText.text = "Floor 1: The Beginning";
        floorDescriptionText.text = "Starting town and surrounding areas";
    }

    private void OnZoomChanged(float value)
    {
        zoomLevelText.text = $"Zoom: {Mathf.RoundToInt(value * 100)}%";
    }
}
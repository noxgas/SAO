using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space VR health display.
/// Attach to a child GameObject of the XR rig so it floats in VR space
/// near the player's wrist or in their field of view.
/// </summary>
public class VRHealthDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image staminaBarFill;

    [Header("Target")]
    [SerializeField] private PlayerCombat playerCombat;

    [Header("Positioning")]
    [Tooltip("Transform to billboard towards (usually the head camera).")]
    [SerializeField] private Transform cameraTransform;

    [Header("Colours")]
    [SerializeField] private Color healthHighColor   = new Color(0.18f, 0.9f, 0.4f);
    [SerializeField] private Color healthMidColor    = new Color(1f, 0.85f, 0f);
    [SerializeField] private Color healthLowColor    = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color staminaColor      = new Color(0.1f, 0.6f, 1f);

    private void Start()
    {
        if (playerCombat == null)
            playerCombat = FindObjectOfType<PlayerCombat>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (staminaBarFill != null)
            staminaBarFill.color = staminaColor;
    }

    private void Update()
    {
        if (playerCombat == null) return;

        // Update bar fills
        float hp  = playerCombat.HealthPercent;
        float sta = playerCombat.StaminaPercent;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = hp;
            healthBarFill.color = hp > 0.5f ? healthHighColor
                                : hp > 0.25f ? healthMidColor
                                : healthLowColor;
        }

        if (staminaBarFill != null)
            staminaBarFill.fillAmount = sta;

        // Billboard towards camera so the UI is always readable
        if (cameraTransform != null)
        {
            Vector3 lookDir = transform.position - cameraTransform.position;
            if (lookDir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}

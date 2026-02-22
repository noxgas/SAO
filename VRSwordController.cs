using UnityEngine;

public enum SwordType
{
    OneHanded,
    TwoHanded
}

/// <summary>
/// Physics-based VR sword controller.
/// Supports one-handed and two-handed swords.
/// Swing velocity is measured each frame; when it exceeds the threshold the
/// sword collider is treated as active so it can register hits on enemies.
/// A second hand can optionally grip the pommel to switch to two-handed mode.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VRSwordController : MonoBehaviour
{
    [Header("Sword Config")]
    [SerializeField] private SwordType swordType = SwordType.OneHanded;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float swingDamageThreshold = 1.5f;   // minimum m/s for swing to deal damage (must be >= this value)
    [SerializeField] private float attackCooldown = 0.3f;

    [Header("Two-Handed Support")]
    [Tooltip("Assign the pommel Transform so a second hand can grip it.")]
    [SerializeField] private Transform pommelGripPoint;
    [SerializeField] private bool secondHandGripping = false;

    [Header("VFX / Audio (optional)")]
    [SerializeField] private TrailRenderer swingTrail;

    [Header("Desktop Test Mode")]
    [Tooltip("Press the swing key to simulate a sword swing without a VR headset.")]
    [SerializeField] private bool desktopTestMode = false;
    [SerializeField] private KeyCode desktopSwingKey = KeyCode.Mouse0;
    [Tooltip("Simulated swing speed used when the swing key is pressed.")]
    [SerializeField] private float desktopSimulatedSwingSpeed = 3f;

    // Runtime state
    private Rigidbody rb;
    private Vector3 prevPosition;
    private float swingSpeed;
    private float cooldownTimer;
    private bool isHeld;
    private Transform primaryGrip;

    public bool IsHeld => isHeld;
    public float SwingSpeed => swingSpeed;
    public SwordType Type => swordType;

    /// <summary>
    /// Call from your VR grab system when the player picks up the sword.
    /// Pass the hand transform as the primary grip anchor.
    /// </summary>
    public void OnGrab(Transform handTransform)
    {
        isHeld = true;
        primaryGrip = handTransform;
        rb.isKinematic = true;
        transform.SetParent(handTransform, worldPositionStays: true);
        prevPosition = transform.position;
    }

    /// <summary>
    /// Call from your VR grab system when the player releases the sword.
    /// </summary>
    public void OnRelease()
    {
        isHeld = false;
        secondHandGripping = false;
        primaryGrip = null;
        transform.SetParent(null);
        rb.isKinematic = false;
        // Inherit the swing velocity so the sword flies naturally
        rb.linearVelocity = swingSpeed * transform.forward;
    }

    /// <summary>
    /// Call from your VR grab system when a second hand grips the pommel
    /// (two-handed mode only).
    /// </summary>
    public void OnSecondHandGrab(bool gripping)
    {
        if (swordType == SwordType.TwoHanded)
            secondHandGripping = gripping;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        prevPosition = transform.position;

        if (swingTrail != null)
            swingTrail.emitting = false;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // Measure swing speed (VR: position delta; Desktop: key-triggered simulation)
        if (desktopTestMode && Input.GetKey(desktopSwingKey))
            swingSpeed = desktopSimulatedSwingSpeed;
        else
            swingSpeed = (transform.position - prevPosition).magnitude / Time.deltaTime;

        prevPosition = transform.position;

        bool activeSwing = isHeld && swingSpeed >= swingDamageThreshold;

        if (swingTrail != null)
            swingTrail.emitting = activeSwing;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHeld || cooldownTimer > 0f) return;
        if (swingSpeed < swingDamageThreshold) return;

        // Two-handed swords require both hands on the weapon
        if (swordType == SwordType.TwoHanded && !secondHandGripping) return;

        AIEnemy enemy = other.GetComponentInParent<AIEnemy>();
        if (enemy != null)
        {
            float appliedDamage = swordType == SwordType.TwoHanded ? damage * 1.5f : damage;
            enemy.TakeDamage(appliedDamage);
            cooldownTimer = attackCooldown;
        }
    }
}

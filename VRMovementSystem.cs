using UnityEngine;

/// <summary>
/// VR arm-swing movement system compatible with Meta Quest 2 and SteamVR.
/// Direction is derived from the average of both hand positions relative to the body.
/// Movement speed is driven by the *peak* hand swing speed (hardest-swinging hand wins)
/// and shaped through a power curve so light taps barely register while hard swings
/// deliver full speed.  A smoothed acceleration/deceleration prevents instant on/off.
/// The head camera rotates freely; the body follows the averaged hand direction.
/// </summary>
public class VRMovementSystem : MonoBehaviour
{
    [Header("XR Rig References")]
    [SerializeField] private Transform headCamera;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private CharacterController characterController;

    [Header("Movement Tuning")]
    [SerializeField] private float maxMoveSpeed = 6f;
    [SerializeField] private float swingSpeedScale = 2.5f;
    [Tooltip("Minimum hand velocity (m/s) before any movement registers.")]
    [SerializeField] private float swingThreshold = 0.4f;
    [SerializeField] private float bodyRotationSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Arm-Swing Intensity")]
    [Tooltip("Use the fastest hand instead of the average. " +
             "One hard swing counts fully rather than being diluted by the other arm.")]
    [SerializeField] private bool usePeakHandSpeed = true;
    [Tooltip("Power curve exponent applied to the normalised swing speed (0-1). " +
             "1 = linear.  Values > 1 make light swings feel weaker and hard swings " +
             "feel dramatically faster.  Recommended: 1.5 – 2.")]
    [SerializeField] private float swingCurveExponent = 1.8f;
    [Tooltip("How quickly speed builds up toward the target value (higher = snappier ramp-up).")]
    [SerializeField] private float speedAcceleration = 8f;
    [Tooltip("How quickly speed falls off when you stop swinging (higher = quicker stop).")]
    [SerializeField] private float speedDeceleration = 5f;

    [Header("Desktop Test Mode")]
    [Tooltip("Enable keyboard (WASD) + mouse movement when no VR headset is present.")]
    [SerializeField] private bool desktopTestMode = false;
    [SerializeField] private float desktopMoveSpeed = 5f;
    [SerializeField] private float desktopMouseSensitivity = 2f;

    // Internal state
    private Vector3 leftHandPrevPos;
    private Vector3 rightHandPrevPos;
    private float verticalVelocity;
    private float desktopYaw;        // accumulated horizontal mouse look (desktop only)
    private float smoothedMoveSpeed; // current smoothed speed value
    private float cachedMaxRawSpeed; // pre-computed to avoid per-frame division
    private float cachedUsableRange; // pre-computed to avoid per-frame division

    // Read-only accessor so debug HUDs can display the live speed
    public float SmoothedMoveSpeed => smoothedMoveSpeed;

    private void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (headCamera == null)
            headCamera = Camera.main != null ? Camera.main.transform : transform;

        if (leftHandTransform != null)
            leftHandPrevPos = leftHandTransform.position;
        if (rightHandTransform != null)
            rightHandPrevPos = rightHandTransform.position;

        CacheSwingConstants();
    }

    /// <summary>Pre-compute constants derived from serialized fields so Update stays cheap.</summary>
    private void CacheSwingConstants()
    {
        cachedMaxRawSpeed = maxMoveSpeed / Mathf.Max(swingSpeedScale, 0.001f);
        cachedUsableRange  = Mathf.Max(cachedMaxRawSpeed - swingThreshold, 0.001f);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep cached values in sync when a field is tweaked in the Inspector at edit-time
        CacheSwingConstants();
    }
#endif

    private void Update()
    {
        if (characterController == null) return;

        if (desktopTestMode)
        {
            UpdateDesktop();
            return;
        }

        if (leftHandTransform == null && rightHandTransform == null)
        {
            Debug.LogWarning("[VRMovementSystem] No hand transforms assigned and desktopTestMode is off. " +
                             "Enable desktopTestMode or assign hand transforms in the Inspector.");
            return;
        }

        UpdateVR();
    }

    private void UpdateDesktop()
    {
        float deltaTime = Time.deltaTime;

        // Mouse look – horizontal only so the head camera stays independent
        desktopYaw += Input.GetAxis("Mouse X") * desktopMouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, desktopYaw, 0f);

        // WASD movement in the direction the body is facing
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;
        Vector3 horizontalMotion = moveDir * desktopMoveSpeed;

        // Gravity
        if (characterController.isGrounded)
            verticalVelocity = -0.5f;
        else
            verticalVelocity += gravity * deltaTime;

        characterController.Move((horizontalMotion + Vector3.up * verticalVelocity) * deltaTime);
    }

    private void UpdateVR()
    {
        float deltaTime = Time.deltaTime;

        // ── Per-hand velocities ───────────────────────────────────────────────
        Vector3 leftHandVelocity  = Vector3.zero;
        Vector3 rightHandVelocity = Vector3.zero;

        if (leftHandTransform != null)
        {
            leftHandVelocity  = (leftHandTransform.position  - leftHandPrevPos)  / deltaTime;
            leftHandPrevPos   = leftHandTransform.position;
        }

        if (rightHandTransform != null)
        {
            rightHandVelocity = (rightHandTransform.position - rightHandPrevPos) / deltaTime;
            rightHandPrevPos  = rightHandTransform.position;
        }

        // Horizontal speed for each hand (ignore vertical bobbing)
        float leftSpeed  = Mathf.Sqrt(leftHandVelocity.x  * leftHandVelocity.x  + leftHandVelocity.z  * leftHandVelocity.z);
        float rightSpeed = Mathf.Sqrt(rightHandVelocity.x * rightHandVelocity.x + rightHandVelocity.z * rightHandVelocity.z);

        // ── Peak vs. average ─────────────────────────────────────────────────
        // Peak: the harder-swinging hand drives speed on its own.
        // Average: both hands must swing – used if usePeakHandSpeed is off.
        float rawSwingSpeed = usePeakHandSpeed
            ? Mathf.Max(leftSpeed, rightSpeed)
            : (leftSpeed + rightSpeed) * 0.5f;

        // ── Power curve ──────────────────────────────────────────────────────
        // Normalise to [0,1] within the usable range, apply exponent, then re-scale.
        // This makes light taps barely register and hard swings feel powerful.
        float normalised  = Mathf.Clamp01((rawSwingSpeed - swingThreshold) / cachedUsableRange);
        float curved      = Mathf.Pow(normalised, swingCurveExponent);
        float targetSpeed = rawSwingSpeed > swingThreshold
            ? Mathf.Min(curved * swingSpeedScale * cachedMaxRawSpeed, maxMoveSpeed)
            : 0f;

        // ── Smooth acceleration / deceleration ───────────────────────────────
        float smoothRate = targetSpeed > smoothedMoveSpeed ? speedAcceleration : speedDeceleration;
        smoothedMoveSpeed = Mathf.MoveTowards(smoothedMoveSpeed, targetSpeed, smoothRate * deltaTime);

        // ── Movement direction from average hand position ─────────────────────
        Vector3 moveDirection = Vector3.zero;
        if (smoothedMoveSpeed > 0.01f)
        {
            Vector3 avgHandPos = Vector3.zero;
            int handCount = 0;

            if (leftHandTransform  != null) { avgHandPos += leftHandTransform.position;  handCount++; }
            if (rightHandTransform != null) { avgHandPos += rightHandTransform.position; handCount++; }

            if (handCount > 0)
            {
                avgHandPos /= handCount;
                Vector3 toHands = avgHandPos - transform.position;
                toHands.y = 0f;

                if (toHands.sqrMagnitude > 0.0001f)
                    moveDirection = toHands.normalized;
            }
        }

        // ── Apply motion ─────────────────────────────────────────────────────
        Vector3 horizontalMotion = moveDirection * smoothedMoveSpeed;

        if (characterController.isGrounded)
            verticalVelocity = -0.5f;
        else
            verticalVelocity += gravity * deltaTime;

        characterController.Move((horizontalMotion + Vector3.up * verticalVelocity) * deltaTime);

        // ── Body rotation follows average hand direction ──────────────────────
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetBodyRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRotation,
                bodyRotationSpeed * deltaTime);
        }
    }
}

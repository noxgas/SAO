using UnityEngine;

/// <summary>
/// VR arm-swing movement system compatible with Meta Quest 2 and SteamVR.
/// Direction is derived from the average of both hand positions relative to the body.
/// Movement speed scales with arm-swing velocity.
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
    [Tooltip("Minimum hand velocity (m/s) that registers as a swing.")]
    [SerializeField] private float swingThreshold = 0.4f;
    [SerializeField] private float bodyRotationSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Desktop Test Mode")]
    [Tooltip("Enable keyboard (WASD) + mouse movement when no VR headset is present.")]
    [SerializeField] private bool desktopTestMode = false;
    [SerializeField] private float desktopMoveSpeed = 5f;
    [SerializeField] private float desktopMouseSensitivity = 2f;

    // Internal state
    private Vector3 leftHandPrevPos;
    private Vector3 rightHandPrevPos;
    private float verticalVelocity;
    private float desktopYaw;   // accumulated horizontal mouse look (desktop only)

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
    }

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

        Vector3 leftHandVelocity = Vector3.zero;
        Vector3 rightHandVelocity = Vector3.zero;

        if (leftHandTransform != null)
        {
            leftHandVelocity = (leftHandTransform.position - leftHandPrevPos) / deltaTime;
            leftHandPrevPos = leftHandTransform.position;
        }

        if (rightHandTransform != null)
        {
            rightHandVelocity = (rightHandTransform.position - rightHandPrevPos) / deltaTime;
            rightHandPrevPos = rightHandTransform.position;
        }

        // Average swing speed (horizontal only, ignore vertical component for speed)
        float leftSpeed  = Mathf.Sqrt(leftHandVelocity.x  * leftHandVelocity.x  + leftHandVelocity.z  * leftHandVelocity.z);
        float rightSpeed = Mathf.Sqrt(rightHandVelocity.x * rightHandVelocity.x + rightHandVelocity.z * rightHandVelocity.z);
        float avgSwingSpeed = (leftSpeed + rightSpeed) * 0.5f;

        // --- Compute movement direction from average hand position ---
        Vector3 moveDirection = Vector3.zero;
        if (avgSwingSpeed > swingThreshold)
        {
            // Use the average world position of both hands relative to the body
            Vector3 avgHandPos = Vector3.zero;
            int handCount = 0;

            if (leftHandTransform != null)  { avgHandPos += leftHandTransform.position;  handCount++; }
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

        // --- Scale speed by swing intensity, clamped to maxMoveSpeed ---
        float moveSpeed = Mathf.Min(avgSwingSpeed * swingSpeedScale, maxMoveSpeed);
        Vector3 horizontalMotion = moveDirection * moveSpeed;

        // --- Gravity ---
        if (characterController.isGrounded)
            verticalVelocity = -0.5f;
        else
            verticalVelocity += gravity * deltaTime;

        Vector3 totalMotion = horizontalMotion + Vector3.up * verticalVelocity;
        characterController.Move(totalMotion * deltaTime);

        // --- Body rotation follows average hand direction, head stays independent ---
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetBodyRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRotation,
                bodyRotationSpeed * deltaTime);
        }
    }
}

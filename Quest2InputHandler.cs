using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Maps Meta Quest 2 controller inputs to all VR prototype systems.
/// Uses Unity's built-in UnityEngine.XR.InputDevices API (OpenXR-compatible).
/// No Meta/Oculus SDK or SteamVR SDK required — just enable OpenXR in
/// Project Settings ▶ XR Plug-in Management.
///
/// ┌─────────────────────────────────────────────────────────────────────┐
/// │                  Quest 2 Control Mapping                           │
/// ├───────────────────────┬─────────────────────────────────────────────┤
/// │ Right Grip (squeeze)  │ Grab / release sword with right hand        │
/// │ Left  Grip (squeeze)  │ Grab / release sword with left hand         │
/// │                       │   (enables two-handed mode when right also  │
/// │                       │    holds the same sword)                    │
/// │ Right Trigger         │ Execute skill: basic_slash                  │
/// │ Left  Trigger         │ Block (logs to console; hook up your block) │
/// │ A Button (right)      │ Toggle pause / debug overlay                │
/// │ B Button (right)      │ Respawn enemies (prototype testing)         │
/// │ Y Button (left)       │ Open / close holographic menu               │
/// │ X Button (left)       │ (reserved – no action in prototype)         │
/// │ Left  Thumbstick      │ Snap-turn body (optional, 45° increments)   │
/// │ Arm swing (tracking)  │ Locomotion via VRMovementSystem             │
/// └───────────────────────┴─────────────────────────────────────────────┘
/// </summary>
public class Quest2InputHandler : MonoBehaviour
{
    // ── Inspector references (auto-found if left empty) ──────────────────────
    [Header("References (auto-found if left empty)")]
    [SerializeField] private VRSwordController rightHandSword;
    [SerializeField] private VRSwordController leftHandSword;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private VRPrototypeTestMode testModeOverlay;

    [Header("Grip Settings")]
    [Tooltip("Grip axis value (0-1) that counts as 'fully squeezed'.")]
    [SerializeField] private float gripActivationThreshold = 0.7f;

    [Header("Snap Turn")]
    [SerializeField] private bool enableSnapTurn = true;
    [SerializeField] private float snapTurnAngle = 45f;
    [Tooltip("Dead-zone on the thumbstick before a snap turn fires.")]
    [SerializeField] private float snapTurnDeadzone = 0.6f;

    // ── Device handles ────────────────────────────────────────────────────────
    private InputDevice rightController;
    private InputDevice leftController;

    // ── State tracking (to detect edge transitions) ───────────────────────────
    private bool rightGripHeld;
    private bool leftGripHeld;
    private bool rightTriggerHeld;
    private bool leftTriggerHeld;
    private bool aButtonHeld;
    private bool bButtonHeld;
    private bool yButtonHeld;
    private bool snapTurnUsed;   // true while joystick is deflected (debounce)

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        AutoFindReferences();
        TryAcquireDevices();
        // Also subscribe to device-connected events in case headset boots after scene load
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    private void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    private void AutoFindReferences()
    {
        if (playerCombat == null)    playerCombat    = FindObjectOfType<PlayerCombat>();
        if (enemySpawner == null)    enemySpawner    = FindObjectOfType<EnemySpawner>();
        if (testModeOverlay == null) testModeOverlay = FindObjectOfType<VRPrototypeTestMode>();
    }

    private void TryAcquireDevices()
    {
        var rightDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            rightDevices);
        if (rightDevices.Count > 0) rightController = rightDevices[0];

        var leftDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            leftDevices);
        if (leftDevices.Count > 0) leftController = leftDevices[0];
    }

    private void OnDeviceConnected(InputDevice device)
    {
        // Re-acquire when a new controller is paired (e.g., waking from sleep)
        TryAcquireDevices();
    }

    // ── Per-frame polling ─────────────────────────────────────────────────────

    private void Update()
    {
        if (!rightController.isValid || !leftController.isValid)
        {
            TryAcquireDevices();
            return;
        }

        PollGrip();
        PollTriggers();
        PollButtons();
        PollSnapTurn();
    }

    // ── Grip → sword grab / release ───────────────────────────────────────────

    private void PollGrip()
    {
        // Right grip
        rightController.TryGetFeatureValue(CommonUsages.grip, out float rightGripVal);
        bool rightGripDown = rightGripVal >= gripActivationThreshold;

        if (rightGripDown && !rightGripHeld)
        {
            rightGripHeld = true;
            GrabSword(rightHandSword, rightHandTransform, isLeftHand: false);
        }
        else if (!rightGripDown && rightGripHeld)
        {
            rightGripHeld = false;
            ReleaseSword(rightHandSword);
        }

        // Left grip
        leftController.TryGetFeatureValue(CommonUsages.grip, out float leftGripVal);
        bool leftGripDown = leftGripVal >= gripActivationThreshold;

        if (leftGripDown && !leftGripHeld)
        {
            leftGripHeld = true;
            // Cache to avoid null-race between the two checks
            VRSwordController rightSword = rightHandSword;
            if (rightSword != null && rightSword.IsHeld &&
                rightSword.Type == SwordType.TwoHanded)
            {
                // Left hand grips the pommel – engage two-handed mode
                // Release any separately held left-hand sword first to avoid an inconsistent state
                if (leftHandSword != null && leftHandSword.IsHeld && leftHandSword != rightSword)
                    ReleaseSword(leftHandSword);

                rightSword.OnSecondHandGrab(true);
                Debug.Log("[Quest2] Left grip → two-handed mode engaged.");
            }
            else
            {
                GrabSword(leftHandSword, leftHandTransform, isLeftHand: true);
            }
        }
        else if (!leftGripDown && leftGripHeld)
        {
            leftGripHeld = false;
            VRSwordController rightSword = rightHandSword;
            if (rightSword != null && rightSword.IsHeld &&
                rightSword.Type == SwordType.TwoHanded)
            {
                rightSword.OnSecondHandGrab(false);
                Debug.Log("[Quest2] Left grip released → two-handed mode disengaged.");
            }
            else
            {
                ReleaseSword(leftHandSword);
            }
        }
    }

    private void GrabSword(VRSwordController sword, Transform anchor, bool isLeftHand)
    {
        if (sword == null || anchor == null) return;
        if (sword.IsHeld) return;
        sword.OnGrab(anchor);
        Debug.Log($"[Quest2] Grabbed {sword.name} ({sword.Type}) with {(isLeftHand ? "left" : "right")} hand.");
    }

    private void ReleaseSword(VRSwordController sword)
    {
        if (sword == null || !sword.IsHeld) return;
        sword.OnRelease();
        Debug.Log($"[Quest2] Released {sword.name}.");
    }

    // ── Triggers ──────────────────────────────────────────────────────────────

    private void PollTriggers()
    {
        // Right trigger → execute skill (basic slash)
        rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigVal);
        bool rightTrigDown = rightTrigVal > 0.5f;

        if (rightTrigDown && !rightTriggerHeld)
        {
            rightTriggerHeld = true;
            if (playerCombat != null)
                playerCombat.TryExecuteSkill("basic_slash");
        }
        else if (!rightTrigDown)
        {
            rightTriggerHeld = false;
        }

        // Left trigger → block
        leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTrigVal);
        bool leftTrigDown = leftTrigVal > 0.5f;

        if (leftTrigDown && !leftTriggerHeld)
        {
            leftTriggerHeld = true;
            Debug.Log("[Quest2] Block!");
        }
        else if (!leftTrigDown)
        {
            leftTriggerHeld = false;
        }
    }

    // ── Face buttons ──────────────────────────────────────────────────────────

    private void PollButtons()
    {
        // A (right controller) → toggle debug overlay / pause
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aDown);
        if (aDown && !aButtonHeld)
        {
            aButtonHeld = true;
            if (testModeOverlay != null)
                testModeOverlay.ToggleOverlay();
            else
                Debug.Log("[Quest2] A button – overlay toggle (no VRPrototypeTestMode found).");
        }
        else if (!aDown) aButtonHeld = false;

        // B (right controller) → respawn enemies
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bDown);
        if (bDown && !bButtonHeld)
        {
            bButtonHeld = true;
            if (enemySpawner != null)
            {
                enemySpawner.DespawnAll();
                Debug.Log("[Quest2] B button – enemies despawned.");
            }
        }
        else if (!bDown) bButtonHeld = false;

        // Y (left controller) → open / close holographic menu
        leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yDown);
        if (yDown && !yButtonHeld)
        {
            yButtonHeld = true;
            VRMenuSystem menuSys = VRMenuSystem.Instance;
            if (menuSys != null)
                menuSys.ToggleMenu();
            else
                Debug.Log("[Quest2] Y button – menu toggle (no VRMenuSystem found).");
        }
        else if (!yDown) yButtonHeld = false;
    }

    // ── Snap turn ─────────────────────────────────────────────────────────────

    private void PollSnapTurn()
    {
        if (!enableSnapTurn) return;

        leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftStick);

        if (Mathf.Abs(leftStick.x) >= snapTurnDeadzone && !snapTurnUsed)
        {
            snapTurnUsed = true;
            float turnDir = leftStick.x > 0 ? 1f : -1f;
            transform.Rotate(Vector3.up, snapTurnAngle * turnDir, Space.World);
        }
        else if (Mathf.Abs(leftStick.x) < snapTurnDeadzone * 0.5f)
        {
            // Require stick to return near centre before next snap fires
            snapTurnUsed = false;
        }
    }
}

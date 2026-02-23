using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Maps Meta Quest 2 controller inputs to all VR prototype systems.
/// Uses Unity's built-in UnityEngine.XR.InputDevices API (OpenXR-compatible).
/// No Meta/Oculus SDK or SteamVR SDK required — just enable OpenXR in
/// Project Settings ▶ XR Plug-in Management.
///
/// ┌──────────────────────────────────────────────────────────────────────────┐
/// │                    Quest 2 Control Mapping                              │
/// ├──────────────────────────────┬──────────────────────────────────────────┤
/// │ Right Grip (squeeze)         │ Grab / release sword with right hand     │
/// │ Left  Grip (squeeze)         │ Grab / release sword with left hand      │
/// │                              │   (two-handed mode when right also held) │
/// │ Right Trigger                │ Execute skill: basic_slash               │
/// │ Left  Trigger (held)         │ Part of menu gesture (see below)         │
/// │ A Button (right)             │ Toggle debug overlay                     │
/// │ B Button (right)             │ Despawn all enemies (test reset)         │
/// │ Y / X Button (left)          │ (reserved)                               │
/// │ Left  Thumbstick ←/→         │ Snap-turn body (45° increments)          │
/// │ Arm swing (tracking)         │ Locomotion via VRMovementSystem          │
/// └──────────────────────────────┴──────────────────────────────────────────┘
///
/// MENU GESTURE (opens / closes the SAO holographic menu)
/// ───────────────────────────────────────────────────────
/// 1. Hold your LEFT hand extended in FRONT of you (pointing forward).
/// 2. Hold the LEFT TRIGGER (squeeze ≥ 50 %).
/// 3. Flick / swipe your wrist DOWNWARD  (downward speed ≥ 1.5 m/s).
/// The menu opens (or closes if already open).
/// A 0.8 s cooldown prevents accidental double-fires.
/// </summary>
public class Quest2InputHandler : MonoBehaviour
{
    // ── Inspector references (auto-found if left empty) ──────────────────────
    [Header("References (auto-found if left empty)")]
    [SerializeField] private VRSwordController rightHandSword;
    [SerializeField] private VRSwordController leftHandSword;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform headCameraTransform;
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

    [Header("Menu Gesture – Left Wrist Swipe Down + Trigger")]
    [Tooltip("Minimum downward wrist speed (m/s) to trigger the menu gesture.")]
    [SerializeField] private float menuSwipeVelocityThreshold = 1.5f;
    [Tooltip("How far forward the left hand must point before the gesture counts.\n" +
             "Dot product of the hand-to-body-forward direction (0 = anywhere, 1 = exactly forward).\n" +
             "Default: 0.3 — hand needs to be roughly in front of you.")]
    [SerializeField] private float menuHandForwardDot = 0.3f;
    [Tooltip("Left trigger value (0-1) that must be held during the gesture.")]
    [SerializeField] private float menuTriggerThreshold = 0.5f;
    [Tooltip("Seconds to wait before the same gesture can fire again (prevents double-fire).")]
    [SerializeField] private float menuGestureCooldown = 0.8f;

    // ── Device handles ────────────────────────────────────────────────────────
    private InputDevice rightController;
    private InputDevice leftController;

    // ── Button / trigger state ────────────────────────────────────────────────
    private bool rightGripHeld;
    private bool leftGripHeld;
    private bool rightTriggerHeld;
    private bool leftTriggerHeld;
    private bool aButtonHeld;
    private bool bButtonHeld;
    private bool snapTurnUsed;

    // ── Menu gesture state ────────────────────────────────────────────────────
    private Vector3 leftHandPrevPos;
    private bool    leftHandPrevPosValid;
    private float   menuGestureCooldownTimer;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        AutoFindReferences();
        TryAcquireDevices();
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
        if (headCameraTransform == null && Camera.main != null)
            headCameraTransform = Camera.main.transform;
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

    private void OnDeviceConnected(InputDevice device) => TryAcquireDevices();

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
        PollMenuGesture();
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
            VRSwordController rightSword = rightHandSword;
            if (rightSword != null && rightSword.IsHeld &&
                rightSword.Type == SwordType.TwoHanded)
            {
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
        if (sword == null || anchor == null || sword.IsHeld) return;
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
        // Right trigger → basic slash
        rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigVal);
        bool rightTrigDown = rightTrigVal > 0.5f;
        if (rightTrigDown && !rightTriggerHeld)
        {
            rightTriggerHeld = true;
            playerCombat?.TryExecuteSkill("basic_slash");
        }
        else if (!rightTrigDown) rightTriggerHeld = false;

        // Left trigger – used by PollMenuGesture; track state for other systems
        leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTrigVal);
        bool leftTrigDown = leftTrigVal > menuTriggerThreshold;
        if (leftTrigDown && !leftTriggerHeld)  leftTriggerHeld = true;
        else if (!leftTrigDown)                leftTriggerHeld = false;
    }

    // ── Face buttons ──────────────────────────────────────────────────────────

    private void PollButtons()
    {
        // A → toggle debug overlay
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aDown);
        if (aDown && !aButtonHeld)
        {
            aButtonHeld = true;
            if (testModeOverlay != null) testModeOverlay.ToggleOverlay();
            else Debug.Log("[Quest2] A – overlay toggle (no VRPrototypeTestMode found).");
        }
        else if (!aDown) aButtonHeld = false;

        // B → despawn enemies
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bDown);
        if (bDown && !bButtonHeld)
        {
            bButtonHeld = true;
            if (enemySpawner != null)
            {
                enemySpawner.DespawnAll();
                Debug.Log("[Quest2] B – enemies despawned.");
            }
        }
        else if (!bDown) bButtonHeld = false;

        // Y / X buttons are now reserved (menu opened via wrist-swipe gesture)
    }

    // ── Menu gesture: hand forward + left trigger + wrist swipe DOWN ──────────

    private void PollMenuGesture()
    {
        // Tick down cooldown; keep tracking position so velocity is valid after cooldown expires
        if (menuGestureCooldownTimer > 0f)
        {
            menuGestureCooldownTimer -= Time.deltaTime;
            if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 p))
            {
                leftHandPrevPos      = p;
                leftHandPrevPosValid = true;
            }
            return;
        }

        // Read left controller world-space position (provided by the XR rig)
        if (!leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos))
        {
            leftHandPrevPosValid = false;
            return;
        }

        // Need at least one previous frame to compute velocity
        if (!leftHandPrevPosValid)
        {
            leftHandPrevPos      = leftPos;
            leftHandPrevPosValid = true;
            return;
        }

        // ── Condition 1: left trigger held ───────────────────────────────────
        leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTrig);
        if (leftTrig < menuTriggerThreshold)
        {
            leftHandPrevPos = leftPos;
            return;
        }

        // ── Condition 2: hand extended forward relative to head camera ────────
        if (headCameraTransform != null)
        {
            Vector3 camForwardFlat = headCameraTransform.forward;
            camForwardFlat.y = 0f;
            Vector3 handOffsetFlat = leftPos - headCameraTransform.position;
            handOffsetFlat.y = 0f;

            if (camForwardFlat.sqrMagnitude > 0.001f && handOffsetFlat.sqrMagnitude > 0.001f)
            {
                float dot = Vector3.Dot(camForwardFlat.normalized, handOffsetFlat.normalized);
                if (dot < menuHandForwardDot)
                {
                    leftHandPrevPos = leftPos;
                    return;
                }
            }
        }

        // ── Condition 3: downward wrist velocity spike ────────────────────────
        float dt = Time.deltaTime;
        // Guard against near-zero dt that would produce unrealistically large velocities
        float vertVelocity = dt > 0.001f ? (leftPos.y - leftHandPrevPos.y) / dt : 0f;
        leftHandPrevPos = leftPos;

        if (vertVelocity > -menuSwipeVelocityThreshold)
            return; // downward velocity too slow (must be < -menuSwipeVelocityThreshold m/s)

        // ── All three conditions met → toggle menu ────────────────────────────
        menuGestureCooldownTimer = menuGestureCooldown;
        Debug.Log($"[Quest2] Menu gesture fired (downVel={vertVelocity:F2} m/s, trig={leftTrig:F2}).");

        SAOMenuController saoMenu = SAOMenuController.Instance;
        if (saoMenu != null)
        {
            saoMenu.ToggleMenu();
        }
        else
        {
            VRMenuSystem menuSys = VRMenuSystem.Instance;
            if (menuSys != null) menuSys.ToggleMenu();
            else Debug.LogWarning("[Quest2] Menu gesture – no menu controller found in scene.");
        }
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
            snapTurnUsed = false;
        }
    }
}

using UnityEngine;
using Valve.VR;

/// <summary>
/// Handles SteamVR controller input for combat and abilities.
/// </summary>
public class VRInputManager : MonoBehaviour
{
    [SerializeField] private SteamVR_Input_Sources handType = SteamVR_Input_Sources.RightHand;
    [SerializeField] private PlayerCombat playerCombat;

    private void Update()
    {
        HandleAttackInput();
        HandleBlockInput();
    }

    private void HandleAttackInput()
    {
        // This will be connected to SteamVR trigger
        // For now, test with keyboard
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerCombat != null)
                playerCombat.TryExecuteSkill("basic_slash");
        }
    }

    private void HandleBlockInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Blocking...");
        }
    }
}
using UnityEngine;

/// <summary>
/// Manages world state including current floor.
/// All systems reference this for scaling.
/// </summary>
public class WorldManager : MonoBehaviour
{
    private static WorldManager instance;

    [SerializeField] private int currentFloor = 1;
    [SerializeField] private string currentFloorName = "Floor 1: The Beginning";
    [SerializeField] private int playersOnFloor = 0;
    [SerializeField] private int guildCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DamageScalingSystem.Instance.SetFloor(currentFloor);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static WorldManager Instance => instance;

    public void AdvanceFloor(int newFloor)
    {
        currentFloor = Mathf.Max(1, Mathf.Min(100, newFloor)); // Clamp 1-100
        DamageScalingSystem.Instance.SetFloor(currentFloor);

        UpdateFloorName();

        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║  🏰 Reached {currentFloorName,26} 🏰  ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");
    }

    private void UpdateFloorName()
    {
        currentFloorName = currentFloor switch
        {
            1 => "Floor 1: The Beginning",
            10 => "Floor 10: The Mid-tier Realm",
            25 => "Floor 25: Escalation Point",
            50 => "Floor 50: The Halfway Mark",
            75 => "Floor 75: The Final Ascent",
            100 => "Floor 100: The Floating Castle",
            _ => $"Floor {currentFloor}"
        };
    }

    public int GetCurrentFloor() => currentFloor;
    public string GetCurrentFloorName() => currentFloorName;
    public float GetDamageMultiplier() => DamageScalingSystem.Instance.GetCurrentMultiplier();

    public void RegisterPlayer()
    {
        playersOnFloor++;
    }

    public void UnregisterPlayer()
    {
        playersOnFloor = Mathf.Max(0, playersOnFloor - 1);
    }

    public void RegisterGuild()
    {
        guildCount++;
    }

    public int GetPlayersOnFloor() => playersOnFloor;
    public int GetGuildsOnFloor() => guildCount;
}
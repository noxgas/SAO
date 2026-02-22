using UnityEngine;

/// <summary>
/// Global damage scaling system based on floor level.
/// Boss health scales with MEDIAN damage of the NEXT floor.
/// This allows players to challenge harder bosses early for rare drops.
/// 
/// Example:
/// Floor 1 Players: 30-50 DMG (Median: 40)
/// Floor 1 Boss Health: Scales to Floor 2 median (54 DMG) = ~1,215 HP
/// 
/// Floor 10 Players: 65-108 DMG (Median: 86)
/// Floor 10 Boss Health: Scales to Floor 11 median (~93) = ~2,325 HP
/// 
/// This creates a "challenge zone" mechanic where players can
/// fight bosses on floors they're not yet equipped for.
/// </summary>
public class DamageScalingSystem : MonoBehaviour
{
    private static DamageScalingSystem instance;

    [Header("Scaling Configuration")]
    [SerializeField] private float baseScaleMultiplier = 1.08f; // 8% per floor
    [SerializeField] private int baseFloor = 1;
    [SerializeField] private float baseWeaponMinDamage = 30f;
    [SerializeField] private float baseWeaponMaxDamage = 50f;
    [SerializeField] private float baseBossHealth = 1000f;

    private int currentFloor = 1;
    private float currentScaleMultiplier = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DamageScalingSystem Instance => instance;

    /// <summary>
    /// Set current floor and recalculate all scaling.
    /// </summary>
    public void SetFloor(int floor)
    {
        currentFloor = Mathf.Max(1, Mathf.Min(100, floor));
        currentScaleMultiplier = CalculateScaleMultiplier(currentFloor);

        Debug.Log($"📍 Floor changed to {currentFloor}");
        Debug.Log($"📊 Damage multiplier: {currentScaleMultiplier:F2}x");
    }

    /// <summary>
    /// Calculate the scaling multiplier for a given floor.
    /// </summary>
    private float CalculateScaleMultiplier(int floor)
    {
        return Mathf.Pow(baseScaleMultiplier, floor - baseFloor);
    }

    /// <summary>
    /// Scale any damage value to current floor.
    /// </summary>
    public float ScaleDamage(float baseDamage, int forFloor = -1)
    {
        if (forFloor == -1)
            forFloor = currentFloor;

        float floorMultiplier = CalculateScaleMultiplier(forFloor);
        return baseDamage * floorMultiplier;
    }

    /// <summary>
    /// Get weapon damage for a specific floor.
    /// Base Floor 1: 30-50 per swing
    /// </summary>
    public float GetWeaponDamageMin(int floor = -1, float baseMin = 30f)
    {
        if (floor == -1)
            floor = currentFloor;
        return ScaleDamage(baseMin, floor);
    }

    public float GetWeaponDamageMax(int floor = -1, float baseMax = 50f)
    {
        if (floor == -1)
            floor = currentFloor;
        return ScaleDamage(baseMax, floor);
    }

    public float GetRandomWeaponDamage(int floor = -1, float baseMin = 30f, float baseMax = 50f)
    {
        if (floor == -1)
            floor = currentFloor;

        float minDamage = GetWeaponDamageMin(floor, baseMin);
        float maxDamage = GetWeaponDamageMax(floor, baseMax);
        return Random.Range(minDamage, maxDamage);
    }

    /// <summary>
    /// Calculate MEDIAN weapon damage for a floor.
    /// Used for boss health scaling.
    /// Median = (Min + Max) / 2
    /// </summary>
    public float GetMedianWeaponDamage(int floor, float baseMin = 30f, float baseMax = 50f)
    {
        float baseMedian = (baseMin + baseMax) / 2f;
        return ScaleDamage(baseMedian, floor);
    }

    /// <summary>
    /// Get boss health for a specific floor.
    /// Boss health scales to the MEDIAN damage of the NEXT floor.
    /// 
    /// Floor 1 Boss: Scales to Floor 2 median damage
    /// Floor 10 Boss: Scales to Floor 11 median damage
    /// 
    /// This creates progression challenge where players can optionally
    /// fight bosses of harder floors for rare drops.
    /// </summary>
    public float GetBossHealth(int floor, float baseHealth = 1000f)
    {
        // Boss health scales to NEXT floor's median damage
        int scalingFloor = Mathf.Min(floor + 1, 100);
        float nextFloorMedianDamage = GetMedianWeaponDamage(scalingFloor);

        // Boss health = base * (next floor median / base median)
        float baseMedian = GetMedianWeaponDamage(floor);
        float healthScale = nextFloorMedianDamage / baseMedian;

        return baseHealth * healthScale;
    }

    /// <summary>
    /// Get boss health for current floor.
    /// </summary>
    public float GetBossHealthCurrent(float baseHealth = 1000f)
    {
        return GetBossHealth(currentFloor, baseHealth);
    }

    /// <summary>
    /// Get loot value for current floor.
    /// </summary>
    public float GetLootValue(float baseLootValue)
    {
        return ScaleDamage(baseLootValue);
    }

    public int GetCurrentFloor() => currentFloor;
    public float GetCurrentMultiplier() => currentScaleMultiplier;

    /// <summary>
    /// Get estimated time to kill boss with a party.
    /// Accounts for next-floor scaling.
    /// </summary>
    public float EstimateTimeToKill(int floor, int partySize, float averageDPS)
    {
        float bossHealth = GetBossHealth(floor);
        float totalPartyDPS = averageDPS * partySize;
        return bossHealth / totalPartyDPS;
    }

    /// <summary>
    /// Get difficulty info for a floor.
    /// Shows recommended stats and party size.
    /// </summary>
    public void PrintFloorDifficultyInfo(int floor)
    {
        float minDamage = GetWeaponDamageMin(floor);
        float maxDamage = GetWeaponDamageMax(floor);
        float medianDamage = GetMedianWeaponDamage(floor);
        float bossHealth = GetBossHealth(floor);
        float ttk4Man = EstimateTimeToKill(floor, 4, medianDamage);
        float ttk8Man = EstimateTimeToKill(floor, 8, medianDamage);

        Debug.Log($"\n╔════════════════════════════════════════════════════╗");
        Debug.Log($"║  FLOOR {floor} DIFFICULTY INFO".PadRight(53) + "║");
        Debug.Log($"╠════════════════════════════════════════════════════╣");
        Debug.Log($"║  Weapon Damage Range: {minDamage:F0}-{maxDamage:F0}".PadRight(53) + "║");
        Debug.Log($"║  Median Damage: {medianDamage:F0}".PadRight(53) + "║");
        Debug.Log($"║  Boss Health: {bossHealth:F0} HP".PadRight(53) + "║");
        Debug.Log($"║  Recommended Party: 4-8 players".PadRight(53) + "║");
        Debug.Log($"║  TTK (4-Man): {ttk4Man:F1}s | TTK (8-Man): {ttk8Man:F1}s".PadRight(53) + "║");
        Debug.Log($"╚════════════════════════════════════════════════════╝");
    }
}
// Add these methods to PlayerCombat.cs

private float staminaRegenRate = 30f;
private float manaRegenRate = 25f;

public float GetStaminaRegenRate() => staminaRegenRate;
public float GetManaRegenRate() => manaRegenRate;

public void SetStaminaRegenRate(float newRate)
{
    staminaRegenRate = Mathf.Max(0, newRate);
}

public void SetManaRegenRate(float newRate)
{
    manaRegenRate = Mathf.Max(0, newRate);
}
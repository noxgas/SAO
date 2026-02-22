// When you deal damage to a boss, call it like this:
// Example in your combat system:

public void DealDamageToBoss(BossEnemy boss, float damage, Player attacker)
{
    boss.TakeDamage(damage, attacker);
}
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Alerted,
    Chase,
    Combat,
    Dead
}

/// <summary>
/// Base AI enemy system with patrol, detection, and combat.
/// </summary>
public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackDamage = 5f;
    [Tooltip("Seconds between attacks. Prevents the enemy dealing damage every frame.")]
    [SerializeField] private float attackInterval = 1f;

    private EnemyState currentState = EnemyState.Idle;
    private Player targetPlayer;
    private float attackTimer;

    private void Update()
    {
        DetectPlayer();
        UpdateAI();
    }

    private void DetectPlayer()
    {
        // Already dead – nothing to do
        if (currentState == EnemyState.Dead) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var collider in colliders)
        {
            Player player = collider.GetComponent<Player>();
            if (player != null && HasLineOfSight(player.transform))
            {
                targetPlayer = player;
                // Only transition to Alerted/Chase from idle/patrol states.
                // Do NOT override Chase or Combat – let UpdateAI handle those.
                if (currentState == EnemyState.Idle || currentState == EnemyState.Patrol)
                    currentState = EnemyState.Chase;
                return;
            }
        }

        // No player detected this frame: return to patrol if we haven't locked onto anyone yet
        if (targetPlayer == null)
            currentState = EnemyState.Patrol;
    }

    private bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, directionToTarget, out hit, sightRange))
        {
            return hit.transform == target;
        }
        return false;
    }

    private void UpdateAI()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Combat:
                Attack();
                break;
        }
    }

    private void Chase()
    {
        if (targetPlayer == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
        
        if (distanceToPlayer < attackRange)
        {
            currentState = EnemyState.Combat;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        if (targetPlayer == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        // Return to chase if player has moved out of attack range
        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        // Attack cooldown – prevents dealing damage every frame
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        attackTimer = attackInterval;
        PlayerCombat combat = targetPlayer.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentState = EnemyState.Dead;
        Destroy(gameObject);
    }
}
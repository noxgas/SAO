using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Alerted,
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

    private EnemyState currentState = EnemyState.Idle;
    private Player targetPlayer;

    private void Update()
    {
        DetectPlayer();
        UpdateAI();
    }

    private void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var collider in colliders)
        {
            Player player = collider.GetComponent<Player>();
            if (player != null && HasLineOfSight(player.transform))
            {
                targetPlayer = player;
                currentState = EnemyState.Alerted;
                return;
            }
        }

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

        PlayerCombat combat = targetPlayer.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.TakeDamage(5f);
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
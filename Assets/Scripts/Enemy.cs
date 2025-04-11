using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum EnemyState { Wander, Pursue, Attack, Recovery }
    EnemyState currentState;

    public float wanderRange = 10f;
    public float playerSightRange = 15f;
    public float playerAttackRange = 3f;
    public float recoveryTime = 2f;

    private float currentStateElapsed = 0f;
    private Vector3 wanderTarget;

    public Rigidbody Rigidbody { get; private set; }
    Vector3 origin;

    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No object with tag 'Player' found.");
        }

        origin = transform.position;
        SwitchState(EnemyState.Wander);
    }

    void Update()
    {
        currentStateElapsed += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Wander:
                UpdateWander();
                break;

            case EnemyState.Pursue:
                UpdatePursue();
                break;

            case EnemyState.Attack:
                
                break;

            case EnemyState.Recovery:
                if (currentStateElapsed >= recoveryTime)
                {
                    SwitchState(EnemyState.Pursue);
                }
                break;
        }
    }

    void SwitchState(EnemyState newState)
    {
        currentState = newState;
        currentStateElapsed = 0f;

        switch (newState)
        {
            case EnemyState.Wander:
                wanderTarget = origin + Random.insideUnitSphere * wanderRange;
                wanderTarget.y = origin.y;
                agent.isStopped = false;
                agent.SetDestination(wanderTarget);
                break;

            case EnemyState.Pursue:
                agent.isStopped = false;
                break;

            case EnemyState.Attack:
                agent.isStopped = true;
                if (player != null)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    Rigidbody.linearVelocity = Vector3.zero;
                    Rigidbody.AddForce(direction * 500f);
                }
                Invoke(nameof(FallbackToRecovery), 1.5f);
                break;

            case EnemyState.Recovery:
                agent.isStopped = true;
                Rigidbody.linearVelocity = Vector3.zero;
                break;
        }
    }

    void UpdateWander()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= playerSightRange)
        {
            SwitchState(EnemyState.Pursue);
            return;
        }

        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            wanderTarget = origin + Random.insideUnitSphere * wanderRange;
            wanderTarget.y = origin.y;
            agent.SetDestination(wanderTarget);
        }
    }

    void UpdatePursue()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= playerAttackRange)
        {
            SwitchState(EnemyState.Attack);
        }
        else if (distanceToPlayer > playerSightRange)
        {
            SwitchState(EnemyState.Wander);
        }
    }

    void FallbackToRecovery()
    {
        if (currentState == EnemyState.Attack)
        {
            SwitchState(EnemyState.Recovery);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);

        if (currentState == EnemyState.Attack && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy!");
            CancelInvoke(nameof(FallbackToRecovery));
            SwitchState(EnemyState.Recovery);
        }
    }

    public void ApplyKnockback(Vector3 knockback)
    {
        Rigidbody.AddForce(knockback, ForceMode.Impulse);
    }

    public void Respawn()
    {
        transform.position = origin;
    }
}

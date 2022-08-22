using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackRange = 2f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float patrolRadius = 3f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float patrolSpeed = 4f;
    [SerializeField] float chaseSpeed = 5f;
    [SerializeField] float attackRate = 2f;
    [SerializeField] int damage = 2;
    [SerializeField] AnimatorOverrideController AnimatorOverrideController;

    [SerializeField] private Animator meshAnimator;

    [SerializeField] private GameObject fx;


    public int score = 5;

    private Health health;

    private NavMeshAgent agent;


    public State currentState = State.Idle;

    Transform player;

    private bool isSearched = false;

    private bool attackProcess = false;

    bool check = false;

    public enum State
    {
        Idle,
        Search,
        Chase,
        Attack
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        health = GetComponent<Health>();
        meshAnimator.runtimeAnimatorController = AnimatorOverrideController;
    }
    void Update()
    {
        if (player == null)
        {
            currentState = State.Idle;
            meshAnimator.SetInteger("State", (int)currentState);
            agent.isStopped = true;
            agent.velocity = agent.velocity * 0.1f;
            return;
        }

        StateCheck();

        if (GameManager.attackingCount >= 10)
        {
            if (currentState == State.Chase || currentState == State.Search)
            {
                currentState = State.Idle;
            }
            meshAnimator.SetInteger("State", (int)currentState);
            agent.isStopped = true;
            agent.velocity = agent.velocity * 0.1f;
        }
        StateExecute();
    }
    private void OnEnable()
    {
        GameManager.instance.allEnemies.Add(this);
    }
    private void OnDisable()
    {
        GameManager.instance.allEnemies.Remove(this);
    }

    public void FX()
    {
        if (fx != null)
        {
            try
            {
                if (player == null)
                {
                    return;
                }

                Instantiate(fx.gameObject, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            catch
            {

            }
        }
    }

    private void StateExecute()
    {
        switch (currentState)
        {
            case State.Search:
 
                SearchNewTarget();
                break;
            case State.Chase:

                Chase();
                break;
            case State.Attack:

                Attack();
                break;
            default:
                break;
        }
    }

    private void SearchNewTarget()
    {
        if (agent.remainingDistance <= 0.1f && !isSearched ||
                            !agent.hasPath && !isSearched)
        {
            Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);

            agent.enabled = false;
            transform.position = agentTarget;
            agent.enabled = true;

            Invoke("Search", patrolWaitTime);

            isSearched = true;
        }
    }

    private void Attack()
    {
        agent.isStopped = true;
        agent.velocity = agent.velocity * 0.1f;
        LookTheTarget(player.position);
        if (!attackProcess)
        {
            StartCoroutine(GiveDamage());
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void Search()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(GetRandomPosition());
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        return hit.position;
    }

    private void StateCheck()
    {
        float distanceToTarget = Vector3.Distance(player.position, transform.position);

        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange ||
            health.GetCurrentHealth() < health.maxHealth && distanceToTarget > attackRange)
        {
            currentState = State.Chase;
            if (check)
            {
                GameManager.attackingCount -= 1;
                check = false;
            }
        }
        else if (distanceToTarget <= attackRange)
        {
            currentState = State.Attack;

            if (!check)
            {
                GameManager.attackingCount += 1;
                check = true;
            }
        }
        else
        {
            if (check)
            {
                GameManager.attackingCount -= 1;
                check = false;
            }
            currentState = State.Search;
        }


        meshAnimator.SetInteger("State", (int)currentState);

    }

    IEnumerator GiveDamage()
    {
        attackProcess = true;
        player.GetComponent<Health>().AddDamage(damage);
        yield return new WaitUntil(() => meshAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && meshAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.55);
        yield return new WaitForSeconds(attackRate);
        attackProcess = false;
    }
    private void OnDrawGizmosSelected()
    {
        switch (currentState)
        {
            case State.Search:
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, chaseRange);
                break;
            case State.Chase:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.position);
                break;
            case State.Attack:
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
                break;
        }
    }
    private void LookTheTarget(Vector3 target)
    {
        Vector3 lookPos = new Vector3(target.x, transform.position.y, target.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position),
            turnSpeed * Time.deltaTime);
    }
}

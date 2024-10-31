
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float walkPointRange;
    public bool isWalking;
    public Vector3 walkPoint;
    public float delayAttackTime;
    private bool isAttacking;
    public Animator animator;
    public int health;
    public float movementSpeed;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        if (!playerInSightRange && !playerInAttackRange && !isAttacking)
        {
            Patrolling();
            animator.SetFloat("Speed", 0.5f);
            animator.SetBool("isAttacking", false);
        }
        else if (playerInSightRange && !playerInAttackRange && !isAttacking)
        {
            Chasing();
            animator.SetFloat("Speed", 1f);
            animator.SetBool("isAttacking", false);

        }
        else
        {
            Attacking();
            animator.SetBool("isAttacking", true);
        }
    }
    private void Patrolling()
    {
        if (!isWalking)
        {
            walkPoint = LookForWalkPoint();
            if (walkPoint == Vector3.zero) isWalking = false;
            else isWalking = true;
        }
        else agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            isWalking = false;
        }
        agent.speed = 3;
    }
    private Vector3 LookForWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 walkPointTemp = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPointTemp, -transform.up, 2f, groundMask))
        {
            return walkPointTemp;
        }
        return Vector3.zero;
    }
    private void Chasing()
    {
        agent.SetDestination(player.position);
        agent.speed = 8;
    }
    private void Attacking()
    {
        agent.SetDestination(transform.position);
        // transform.LookAt(player);
        if (!isAttacking)
        {
            isAttacking = true;
            Invoke(nameof(ResetAttack), delayAttackTime);
        }
    }
    private void ResetAttack()
    {
        isAttacking = false;
    }
    private void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), .5f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}

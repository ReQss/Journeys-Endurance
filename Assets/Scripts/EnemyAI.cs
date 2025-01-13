
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public AudioSource walkingSound, runningSound, attackingSound;
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
    public int health = 3;
    public float movementSpeed;
    private bool isTakingDamage = false;
    public float invincibleDelayTime = 0.05f;
    [SerializeField]
    private GameObject healthBar;
    public LayerMask playerLayer;
    private bool isTakingBulletDamage = false;

    private void Start()
    {
        healthBar.GetComponent<HealthBar>().SetMaxHealth(health);
    }
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
        if (GameManager.Instance.huntingTime)
        {
            sightRange = 1000;
        }
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
            StartCoroutine(CheckForDamage());
        }
    }
    private IEnumerator CheckForDamage()
    {
        yield return new WaitForSeconds(.5f);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.8f, playerLayer);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player") && GameManager.Instance.isInvincible == false)
            {
                Debug.Log("Hit player");
                player.GetComponent<ThirdPersonMovement>().TakeDamage();
                GameManager.Instance.isInvincible = true;
                yield return new WaitForSeconds(1f);
                GameManager.Instance.isInvincible = false;
            }
        }

    }
    private IEnumerator resetPoint()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            isWalking = false;
        }
    }
    private void Patrolling()
    {
        walkingSound.enabled = true;
        runningSound.enabled = false;
        attackingSound.enabled = false;
        StartCoroutine(resetPoint());
        if (!isWalking)
        {
            walkPoint = LookForWalkPoint();
            if (walkPoint == Vector3.zero) isWalking = false;
            else isWalking = true;
        }
        else agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        // Debug.Log(distanceToWalkPoint);
        if (distanceToWalkPoint.magnitude < 1.5f)
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
        walkingSound.enabled = false;
        runningSound.enabled = true;
        attackingSound.enabled = false;
        agent.SetDestination(player.position);
        agent.speed = movementSpeed;
    }
    private void Attacking()
    {
        walkingSound.enabled = false;
        runningSound.enabled = false;
        attackingSound.enabled = true;
        agent.SetDestination(transform.position);
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
    public IEnumerator TakeDamage(int damage)
    {
        if (isTakingDamage) yield break;
        isTakingDamage = true;
        health -= damage;
        healthBar.GetComponent<HealthBar>().TakeDamage(damage);
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
            yield break;
        }
        yield return new WaitForSeconds(invincibleDelayTime);
        isTakingDamage = false;

    }
    public IEnumerator TakeBulletDamage(int damage)
    {
        if (isTakingBulletDamage) yield break;
        isTakingBulletDamage = true;
        health -= damage;
        healthBar.GetComponent<HealthBar>().TakeDamage(damage);
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
            yield break;
        }
        yield return new WaitForSeconds(1f);
        isTakingBulletDamage = false;

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet") && !isTakingDamage)
        {

            StartCoroutine(TakeDamage(1));
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}

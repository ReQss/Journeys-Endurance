using System.Collections;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public Transform player;
    public LayerMask playerMask;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float movementSpeed;
    public float hoverHeight = 10f;
    public float attackDelay = 1f;
    public int health = 3;
    public GameObject healthBar;
    private bool isAttacking = false;
    private bool isTakingDamage = false;
    public Animator animator;
    public GameObject bulletPrefab;
    public Transform attack_pos;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        healthBar.GetComponent<HealthBar>().SetMaxHealth(health);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patrolling();
            animator.SetFloat("Speed", 0.5f);
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Chasing();
            animator.SetFloat("Speed", 1f);
        }
        else if (playerInAttackRange)
        {
            if (!isAttacking)
            {

                StartCoroutine(AttackContinuously());
            }
            animator.SetBool("isAttacking", true);
        }
    }

    private IEnumerator AttackContinuously()
    {
        isAttacking = true;
        while (playerInAttackRange)  // While the player is within attack range, spawn bullets
        {
            GameObject bullet = Instantiate(bulletPrefab, attack_pos.position, Quaternion.identity);

            Vector3 directionToPlayer = (player.position - attack_pos.position).normalized;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * 40f;
            }
            bullet.transform.LookAt(player);

            yield return new WaitForSeconds(0.1f);  // Delay between each bullet spawn (adjust as needed)
        }
        isAttacking = false;
    }


    private void Patrolling()
    {
        Vector3 hoverPosition = new Vector3(transform.position.x, hoverHeight, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, hoverPosition, Time.deltaTime * movementSpeed / 2);
    }

    private void Chasing()
    {
        float enemyHeight = player.position.y < hoverHeight ? hoverHeight : player.position.y;

        Vector3 targetPosition = new Vector3(player.position.x, enemyHeight, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * movementSpeed);

    }

    private void Attacking()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Invoke(nameof(ResetAttack), attackDelay);

            GameObject bullet = Instantiate(bulletPrefab, attack_pos.position, Quaternion.identity);

            Vector3 directionToPlayer = (player.position - attack_pos.position).normalized;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * 40f;
            }
            bullet.transform.LookAt(player);
        }
    }



    private void ResetAttack()
    {
        if (player != null)
        {
            // player.GetComponent<ThirdPersonMovement>().TakeDamage();
            Debug.Log("Player damaged by flying enemy!");
        }
        isAttacking = false;
    }

    public IEnumerator TakeDamage(int damage)
    {
        if (isTakingDamage) yield break;
        isTakingDamage = true;
        health -= damage;
        if (healthBar != null)
            healthBar.GetComponent<HealthBar>().TakeDamage(damage);

        if (health <= 0)
        {
            DestroyEnemy();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        isTakingDamage = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(TakeDamage(1));
        }
    }
}

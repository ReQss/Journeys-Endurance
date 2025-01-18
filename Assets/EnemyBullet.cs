using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MagicBullet
{
    public bool canDealDamage = false;
    public void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        Collider[] foundObjects = Physics.OverlapSphere(transform.position, radius, layer);
        float closestDistance = Mathf.Infinity;
        foreach (Collider collider in foundObjects)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = collider;
            }
        }
        if (closestObject != null)
        {
            wasEnemyFounded = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Vector3 direction = (closestObject.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, closestObject.transform.position, moveSpeed * Time.deltaTime);
            ThirdPersonMovement foundedEnemy = closestObject.GetComponent<ThirdPersonMovement>();
            if (foundedEnemy != null)
            {
                if (bulletDamage != 0)
                {
                    // if (canDealDamage)
                    // {
                    //     foundedEnemy.GetComponent<ThirdPersonMovement>().TakeDamage();
                    //     StartCoroutine(InvicibleTime());
                    // }
                    // Destroy(gameObject);
                }
            }

        }
        else if (wasEnemyFounded)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player damaged");

            ThirdPersonMovement player = other.gameObject.GetComponent<ThirdPersonMovement>();
            if (player != null)
            {
                player.TakeDamage();
                StartCoroutine(InvicibleTime());
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator InvicibleTime()
    {
        GameManager.Instance.isInvincible = true;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.isInvincible = false;
    }

    public IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(destroyBulletTime);
        Destroy(gameObject);
        yield return null;
    }
}

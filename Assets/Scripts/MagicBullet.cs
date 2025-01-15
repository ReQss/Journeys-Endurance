using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask layer;
    public float radius = 5f;
    public float moveSpeed = 5f;
    private bool wasEnemyFounded = false;
    public Collider closestObject = null;
    public int bulletDamage = 1;
    public float destroyBulletTime = 15f;
    public void Start()
    {

        StartCoroutine(DestroyBullet());
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
            EnemyAI foundedEnemy = closestObject.GetComponent<EnemyAI>();
            if (foundedEnemy != null)
            {
                if (bulletDamage != 0)
                    StartCoroutine(foundedEnemy.TakeBulletDamage(bulletDamage));
            }

        }
        else if (wasEnemyFounded)
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(destroyBulletTime);
        Destroy(gameObject);
        yield return null;
    }
}

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
    void Start()
    {

        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] foundObjects = Physics.OverlapSphere(transform.position, radius, layer);
        Collider closestObject = null;
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
                StartCoroutine(foundedEnemy.TakeBulletDamage(1));
            }

        }
        else if (wasEnemyFounded)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
        yield return null;
    }
}

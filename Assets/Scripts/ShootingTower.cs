using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTower : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject bullet;
    public float enemyDetectionRadius = 10f;
    public LayerMask enemyLayer;
    private bool enemyDetected;
    private Coroutine shootingCoroutine;
    [SerializeField]
    private GameObject tower;
    [SerializeField]
    private float bulletSpeed = 20f;
    [SerializeField]
    private float attackDelay = 4f;
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius, enemyLayer);
        enemyDetected = hitColliders.Length > 0;

        if (enemyDetected && shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(ShootAtEnemies());
        }
        else if (!enemyDetected && shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = enemyDetected ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);
    }

    private IEnumerator ShootAtEnemies()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius, enemyLayer);
            foreach (Collider collider in hitColliders)
            {

                Shoot(collider.transform);
            }

            yield return new WaitForSeconds(attackDelay);
        }
    }

    private void Shoot(Transform enemy)
    {
        GameObject temp = Instantiate(bullet, shootingPoint.position, Quaternion.identity);

        Destroy(temp, 3f);
        temp.transform.LookAt(enemy);

        if (tower != null)
        {
            tower.transform.LookAt(enemy);
            // tower.transform.eulerAngles = new Vector3(90f, tower.transform.eulerAngles.y, tower.transform.eulerAngles.z);
            // tower.transform.eulerAngles = new Vector3(tower.transform.eulerAngles.x, -180f, tower.transform.eulerAngles.z);

        }
        temp.GetComponent<Rigidbody>().velocity = temp.transform.forward * bulletSpeed;
        temp.transform.eulerAngles = new Vector3(180f, temp.transform.eulerAngles.y, temp.transform.eulerAngles.z);
    }
}

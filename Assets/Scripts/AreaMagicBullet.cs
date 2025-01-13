using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMagicBullet : MagicBullet
{
    private bool isEveryEnemyHitted = false;
    void Start()
    {
        base.Start();

    }

    void Update()
    {
        base.Update();
        if (isEveryEnemyHitted == true) return;
        Collider[] foundObjects = Physics.OverlapSphere(transform.position, radius, layer);
        foreach (Collider collider in foundObjects)
        {
            EnemyAI enemy = collider.GetComponent<EnemyAI>();
            Debug.Log("Damage dealt to" + enemy.gameObject);
            enemy.TakeBulletDamage(bulletDamage);
        }
        isEveryEnemyHitted = true;

    }


}

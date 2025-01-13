using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBullet : MagicBullet
{
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
        if (closestObject != null)
        {
            EnemyAI foundedEnemy = closestObject.GetComponent<EnemyAI>();
            if (foundedEnemy != null && !foundedEnemy.isFrozen)
            {
                foundedEnemy.movementSpeed /= 4;
                foundedEnemy.agent.speed = foundedEnemy.movementSpeed;
                foundedEnemy.isFrozen = true;
                StartCoroutine(ResetSpeed(foundedEnemy, 5f));
            }
        }
    }

    private IEnumerator ResetSpeed(EnemyAI enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        enemy.movementSpeed *= 2;
        enemy.agent.speed = enemy.movementSpeed;
        enemy.isFrozen = false;
    }
}

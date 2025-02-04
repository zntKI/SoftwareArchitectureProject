using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns Strong Enemies first, then the Weak ones
/// </summary>
public class StrongFirstOrderStrategy : SpawnOrderStrategy
{
    public override void PickEnemy()
    {
        EnemyController spawnedEnemy = null;

        if (currentWaveProperties.StrongEnemies > 0)
        {
            spawnedEnemy = Instantiate(enemyStrongPrefab, this.transform.position, Quaternion.identity).GetComponent<EnemyController>();
            currentWaveProperties.ReduceStrongEnemies();
        }
        else if (currentWaveProperties.WeakEnemies > 0)
        {
            spawnedEnemy = Instantiate(enemyWeakPrefab, this.transform.position, Quaternion.identity).GetComponent<EnemyController>();
            currentWaveProperties.ReduceWeakEnemies();
        }

        EnemySpawned(spawnedEnemy);
    }
}

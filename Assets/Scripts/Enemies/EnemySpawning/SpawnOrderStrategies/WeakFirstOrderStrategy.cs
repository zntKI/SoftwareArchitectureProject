using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns Weak Enemies first, then the Strong ones
/// </summary>
public class WeakFirstOrderStrategy : SpawnOrderStrategy
{
    public override void PickEnemy()
    {
        EnemyController spawnedEnemy = null;

        if (currentWaveProperties.WeakEnemies > 0)
        {
            spawnedEnemy = Instantiate(enemyWeakPrefab, this.transform.position, Quaternion.identity).GetComponent<EnemyController>();
            spawnedEnemy.Init();
            currentWaveProperties.ReduceWeakEnemies();
        }
        else if (currentWaveProperties.StrongEnemies > 0)
        {
            spawnedEnemy = Instantiate(enemyStrongPrefab, this.transform.position, Quaternion.identity).GetComponent<EnemyController>();
            spawnedEnemy.Init();
            currentWaveProperties.ReduceStrongEnemies();
        }
        else
            return;

        EnemySpawned(spawnedEnemy);
    }
}

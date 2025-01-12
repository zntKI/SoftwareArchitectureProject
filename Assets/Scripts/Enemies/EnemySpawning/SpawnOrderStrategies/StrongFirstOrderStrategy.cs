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
        if (currentWaveProperties.StrongEnemies > 0)
        {
            Instantiate(enemyStrongPrefab, this.transform.position, Quaternion.identity);
            currentWaveProperties.ReduceStrongEnemies();
        }
        else if (currentWaveProperties.WeakEnemies > 0)
        {
            Instantiate(enemyWeakPrefab, this.transform.position, Quaternion.identity);
            currentWaveProperties.ReduceWeakEnemies();
        }
    }
}

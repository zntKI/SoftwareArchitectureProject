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
        if (currentWaveProperties.WeakEnemies > 0)
        {
            Instantiate(enemyWeakPrefab, this.transform.position, Quaternion.identity);
            currentWaveProperties.ReduceWeakEnemies();
        }
        else if (currentWaveProperties.StrongEnemies > 0)
        {
            Instantiate(enemyStrongPrefab, this.transform.position, Quaternion.identity);
            currentWaveProperties.ReduceStrongEnemies();
        }
    }
}

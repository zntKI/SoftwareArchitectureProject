using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Template class for all type of spawn order strategies (Weak/Strong enemies first)
/// </summary>
[RequireComponent(typeof(SpawnSpeedStrategy))]
public abstract class SpawnOrderStrategy : Strategy
{
    public static event Action<EnemyController> OnEnemySpawned;

    [SerializeField]
    protected GameObject enemyWeakPrefab;
    [SerializeField]
    protected GameObject enemyStrongPrefab;

    /// <summary>
    /// Holds information about the current wave
    /// </summary>
    protected WaveEnemiesNum currentWaveProperties;

    public void SetWaveProperties(WaveEnemiesNum waveProperties)
        => currentWaveProperties = waveProperties;

    /// <summary>
    /// Decides whether to spawn a strong or a weak enemy
    /// </summary>
    public abstract void PickEnemy();

    /// <summary>
    /// Called from subclasses when an Enemy gets spawned
    /// </summary>
    protected void EnemySpawned(EnemyController spawnedEnemy)
    {
        OnEnemySpawned?.Invoke(spawnedEnemy);
    }
}

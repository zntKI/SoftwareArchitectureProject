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
    [SerializeField]
    protected GameObject enemyWeakPrefab;
    [SerializeField]
    protected GameObject enemyStrongPrefab;

    protected WaveEnemiesNum currentWaveProperties;

    public void SetWaveProperties(WaveEnemiesNum waveProperties)
        => currentWaveProperties = waveProperties;
    public abstract void PickEnemy();
}

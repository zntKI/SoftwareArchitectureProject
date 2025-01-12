using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns Enemies in a linear fashion
/// </summary>
public class LinearSpawnSpeedStrategy : SpawnSpeedStrategy
{
    [SerializeField]
    private float timeBetweenEnemiesSec = 2f;

    void Start()
    {
        // Delete when actual game
        spawnIntervalCounter = timeBetweenEnemiesSec;
    }

    public override bool CountTime()
    {
        spawnIntervalCounter += Time.deltaTime;
        if (spawnIntervalCounter >= timeBetweenEnemiesSec)
        {
            spawnIntervalCounter = 0f;
            return true;
        }
        return false;
    }
}

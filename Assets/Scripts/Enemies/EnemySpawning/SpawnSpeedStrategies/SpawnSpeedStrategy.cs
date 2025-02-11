using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template class for all type of spawn speed strategies (Linear spawn speed, ...)
/// </summary>
public abstract class SpawnSpeedStrategy : Strategy
{
    protected float spawnIntervalCounter;

    /// <summary>
    /// Counts time between spawning of enemies
    /// </summary>
    /// <returns>If time to spawn new enemy</returns>
    public abstract bool CountTime();
}

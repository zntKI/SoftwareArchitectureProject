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

    public abstract bool CountTime();
}

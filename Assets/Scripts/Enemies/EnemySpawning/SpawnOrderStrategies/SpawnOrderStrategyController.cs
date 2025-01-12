using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for the change in spawn order strategies
/// </summary>
[RequireComponent(typeof(SpawnOrderStrategy))]
public class SpawnOrderStrategyController : StrategyController
{
    public SpawnOrderStrategy CurrentSpawnOrderStrategy
        => currentSpawnOrderStrategy;

    [SerializeField]
    private SpawnOrderStrategy currentSpawnOrderStrategy;

    void Start()
    {
        if (currentSpawnOrderStrategy == null) // If not specified in the inspector
        {
            currentSpawnOrderStrategy = GetComponents<SpawnOrderStrategy>().FirstOrDefault(w => w.enabled);
        }
    }

    /// <summary>
    /// Checks new strategy when enabled
    /// </summary>
    public override void EnableStrategy(Strategy newStrategy)
    {
        if (newStrategy is not SpawnOrderStrategy) // Due to the event being static
            return;

        var newSpawnOrderStrategy = (SpawnOrderStrategy)newStrategy;

        if (newSpawnOrderStrategy == currentSpawnOrderStrategy) // Avoid Unity start-up calls
            return;

        var previousStrategy = currentSpawnOrderStrategy;
        currentSpawnOrderStrategy = newSpawnOrderStrategy;

        // Check if change of strategies happened while spawning takes place
        StrategyEnabled();

        // Only for the inspector
        if (previousStrategy != null)
            previousStrategy.enabled = false;
    }

    public override void DisableStrategy(Strategy strategy)
    {
        if (strategy is SpawnOrderStrategy &&
            strategy == currentSpawnOrderStrategy) // Perform only if the right strategy
        {
            currentSpawnOrderStrategy = null;

            // Check if current spawning should be interupted
            StrategyDisabled();
        }
    }
}
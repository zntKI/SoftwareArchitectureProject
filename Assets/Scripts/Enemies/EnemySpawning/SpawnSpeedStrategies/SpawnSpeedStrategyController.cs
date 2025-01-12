using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for the change in spawn speed strategies
/// </summary>
[RequireComponent(typeof(SpawnSpeedStrategy))]
public class SpawnSpeedStrategyController : StrategyController
{
    public SpawnSpeedStrategy CurrentSpawnSpeedStrategy
        => currentSpawnSpeedStrategy;

    [SerializeField]
    private SpawnSpeedStrategy currentSpawnSpeedStrategy;

    void Start()
    {
        if (currentSpawnSpeedStrategy == null) // If not specified in the inspector
        {
            currentSpawnSpeedStrategy = GetComponents<SpawnSpeedStrategy>().FirstOrDefault(w => w.enabled);
        }
    }

    /// <summary>
    /// Checks new strategy when enabled
    /// </summary>
    public override void EnableStrategy(Strategy newStrategy)
    {
        if (newStrategy is not SpawnSpeedStrategy) // Due to the event being static
            return;

        var newSpeedOrderStrategy = (SpawnSpeedStrategy)newStrategy;

        if (newSpeedOrderStrategy == currentSpawnSpeedStrategy) // Avoid Unity start-up calls
            return;

        var previousStrategy = currentSpawnSpeedStrategy;
        currentSpawnSpeedStrategy = newSpeedOrderStrategy;

        // Check if change of strategies happened while spawning takes place
        StrategyEnabled();

        // Only for the inspector
        if (previousStrategy != null)
            previousStrategy.enabled = false;
    }

    public override void DisableStrategy(Strategy strategy)
    {
        if (strategy is SpawnSpeedStrategy &&
            strategy == currentSpawnSpeedStrategy /*Just to be sure*/)
        {
            currentSpawnSpeedStrategy = null;

            // Check if current spawning should be interupted
            StrategyDisabled();
        }
    }
}

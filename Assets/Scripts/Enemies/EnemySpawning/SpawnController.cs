using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information of the amount of enemies to be spawned in a given wave
/// </summary>
[Serializable]
public class WaveEnemiesNum
{
    public int WeakEnemies => weakEnemies;
    public int StrongEnemies => strongEnemies;

    [SerializeField]
    private int weakEnemies;
    [SerializeField]
    private int strongEnemies;

    public void ReduceWeakEnemies()
    {
        weakEnemies--;
        if (weakEnemies < 0)
            weakEnemies = 0;
    }

    public void ReduceStrongEnemies()
    {
        strongEnemies--;
        if (strongEnemies < 0)
            strongEnemies = 0;
    }

    public bool IsFinished()
        => weakEnemies == 0 && strongEnemies == 0;
}

/// <summary>
/// State of spawning
/// Not a full-blown FSM pattern thing since it may be an overhead
/// </summary>
public enum SpawnState
{
    NoSpawning,
    Spawning,
    NoStrategySelected,
}

/// <summary>
/// Does the heavy-lifting of spawning:<br></br>
/// Distributes spawning tasks to the corresponding objects<br></br>
/// Communicates with the strategy controllers to retrieve current strategy<br></br>
/// Switches spawning logic according to the strategy in place allowing mid-spawning modifications and combinations
/// </summary>
[RequireComponent(typeof(SpawnOrderStrategyController))]
[RequireComponent(typeof(SpawnSpeedStrategyController))]
public class SpawnController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If you change amount of wave enemies num, also change the amount of waves in 'EnemiesSpawner'")]
    private WaveEnemiesNum[] allWavesProperties;
    private WaveEnemiesNum currentWaveProperties;

    private SpawnOrderStrategyController spawnOrderStrategyController;
    private SpawnSpeedStrategyController spawnSpeedStrategyController;

    private SpawnOrderStrategy currentSpawnOrderStrategy;
    private SpawnSpeedStrategy currentSpawnSpeedStrategy;

    private SpawnState spawnState;

    // Start is called before the first frame update
    void Start()
    {
        spawnOrderStrategyController = GetComponent<SpawnOrderStrategyController>();
        spawnSpeedStrategyController = GetComponent<SpawnSpeedStrategyController>();

        spawnOrderStrategyController.OnStrategyEnabled += CheckChangingOrderStrategy;
        spawnSpeedStrategyController.OnStrategyEnabled += CheckChangingSpeedStrategy;

        spawnOrderStrategyController.OnStrategyDisabled += CheckDisabledOrderStrategy;
        spawnSpeedStrategyController.OnStrategyDisabled += CheckDisabledSpeedStrategy;

        // Retrieves them from the Start so that when SetupSpawning is called it does not do an early return
        currentSpawnOrderStrategy = spawnOrderStrategyController.CurrentSpawnOrderStrategy;
        currentSpawnSpeedStrategy = spawnSpeedStrategyController.CurrentSpawnSpeedStrategy;
    }

    void SwitchState(SpawnState newState)
    {
        spawnState = newState;
    }

    // State controller
    void Update()
    {
        switch (spawnState)
        {
            case SpawnState.NoSpawning: // Do nothing...
                break;
            case SpawnState.Spawning:
                DoSpawning();
                break;
            case SpawnState.NoStrategySelected:
                LogWarnings();
                break;
            default:
                break;
        }
    }

    void DoSpawning()
    {
        if (currentSpawnSpeedStrategy.CountTime())
        {
            currentSpawnOrderStrategy.PickEnemy();
            // currentWaveProperties updated through all who have a reference to it
            if (currentWaveProperties.IsFinished())
            {
                SwitchState(SpawnState.NoSpawning);
            }
        }
    }

    /// <summary>
    /// Acts as a warning to users (Designers) when they try out different design combinations
    /// </summary>
    void LogWarnings()
    {
        if (currentSpawnOrderStrategy == null)
        {
            Debug.Log("All SpawnOrder strategies are disabled! Enable one to continue spawning.");
        }
        if (currentSpawnSpeedStrategy == null)
        {
            Debug.Log("All SpawnSpeed strategies are disabled! Enable one to continue spawning.");
        }
        if (currentSpawnOrderStrategy != null && currentSpawnSpeedStrategy != null)
        {
            SwitchState(SpawnState.Spawning);
        }
    }

    /// <summary>
    /// Called when a new wave should start spawning
    /// </summary>
    public void SetupSpawning(WaveNum wave)
    {
        currentWaveProperties = allWavesProperties[(int)wave];

        if (currentSpawnOrderStrategy == null ||
            currentSpawnSpeedStrategy == null)
        {
            SwitchState(SpawnState.NoStrategySelected);
            return;
        }

        currentSpawnOrderStrategy = spawnOrderStrategyController.CurrentSpawnOrderStrategy;
        currentSpawnSpeedStrategy = spawnSpeedStrategyController.CurrentSpawnSpeedStrategy;

        currentSpawnOrderStrategy.SetWaveProperties(currentWaveProperties);

        SwitchState(SpawnState.Spawning);
    }

    /// <summary>
    /// Enables changing spawning strategies while spawning is currently happening<br></br>
    /// so that the new strategy picks up from where the old one left off
    /// </summary>
    void CheckChangingOrderStrategy()
    {
        currentSpawnOrderStrategy = spawnOrderStrategyController.CurrentSpawnOrderStrategy;
        currentSpawnOrderStrategy.SetWaveProperties(currentWaveProperties);
    }

    /// <summary>
    /// Enables changing spawning strategies while spawning is currently happening<br></br>
    /// so that the new strategy picks up from where the old one left off
    /// </summary>
    void CheckChangingSpeedStrategy()
    {
        currentSpawnSpeedStrategy = spawnSpeedStrategyController.CurrentSpawnSpeedStrategy;
    }

    /// <summary>
    /// Called only when the user disables it from the inspector and there is no active strategy after that
    /// For when swaping strategies, it is handled by the strategy enabling logic
    /// </summary>
    void CheckDisabledOrderStrategy()
    {
        currentSpawnOrderStrategy = null;
        CheckStateAfterDisabledStrategy();
    }

    /// <summary>
    /// Called only when the user disables it from the inspector and there is no active strategy after that
    /// For when swaping strategies, it is handled by the strategy enabling logic
    /// </summary>
    void CheckDisabledSpeedStrategy()
    {
        currentSpawnSpeedStrategy = null;
        CheckStateAfterDisabledStrategy();
    }

    void CheckStateAfterDisabledStrategy()
    {
        if (spawnState == SpawnState.Spawning)
        {
            SwitchState(SpawnState.NoStrategySelected);
        }
    }

    void OnDestroy()
    {
        spawnOrderStrategyController.OnStrategyEnabled -= CheckChangingOrderStrategy;
        spawnSpeedStrategyController.OnStrategyEnabled -= CheckChangingSpeedStrategy;

        spawnOrderStrategyController.OnStrategyDisabled -= CheckDisabledOrderStrategy;
        spawnSpeedStrategyController.OnStrategyDisabled -= CheckDisabledSpeedStrategy;
    }
}
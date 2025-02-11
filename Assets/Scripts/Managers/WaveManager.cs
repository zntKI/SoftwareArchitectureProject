using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The amount of spawn waves
/// </summary>
public enum WaveNum
{
    Zero,
    First,
    Second,
    Third,
    Fourth,
    Fifth
}

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

    /// <summary>
    /// Checks if there are no more enemies to spawn
    /// </summary>
    public bool IsFinished()
        => weakEnemies == 0 && strongEnemies == 0;
}

/// <summary>
/// Wave Singleton manager, responsible for keeping track of enemies killed and passed as well as communicating with other managers
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static event Action<WaveEnemiesNum> OnSetupSpawning;

    /// <summary>
    /// <result, isLastWave>
    /// </summary>
    public static event Action<bool, bool> OnWaveOver;

    public static event Action<GameState> OnWaveStartUI;
    /// <summary>
    /// Enemies left before game over
    /// <currentNumEnemiesPassed, enemyNumsPerWave[currentWave - 1]>
    /// </summary>
    public static event Action<int, int> OnUpdateEnemiesAmount;
    public static event Action<int, int> OnUpdateWavesAmount;

    public static WaveManager Instance => instance;
    static WaveManager instance;

    [SerializeField]
    [Tooltip("If you change amount of waves, also change the amount of wave enemies num")]
    private WaveNum currentWave = WaveNum.Zero;
    [SerializeField]
    [Tooltip("If you change amount of wave enemies num, also change the amount of waves")]
    private WaveEnemiesNum[] allWavesProperties;

    /// <summary>
    /// How much enemies can pass before GameOver, for each wave
    /// </summary>
    [SerializeField]
    private int[] enemyNumsPerWave;
    private int currentNumEnemiesPassed;

    private int totalAmountOfEnemiesForCurrentWave;
    private int currentEnemiesKilled;

    public static IReadOnlyCollection<EnemyController> SpawnedEnemies => instance.spawnedEnemies;
    private List<EnemyController> spawnedEnemies;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new InvalidOperationException("There can only be only one WaveManager in the scene!");
        }

        GameManager.OnBeginWave += StartSpawning;

        SpawnOrderStrategy.OnEnemySpawned += AddEnemy;
        EnemyController.OnReachedEnd += RemoveAndIncreaseEnemyPassed;

        EnemyController.OnDied += IncreaseAmountEnemiesKilled;
    }

    void Start()
    {
        spawnedEnemies = new List<EnemyController>();

        if (!GameManager.IsBuildFirst)
        {
            StartSpawning();
        }
    }

    void StartSpawning()
    {
        instance.currentWave = (WaveNum)((int)instance.currentWave + 1); // Increment current wave

        // Setup spawning
        WaveEnemiesNum currentWaveProperties = instance.allWavesProperties[(int)instance.currentWave - 1];

        currentNumEnemiesPassed = 0;

        totalAmountOfEnemiesForCurrentWave = currentWaveProperties.WeakEnemies + currentWaveProperties.StrongEnemies;
        currentEnemiesKilled = 0;

        // UI
        OnWaveStartUI?.Invoke(GameState.Wave); // Hide Building UI and show Wave UI
        OnUpdateWavesAmount?.Invoke((int)instance.currentWave, allWavesProperties.Length); // Update Waves text
        OnUpdateEnemiesAmount?.Invoke(currentNumEnemiesPassed, instance.enemyNumsPerWave[(int)instance.currentWave - 1]); // Update Enemies' amount

        OnSetupSpawning?.Invoke(currentWaveProperties); // Tell 'SpawnController' to start spawning
    }

    /// <summary>
    /// Adds new Enemy to collection when spawned
    /// </summary>
    void AddEnemy(EnemyController newEnemy)
    {
        spawnedEnemies.Add(newEnemy);
    }

    /// <summary>
    /// Removes Enemy from collection
    /// </summary>
    void RemoveEnemy(EnemyController enemyToRemove)
    {
        spawnedEnemies.Remove(enemyToRemove);
    }

    /// <summary>
    /// Registers Event on Enemy death and checks if wave is survived
    /// </summary>
    void IncreaseAmountEnemiesKilled(EnemyController enemyToRemove)
    {
        RemoveEnemy(enemyToRemove);

        currentEnemiesKilled++;
        if (currentEnemiesKilled >= totalAmountOfEnemiesForCurrentWave)
        {
            OnWaveOver?.Invoke(true, (int)instance.currentWave == instance.allWavesProperties.Length);
        }
    }

    /// <summary>
    /// Registers Event on Enemy reached end and check if wave survived or not
    /// </summary>
    void RemoveAndIncreaseEnemyPassed(EnemyController enemyToRemove)
    {
        RemoveEnemy(enemyToRemove);

        currentNumEnemiesPassed++;
        if (currentNumEnemiesPassed >= instance.enemyNumsPerWave[(int)instance.currentWave - 1])
        {
            OnWaveOver?.Invoke(false, (int)instance.currentWave == instance.allWavesProperties.Length);
        }
        else if (spawnedEnemies.Count == 0)
        {
            OnWaveOver?.Invoke(true, (int)instance.currentWave == instance.allWavesProperties.Length);
        }

        OnUpdateEnemiesAmount?.Invoke(currentNumEnemiesPassed, instance.enemyNumsPerWave[(int)instance.currentWave - 1]);
    }

    void OnDestroy()
    {
        GameManager.OnBeginWave -= StartSpawning;

        SpawnOrderStrategy.OnEnemySpawned -= AddEnemy;
        EnemyController.OnReachedEnd -= RemoveAndIncreaseEnemyPassed;

        EnemyController.OnDied -= IncreaseAmountEnemiesKilled;
    }
}
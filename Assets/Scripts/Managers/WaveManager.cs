using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If you change amount of waves, also change the amount of wave enemies num in SpawnController
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

    public bool IsFinished()
        => weakEnemies == 0 && strongEnemies == 0;
}

public class WaveManager : MonoBehaviour
{
    public static event Action<WaveEnemiesNum> OnSetupSpawning;

    public static event Action<bool> OnGameOver;

    // Enemies left before game over
    // <currentNumEnemiesPassed, enemyNumsPerWave[currentWave - 1]>
    public static event Action<int, int> OnUpdateEnemiesAmount;

    public static WaveManager Instance => instance;
    static WaveManager instance;

    [SerializeField]
    [Tooltip("If you change amount of waves, also change the amount of wave enemies num in 'SpawnController'")]
    private WaveNum currentWave = WaveNum.Zero;
    [SerializeField]
    [Tooltip("If you change amount of wave enemies num, also change the amount of waves in 'EnemiesSpawner'")]
    private WaveEnemiesNum[] allWavesProperties;

    [SerializeField]
    private int[] enemyNumsPerWave;
    private int currentNumEnemiesPassed;

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

        EnemyController.OnDied += RemoveEnemy;
    }

    void Start()
    {
        spawnedEnemies = new List<EnemyController>();
    }

    void StartSpawning()
    {
        instance.currentWave = (WaveNum)((int)instance.currentWave + 1);

        OnUpdateEnemiesAmount?.Invoke(currentNumEnemiesPassed, instance.enemyNumsPerWave[(int)instance.currentWave - 1]);

        OnSetupSpawning?.Invoke(instance.allWavesProperties[(int)instance.currentWave - 1]);
    }

    void AddEnemy(EnemyController newEnemy)
    {
        spawnedEnemies.Add(newEnemy);
    }

    void RemoveEnemy(EnemyController enemyToRemove)
    {
        spawnedEnemies.Remove(enemyToRemove);
    }

    void RemoveAndIncreaseEnemyPassed(EnemyController enemyToRemove)
    {
        RemoveEnemy(enemyToRemove);

        currentNumEnemiesPassed++;
        if (currentNumEnemiesPassed > instance.enemyNumsPerWave[(int)instance.currentWave - 1])
        {
            OnGameOver?.Invoke(false);
        }

        OnUpdateEnemiesAmount?.Invoke(currentNumEnemiesPassed, instance.enemyNumsPerWave[(int)instance.currentWave - 1]);
    }

    void OnDestroy()
    {
        GameManager.OnBeginWave -= StartSpawning;

        SpawnOrderStrategy.OnEnemySpawned -= AddEnemy;
        EnemyController.OnReachedEnd -= RemoveAndIncreaseEnemyPassed;

        EnemyController.OnDied -= RemoveEnemy;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If you change amount of waves, also change the amount of wave enemies num in SpawnController
public enum WaveNum
{
    First,
    Second,
    Third,
    Fourth,
    Fifth
}

/// <summary>
/// The middleman who accepts notifications from GameManager, who says when
/// to start spawning a new wave, and the SpawnController - responsible for the actual spawning
/// </summary>
[RequireComponent(typeof(SpawnController))]
public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If you change amount of waves, also change the amount of wave enemies num in 'SpawnController'")]
    private WaveNum currentWave;

    private SpawnController spawnController;

    void Start()
    {
        spawnController = GetComponent<SpawnController>();

        // Done in Start for only for testing
        // TODO: In actual game - GameManager fires an event telling which wave to start spawning
        spawnController.SetupSpawning(currentWave);
    }
}
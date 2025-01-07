using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private float spawnIntervalSec;
    private float spawnIntervalCounter = 0f;

    void Update()
    {
        spawnIntervalCounter += Time.deltaTime;
        if (spawnIntervalCounter >= spawnIntervalSec)
        {
            // Spawn enemy
            Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);

            spawnIntervalCounter = 0f;
        }
    }
}

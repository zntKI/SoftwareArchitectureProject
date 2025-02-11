using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that targets only one enemy at a time - it registers it when it gets in range and targets it until it is out of range again.
/// </summary>
[RequireComponent(typeof(SelectionRange))]
public class SingleTargetSelector : TargetSelector
{
    public override List<EnemyController> SelectTarget()
    {
        // Selection
        foreach (var enemy in WaveManager.SpawnedEnemies)
        {
            float distance = (enemy.transform.position - this.transform.position).magnitude;
            if (distance <= selectionRange.Value && selectedTargets.Count == 0) // If distance less than selection range and not in collection - add
            {
                selectedTargets.Insert(0, enemy);
                //Debug.Log("Target in range!");
            }
            else if (selectedTargets.Count != 0 &&
                selectedTargets[0] == enemy && distance > selectionRange.Value) // If distance more than selection range and in collection - remove
            {
                selectedTargets.Clear();
                //Debug.Log("Target out of range!");
            }
        }

        return selectedTargets;
    }
}
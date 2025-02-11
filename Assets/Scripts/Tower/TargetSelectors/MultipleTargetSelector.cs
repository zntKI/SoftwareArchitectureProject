using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tower that targets multiple enemies at once - enemies are added to a collection upon their entrance within the attack range and then removed from the collection when out of range.
/// </summary>
[RequireComponent(typeof(SelectionRange))]
public class MultipleTargetSelector : TargetSelector
{
    public override List<EnemyController> SelectTarget()
    {
        // Selection
        foreach (var enemy in WaveManager.SpawnedEnemies)
        {
            float distance = (enemy.transform.position - this.transform.position).magnitude;
            if (distance <= selectionRange.Value && !selectedTargets.Contains(enemy)) // If distance less than selection range and not in collection - add
            {
                selectedTargets.Add(enemy);
                //Debug.Log("Target in range!" + selectedTargets.Count);
            }
            else if (selectedTargets.Contains(enemy) && distance > selectionRange.Value) // If distance more than selection range and in collection - remove
            {
                selectedTargets.Remove(enemy);
                enemy.OnTargetZoneLeave();
                //Debug.Log("Target out of range!" + selectedTargets.Count);
            }
        }

        return selectedTargets;
    }
}
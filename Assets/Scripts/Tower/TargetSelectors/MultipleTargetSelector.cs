using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectionRange))]
public class MultipleTargetSelector : TargetSelector
{
    private SelectionRange selectionRange;

    private void Start()
    {
        selectionRange = GetComponent<SelectionRange>();

        selectedTargets = new List<EnemyController>();
    }

    private void Update()
    {
    }

    public override List<EnemyController> SelectTarget()
    {
        // Selection
        foreach (var enemy in WaveManager.SpawnedEnemies)
        {
            float distance = (enemy.transform.position - this.transform.position).magnitude;
            if (distance <= selectionRange.Value && !selectedTargets.Contains(enemy))
            {
                selectedTargets.Add(enemy);
                //Debug.Log("Target in range!" + selectedTargets.Count);
            }
            else if (selectedTargets.Contains(enemy) && distance > selectionRange.Value)
            {
                selectedTargets.Remove(enemy);
                enemy.OnTargetZoneLeave();
                //Debug.Log("Target out of range!" + selectedTargets.Count);
            }
        }

        return selectedTargets;
    }
}
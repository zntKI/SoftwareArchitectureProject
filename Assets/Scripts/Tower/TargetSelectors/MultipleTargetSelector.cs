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
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            float distance = (enemy.transform.position - this.transform.position).magnitude;
            if (distance <= selectionRange.Range && !selectedTargets.Contains(enemy))
            {
                selectedTargets.Add(enemy);
                //Debug.Log("Target in range!" + selectedTargets.Count);
            }
            else if (selectedTargets.Contains(enemy) && distance > selectionRange.Range)
            {
                selectedTargets.Remove(enemy);
                enemy.OnTargetZoneLeave();
                //Debug.Log("Target out of range!" + selectedTargets.Count);
            }
        }

        return selectedTargets;
    }
}
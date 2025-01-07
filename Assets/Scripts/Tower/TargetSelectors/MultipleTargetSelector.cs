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

        selectedTargets = new List<GameObject>();
    }

    private void Update()
    {
        SelectTarget();
    }

    public override List<GameObject> SelectTarget()
    {
        // Selection
        var enemies = GameObject.FindGameObjectsWithTag("Target");
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
                //Debug.Log("Target out of range!" + selectedTargets.Count);
            }
        }

        return selectedTargets;
    }
}
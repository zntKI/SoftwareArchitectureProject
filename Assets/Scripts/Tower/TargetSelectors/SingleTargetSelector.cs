using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectionRange))]
public class SingleTargetSelector : TargetSelector
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
        // TODO: Think of smth better - maybe a game manager holds the enemies and you request when needed
        var enemies = GameObject.FindGameObjectsWithTag("Target");
        foreach (var enemy in enemies)
        {
            float distance = (enemy.transform.position - this.transform.position).magnitude;
            if (distance <= selectionRange.Range && selectedTargets.Count == 0)
            {
                selectedTargets.Insert(0, enemy);
                //Debug.Log("Target in range!");
            }
            else if (selectedTargets.Count != 0 &&
                selectedTargets[0] == enemy && distance > selectionRange.Range)
            {
                selectedTargets.Clear();
                //Debug.Log("Target out of range!");
            }
        }

        return selectedTargets;
    }
}
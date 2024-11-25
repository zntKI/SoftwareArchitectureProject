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
    }

    private void Update()
    {
        SelectTarget();
    }

    public override List<ITarget> SelectTarget()
    {
        // TODO: Do the selection
        return new List<ITarget>();
    }
}
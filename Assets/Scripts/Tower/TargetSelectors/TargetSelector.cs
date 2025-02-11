using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract behaviour that acts as a blueprint for more specific target selectors
/// </summary>
public abstract class TargetSelector : MonoBehaviour
{
    protected IPropertyReadOnlyValue<float> selectionRange;

    protected List<EnemyController> selectedTargets;

    void Awake()
    {
        EnemyController.OnDied += CheckIfShouldRemoveFromCollection;
    }

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public virtual void Init()
    {
        selectionRange = GetComponent<SelectionRange>();

        selectedTargets = new List<EnemyController>();
    }

    /// <summary>
    /// Called on Enemy death to check if should remove it from collections
    /// </summary>
    protected virtual void CheckIfShouldRemoveFromCollection(EnemyController enemy)
    {
        if (selectedTargets.Contains(enemy))
        {
            selectedTargets.Remove(enemy);
        }
    }

    /// <summary>
    /// Main method that does the selection
    /// </summary>
    /// <returns>Selected targets</returns>
    public abstract List<EnemyController> SelectTarget();

    void OnDestroy() 
    {
        EnemyController.OnDied -= CheckIfShouldRemoveFromCollection;
    }
}
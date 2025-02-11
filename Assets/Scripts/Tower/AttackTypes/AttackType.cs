using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract behaviour that acts as a blueprint for more specific attack types
/// </summary>
public abstract class AttackType : MonoBehaviour
{
    /// <summary>
    /// Called before Start, when TowerController's Init gets called
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Registers targets to attack
    /// </summary>
    public abstract void SetUp(List<EnemyController> targets);

    /// <summary>
    /// Deregisters targets to attack if needed
    /// </summary>
    public abstract void SetDown();
}
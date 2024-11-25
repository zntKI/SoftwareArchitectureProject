using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All Targets need to implement this interface
/// </summary>
public interface ITarget
{
    Transform GetTransform();
}

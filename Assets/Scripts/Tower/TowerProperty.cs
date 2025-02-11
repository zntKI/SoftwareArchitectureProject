using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface which all tower properties implement with value according to their needs
/// </summary>
public interface IPropertyReadOnlyValue<T>
{
    T Value { get; }
}

/// <summary>
/// Add-on to 'IPropertyReadOnlyValue' enabling Value modification
/// </summary>
public interface IPropertyModifiableValue<T>
{
    /// <summary>
    /// Adds or subtracts from the original value depending on whether the parameter is with a '+' or '-'
    /// </summary>
    public void ModifyValue(T value);
}
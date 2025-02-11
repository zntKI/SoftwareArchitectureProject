using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private float initialWidth;

    void Start()
    {
        initialWidth = transform.localScale.x;
    }

    /// <summary>
    /// Updates the health bar's scale
    /// </summary>
    public void UpdateHealthBar(float proportion)
    {
        transform.localScale = new Vector3(proportion * initialWidth, transform.localScale.y, transform.localScale.z);
    }
}

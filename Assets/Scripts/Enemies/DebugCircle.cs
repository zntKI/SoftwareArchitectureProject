using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draws a debug circle around an object making it more intuitive and easy to work with
/// Also used mid-game for its radius
/// </summary>
public class DebugCircle : MonoBehaviour
{
    public float Radius => radius;

    Vector3 center;


    [SerializeField]
    bool shouldDraw = false;

    [Header("Circle")]
    [SerializeField]
    float radius = 5f; // Radius of the circle
    [SerializeField]
    Color color = Color.red; // Color of the circle

    [Header("All")]
    [SerializeField]
    int segments = 50; // Number of segments to create the circle
    float duration = 0.0f; // Duration for which the circle is visible

    void OnDrawGizmos()
    {
        if (shouldDraw)
        {
            center = (Vector2)this.transform.position; // Cast to Vector2 for 2D context
            DrawCircle(center, radius, segments, color, duration);
        }
    }

    void DrawCircle(Vector2 center, float radius, int segments, Color color, float duration)
    {
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector2 prevPoint = center + new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        angle += angleStep;

        for (int i = 1; i <= segments; i++)
        {
            Vector2 nextPoint = center + new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
            angle += angleStep;
        }
    }
}

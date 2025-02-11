using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A movement behaviour that uses waypoints to navigate Enemies' movement
/// </summary>
public class WaypointMovementBehaviour : MovementBehaviour
{
    private EnemyModel model;
    private float speed;

    private Queue<GameObject> waypoints;

    /// <summary>
    /// Waypoint game object, its detection radius that is used to check if enemy has reached it
    /// </summary>
    private Tuple<GameObject, DebugCircle> currentTargetWaypoint;

    void Start()
    {
        model = GetComponent<EnemyModel>();
        speed = model.Speed;

        SetupCollections();
        currentTargetWaypoint = new Tuple<GameObject, DebugCircle>(null, null);
    }

    void SetupCollections()
    {
        waypoints = new Queue<GameObject>();

        // Retrieve waypoints ordered by ID
        var tempWaypoints = FindObjectsByType<IdHolder>(FindObjectsSortMode.None).OrderBy(w => w.Id);
        foreach (var waypoint in tempWaypoints)
        {
            waypoints.Enqueue(waypoint.gameObject);
        }
    }

    void Update()
    {
        speed = model.Speed;
    }

    public override void DoMoving()
    {
        // Pick next waypoint
        if (!PickWaypoint())
            return;

        // Do the actual moving
        Move();

        // Delete current waypoint if reached
        CheckIfWaypointReached();
    }

    /// <returns>If there is a waypoint to be picked</returns>
    bool PickWaypoint()
    {
        if (currentTargetWaypoint.Item1 == null)
        {
            if (HasReachedEnd())
                return false;

            var newWaypoint = waypoints.Peek();
            currentTargetWaypoint = new Tuple<GameObject, DebugCircle>(waypoints.Dequeue(), newWaypoint.GetComponent<DebugCircle>());
        }
        return true;
    }

    /// <summary>
    /// Check if there are more waypoints to visit
    /// </summary>
    bool HasReachedEnd()
    {
        if (waypoints.Count == 0) // There is no more waypoints to visit
        {
            ReachedEnd();
            return true;
        }
        return false;
    }

    void Move()
    {
        var amountToMove = (currentTargetWaypoint.Item1.transform.position - this.transform.position).normalized * speed * Time.deltaTime;
        transform.Translate(amountToMove);
    }

    /// <summary>
    /// Checks if Enemy's positions is smaller than the waypoint's radius
    /// </summary>
    void CheckIfWaypointReached()
    {
        float distance = (currentTargetWaypoint.Item1.transform.position - this.transform.position).magnitude;
        if (distance < currentTargetWaypoint.Item2.Radius)
        {
            currentTargetWaypoint = new Tuple<GameObject, DebugCircle>(null, null);
        }
    }
}
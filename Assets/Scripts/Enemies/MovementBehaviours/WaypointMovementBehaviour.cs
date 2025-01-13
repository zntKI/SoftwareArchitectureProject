using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoveSpeed))]
public class WaypointMovementBehaviour : MovementBehaviour
{
    private MoveSpeed moveSpeed;

    private Queue<GameObject> waypoints;

    private Tuple<GameObject, DebugCircle> currentTargetWaypoint;

    void Start()
    {
        moveSpeed = GetComponent<MoveSpeed>();

        SetupCollections();
        currentTargetWaypoint = new Tuple<GameObject, DebugCircle>(null, null);
    }

    void SetupCollections()
    {
        waypoints = new Queue<GameObject>();

        // TODO: Think of smth better - maybe a game manager holds the waypoints and you request when needed
        var tempWaypoints = FindObjectsByType<IdHolder>(FindObjectsSortMode.None).OrderBy(w => w.Id);
        foreach (var waypoint in tempWaypoints)
        {
            waypoints.Enqueue(waypoint.gameObject);
        }
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
        var amountToMove = (currentTargetWaypoint.Item1.transform.position - this.transform.position).normalized * moveSpeed.MoveSpeeD * Time.deltaTime;
        transform.Translate(amountToMove);
    }

    void CheckIfWaypointReached()
    {
        float distance = (currentTargetWaypoint.Item1.transform.position - this.transform.position).magnitude;
        if (distance < currentTargetWaypoint.Item2.Radius)
        {
            currentTargetWaypoint = new Tuple<GameObject, DebugCircle>(null, null);
        }
    }
}
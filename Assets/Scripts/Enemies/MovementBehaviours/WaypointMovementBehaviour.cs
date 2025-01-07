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

    private GameObject currentTargetWaypoint;
    private DebugCircle currentTargetWaypointCircle;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = GetComponent<MoveSpeed>();

        SetupCollections();
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
        if (currentTargetWaypoint == null)
        {
            if (!HasReachedEnd())
            {
                currentTargetWaypoint = waypoints.Dequeue();
                currentTargetWaypointCircle = currentTargetWaypoint.GetComponent<DebugCircle>();

                //Debug.Log("Waypoint picked!");
            }
            else
                return false;
        }
        return true;
    }

    bool HasReachedEnd()
    {
        if (waypoints.Count == 0) // There is no more waypoints to visit
        {
            // TODO: move it elsewhere - single responsibility
            // Remove enemy
            Destroy(gameObject);
            //Debug.Log("Reached end - Enemy destroyed!");

            return true;
        }
        return false;
    }

    void Move()
    {
        var amountToMove = (currentTargetWaypoint.transform.position - this.transform.position).normalized * moveSpeed.MoveSpeeD * Time.deltaTime;
        transform.Translate(amountToMove);
    }

    void CheckIfWaypointReached()
    {
        float distance = (currentTargetWaypoint.transform.position - this.transform.position).magnitude;
        if (distance < currentTargetWaypointCircle.Radius)
        {
            currentTargetWaypoint = null;
            currentTargetWaypointCircle = null;
            //Debug.Log("Waypoint reached!");
        }
    }
}
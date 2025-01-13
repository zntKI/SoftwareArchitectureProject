using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MovementBehaviour))]
public class EnemyController : MonoBehaviour
{
    private MovementBehaviour movementBehaviour;

    void Awake()
    {
        MovementBehaviour.OnReachedEnd += ReachedEnd;
    }

    // Start is called before the first frame update
    void Start()
    {
        movementBehaviour = GetComponent<MovementBehaviour>();
    }

    // TODO: Do this in a differen class that abstracts the movement logic
    void Update()
    {
        movementBehaviour.DoMoving();
    }

    void ReachedEnd(GameObject enemy)
    {
        if (enemy == gameObject)
        {
            Destroy(gameObject);
            // Also fire an event to increase the counter of enemies passed?
        }
    }

    void OnDestroy()
    {
        MovementBehaviour.OnReachedEnd -= ReachedEnd;
    }
}

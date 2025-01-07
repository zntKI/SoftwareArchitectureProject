using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MovementBehaviour))]
public class EnemyController : MonoBehaviour
{
    private MovementBehaviour movementBehaviour;

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
}

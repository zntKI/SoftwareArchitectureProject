using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    private GameObject towerToTarget;

    // Start is called before the first frame update
    void Start()
    {
        towerToTarget = GameObject.FindWithTag("Tower");
    }

    // Update is called once per frame
    void Update()
    {
        var amountToMove = (towerToTarget.transform.position - this.transform.position).normalized * moveSpeed * Time.deltaTime;
        transform.Translate(amountToMove);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDropParticleController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.5f;
    [SerializeField]
    private float lifetime = 1.0f;

    private float timeCounter = 0f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timeCounter += Time.deltaTime;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(1f, 0f, timeCounter / lifetime));
    }
}
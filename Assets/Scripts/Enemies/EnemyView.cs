using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField]
    private Sprite fullHealthSprite;
    [SerializeField]
    private Sprite midHealthSprite;
    [SerializeField]
    private Sprite lowHealthSprite;

    [SerializeField]
    private GameObject moneyParticlePrefab;
    [SerializeField]
    private float moneyParticleSpawnRadius = 1f;

    private float healthAmountForFirstSpriteTransition;
    private float healthAmountForSecondSpriteTransition;

    private SpriteRenderer spriteRenderer;

    private HealthBarController healthBar;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = fullHealthSprite;

        EnemyModel model = GetComponent<EnemyModel>();
        healthAmountForFirstSpriteTransition = model.InitialHealth * 0.66f;
        healthAmountForSecondSpriteTransition = model.InitialHealth * 0.33f;

        healthBar = transform.GetComponentInChildren<HealthBarController>();
    }

    public void CheckHealth(float newHealth)
    {
        if (newHealth <= healthAmountForSecondSpriteTransition)
        {
            spriteRenderer.sprite = lowHealthSprite;
        }
        else if (newHealth <= healthAmountForFirstSpriteTransition)
        {
            spriteRenderer.sprite = midHealthSprite;
        }

        // Update health bar
        //healthBar.UpdateHealthBar();
    }

    public void SpawnMoneyParticle(int moneyAmount)
    {
        Vector2 rndPos = Vector2.zero;
        for (int i = 0; i < moneyAmount; i++)
        {
            rndPos = Random.insideUnitCircle;
            rndPos *= moneyParticleSpawnRadius;

            Vector3 positionToSpawn = transform.position + new Vector3(rndPos.x, rndPos.y);

            MoneyDropParticleController moneyParticle = Instantiate(moneyParticlePrefab, positionToSpawn, Quaternion.identity).GetComponent<MoneyDropParticleController>();
            moneyParticle.Init();
        }
    }
}

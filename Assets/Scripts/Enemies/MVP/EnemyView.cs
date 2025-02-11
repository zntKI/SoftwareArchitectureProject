using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the MVP pattern for Enemy<br></br>
/// Handles all visuals regarding Enemy
/// </summary>
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
    private Color originalColor;

    private HealthBarController healthBar;

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = fullHealthSprite;

        originalColor = spriteRenderer.color;

        EnemyModel model = GetComponent<EnemyModel>();
        healthAmountForFirstSpriteTransition = model.InitialHealth * 0.66f;
        healthAmountForSecondSpriteTransition = model.InitialHealth * 0.33f;

        healthBar = transform.GetComponentInChildren<HealthBarController>();
    }

    /// <summary>
    /// Checks if necessary to change Enemy sprite
    /// </summary>
    public void CheckHealth(float newHealth, float initialHealth)
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
        healthBar.UpdateHealthBar(newHealth / initialHealth);
    }

    /// <summary>
    /// Sets the color overlay of the sprite renderer due to being slowed down
    /// </summary>
    public void UpdateColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    /// <summary>
    /// Sets back the color overlay of the sprite renderer to its initial value
    /// </summary>
    public void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// On Enemy death, spawn particle to make clear how much money the Player gained
    /// </summary>
    public void SpawnMoneyParticle(int moneyAmount)
    {
        Vector2 rndPos = Vector2.zero;
        for (int i = 0; i < moneyAmount; i++) // Get a random position within a given range for each particle to spawn at
        {
            rndPos = Random.insideUnitCircle;
            rndPos *= moneyParticleSpawnRadius;

            Vector3 positionToSpawn = transform.position + new Vector3(rndPos.x, rndPos.y);

            MoneyDropParticleController moneyParticle = Instantiate(moneyParticlePrefab, positionToSpawn, Quaternion.identity).GetComponent<MoneyDropParticleController>();
            moneyParticle.Init();
        }
    }
}

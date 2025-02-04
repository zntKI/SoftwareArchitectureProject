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
}

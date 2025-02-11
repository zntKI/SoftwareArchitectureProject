using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the MVP pattern for Tower<br></br>
/// Handles all visuals regarding Tower
/// </summary>
public class TowerView : MonoBehaviour
{
    /// <summary>
    /// Tower Sprites indicating each level of Tower
    /// </summary>
    [SerializeField]
    private Sprite[] diffLevelSprites;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex;

    /// <summary>
    /// Sprite Renderer of Child of the tower that is made visible when tower selected for upgrading or selling<br></br>
    /// to make it clear which tower is currently selected
    /// </summary>
    private SpriteRenderer childSRVisibleWhileUpgrading;

    /// <summary>
    /// Need to be called before Start
    /// </summary>
    public void Init()
    {
        InitSprite();

        childSRVisibleWhileUpgrading = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Helper method to setup sprite logic
    /// </summary>
    void InitSprite()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite currSprite = spriteRenderer.sprite;

        currentSpriteIndex = -1;
        for (int i = 0; i < diffLevelSprites.Length; i++)
        {
            if (diffLevelSprites[i] == currSprite)
            {
                currentSpriteIndex = i;
            }
        }
        if (currentSpriteIndex == -1)
        {
            Debug.LogError("Tower Sprite provided in SpriteRenderer does not match with any of the ones given in the TowerView script!");
            currentSpriteIndex = 0;
        }

        spriteRenderer.sprite = diffLevelSprites[currentSpriteIndex];
    }

    /// <summary>
    /// Check if Tower at last level
    /// </summary>
    public bool IsAtLastSprite()
        => currentSpriteIndex == diffLevelSprites.Length - 1;

    /// <summary>
    /// Hides/shows outline depending on whether the Tower is selected for Upgrading/Selling
    /// </summary>
    public void UpdateOutline()
    {
        childSRVisibleWhileUpgrading.enabled = !childSRVisibleWhileUpgrading.enabled;
    }

    /// <summary>
    /// Called on Tower Upgrade to update Tower visuals accordingly
    /// </summary>
    public void Upgrade()
    {
        currentSpriteIndex++;
        if (currentSpriteIndex >= diffLevelSprites.Length)
        {
            Debug.LogError("currentSpriteIndex is out of bounds - upgrading a tower more times than allowed!");
            return;
        }

        spriteRenderer.sprite = diffLevelSprites[currentSpriteIndex];
        childSRVisibleWhileUpgrading.sprite = spriteRenderer.sprite;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerView : MonoBehaviour
{
    [SerializeField]
    private Sprite[] diffLevelSprites;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex;

    /// <summary>
    /// Sprite Renderer of Child of the tower that is made visible when tower selected for upgrading or selling<br></br>
    /// to make it clear which tower is currently selected
    /// </summary>
    private SpriteRenderer childSRVisibleWhileUpgrading;

    void Start()
    {
        InitSprite();

        childSRVisibleWhileUpgrading = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

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

    public bool IsAtLastSprite()
        => currentSpriteIndex == diffLevelSprites.Length - 1;

    public void UpdateOutline()
    {
        childSRVisibleWhileUpgrading.enabled = !childSRVisibleWhileUpgrading.enabled;
    }

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
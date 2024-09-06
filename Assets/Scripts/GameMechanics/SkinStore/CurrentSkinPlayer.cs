using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class CurrentSkinPlayer : MonoBehaviour
{
    [SerializeField] private Sprite[] skins;
    private int currentSkinIndex;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadSaveCloud();
        ApplyCurrentSkin();
    }

    public void LoadSaveCloud()
    {
        currentSkinIndex = YandexGame.savesData.currentSkinIndex;
    }

    public void ApplyCurrentSkin()
    {
        if (skins != null && skins.Length > 0 && currentSkinIndex >= 0 && currentSkinIndex < skins.Length)
        {
            spriteRenderer.sprite = skins[currentSkinIndex];
        }
        else
        {
            Debug.LogError("Invalid skin index or skins array is empty.");
        }
    }
}

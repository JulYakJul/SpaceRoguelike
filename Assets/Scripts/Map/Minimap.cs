using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("Minimap Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private GameObject enemyIconPrefab;
    [SerializeField] private GameObject objectIconPrefab;
    [SerializeField] private float mapScale = 1f;
    [SerializeField] private Sprite[] itemSprites;

    private Dictionary<GameObject, GameObject> enemyIcons = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> objectIcons = new Dictionary<GameObject, GameObject>();

    private void Update()
    {
        // Update the player's position on the minimap
        UpdatePlayerPosition();

        // Update the positions of enemy and object icons on the minimap
        UpdateIcons(enemyIcons);
        UpdateIcons(objectIcons);
    }

    private void UpdatePlayerPosition()
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.y) * mapScale;
        minimapRect.localPosition = playerPos;
    }

    private void UpdateIcons(Dictionary<GameObject, GameObject> icons)
    {
        // List to store keys of icons that need to be removed
        List<GameObject> keysToRemove = new List<GameObject>();

        foreach (var kvp in icons)
        {
            GameObject obj = kvp.Key;
            GameObject icon = kvp.Value;

            if (obj == null)
            {
                keysToRemove.Add(kvp.Key);
            }
            else
            {
                RectTransform rectTransform = icon.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 iconPos = new Vector2(obj.transform.position.x, obj.transform.position.y) * mapScale;
                    rectTransform.localPosition = iconPos;
                }
                else
                {
                    Debug.LogWarning("Icon prefab does not have RectTransform component.");
                }
            }
        }

        // Remove icons for destroyed objects
        RemoveIcons(keysToRemove, icons);
    }

    private void RemoveIcons(List<GameObject> keysToRemove, Dictionary<GameObject, GameObject> icons)
    {
        foreach (var key in keysToRemove)
        {
            if (icons.TryGetValue(key, out GameObject icon))
            {
                Destroy(icon);
                icons.Remove(key);
            }
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            GameObject icon = Instantiate(enemyIconPrefab, minimapRect);
            if (icon.GetComponent<RectTransform>() != null)
            {
                enemyIcons[enemy] = icon;
            }
            else
            {
                Debug.LogWarning("Enemy icon prefab does not have RectTransform component.");
                Destroy(icon);
            }
        }
    }

    public void AddObject(GameObject obj, int itemType)
    {
        if (obj != null)
        {
            GameObject icon = Instantiate(objectIconPrefab, minimapRect);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage != null && itemType >= 0 && itemType < itemSprites.Length)
                {
                    iconImage.sprite = itemSprites[itemType];
                }
                objectIcons[obj] = icon;
            }
            else
            {
                Debug.LogWarning("Object icon prefab does not have RectTransform component.");
                Destroy(icon);
            }
        }
    }
}

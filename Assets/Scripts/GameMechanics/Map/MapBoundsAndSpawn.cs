using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MapBoundsAndSpawn : MonoBehaviour
{
    [Header("Map Settings")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] initialEnemyPrefabs;
    [SerializeField] private GameObject[] secondWaveEnemyPrefabs;
    [SerializeField] private GameObject[] thirdWaveEnemyPrefabs;
    [SerializeField] private GameObject[] fourthWaveEnemyPrefabs;
    [SerializeField] private GameObject[] fifthWaveEnemyPrefabs;
    [SerializeField] private GameObject[] sixthWaveEnemyPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Minimap minimap;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxEnemies;
    // [SerializeField] private int maxItems;
    [SerializeField] private float waveDuration;
    [SerializeField] private float minimumDistanceBetweenItems;
    [SerializeField] private float minimumDistanceOfEnemiesFromPlayer;

    private int currentEnemies;
    private int currentItems;
    private List<GameObject> spawnedItems = new List<GameObject>();

    [SerializeField] private float[] itemWeights;

    private bool hasReachedMaxSpeed;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnObjectsCoroutine());
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        GameObject[][] waves = new GameObject[][]
        {
            initialEnemyPrefabs,
            secondWaveEnemyPrefabs,
            thirdWaveEnemyPrefabs,
            fourthWaveEnemyPrefabs,
            fifthWaveEnemyPrefabs,
            sixthWaveEnemyPrefabs
        };

        int waveCount = waves.Length;
        int currentWaveIndex = 0;

        while (true)
        {
            SpawnWave(waves[currentWaveIndex]);
            currentWaveIndex = (currentWaveIndex + 1) % waveCount;
            yield return new WaitForSeconds(waveDuration);
        }
    }

    private void SpawnWave(GameObject[] enemyPrefabs)
    {
        currentEnemies = 0;
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy(enemyPrefabs);
        }

        currentItems = 0;

        // for (int i = 0; i < maxItems; i++)
        // {
        //     SpawnItem();
        // }
    }

    private void SpawnEnemy(GameObject[] enemyPrefabs)
    {
        Vector2 spawnPosition = GetRandomPositionWithinBounds();

        int maxAttempts = 10;
        int attempts = 0;
        while (attempts < maxAttempts && !IsPositionFarEnoughFromPlayer(spawnPosition))
        {
            spawnPosition = GetRandomPositionWithinBounds();
            attempts++;
        }

        if (!IsPositionFarEnoughFromPlayer(spawnPosition))
        {
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        minimap.AddEnemy(enemy);
        currentEnemies++;
    }

    // private void SpawnItem()
    // {
    //     Vector2 spawnPosition = GetRandomPositionWithinBounds();
    //     int randomIndex = GetWeightedRandomIndex(itemWeights);
        
    //     if (hasReachedMaxSpeed && (randomIndex == 1 || randomIndex == 2))
    //     {
    //         return;
    //     }

    //     int maxAttempts = 10;
    //     int attempts = 0;
    //     while (attempts < maxAttempts && !IsPositionValid(spawnPosition))
    //     {
    //         spawnPosition = GetRandomPositionWithinBounds();
    //         attempts++;
    //     }

    //     if (!IsPositionValid(spawnPosition))
    //     {
    //         return;
    //     }

    //     GameObject item = Instantiate(itemPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    //     minimap.AddObject(item, randomIndex);
    //     spawnedItems.Add(item);
    //     currentItems++;
    // }

    private bool IsPositionValid(Vector2 position)
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                float distance = Vector2.Distance(position, item.transform.position);
                if (distance < minimumDistanceBetweenItems)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsPositionFarEnoughFromPlayer(Vector2 position)
    {
        if (player != null)
        {
            float distance = Vector2.Distance(position, player.transform.position);
            return distance >= minimumDistanceOfEnemiesFromPlayer;
        }
        return true;
    }

    private Vector2 GetRandomPositionWithinBounds()
    {
        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        return new Vector2(randomX, randomY);
    }

    public void OnEnemyDestroyed()
    {
        currentEnemies = Mathf.Max(currentEnemies - 1, 0);
    }

    public void OnItemDestroyed(GameObject item)
    {
        if (spawnedItems.Contains(item))
        {
            spawnedItems.Remove(item);
        }
        currentItems = Mathf.Max(currentItems - 1, 0);
    }

    private int GetWeightedRandomIndex(float[] weights)
    {
        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }

        return weights.Length - 1;
    }
}

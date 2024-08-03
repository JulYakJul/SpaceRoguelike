using UnityEngine;
using System.Collections;

public class MapBounds : MonoBehaviour
{
    [Header("Map Settings")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] initialEnemyPrefabs;
    [SerializeField] private GameObject[] secondWaveEnemyPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Minimap minimap;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxEnemies;
    [SerializeField] private int maxItems;
    [SerializeField] private float waveDuration;

    private int currentEnemies;
    private int currentItems;

    private void Start()
    {
        StartCoroutine(SpawnObjectsCoroutine());
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        GameObject[][] waves = new GameObject[][]
        {
            initialEnemyPrefabs,
            secondWaveEnemyPrefabs
        };

        while (true)
        {
            foreach (var wave in waves)
            {
                SpawnWave(wave);
                yield return new WaitForSeconds(waveDuration);
            }
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

        for (int i = 0; i < maxItems; i++)
        {
            SpawnItem();
        }
    }

    private void SpawnEnemy(GameObject[] enemyPrefabs)
    {
        Vector2 spawnPosition = GetRandomPositionWithinBounds();
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        minimap.AddEnemy(enemy);
        currentEnemies++;
    }

    private void SpawnItem()
    {
        Vector2 spawnPosition = GetRandomPositionWithinBounds();
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject item = Instantiate(itemPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        minimap.AddObject(item, randomIndex);
        currentItems++;
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

    public void OnItemDestroyed()
    {
        currentItems = Mathf.Max(currentItems - 1, 0);
    }
}

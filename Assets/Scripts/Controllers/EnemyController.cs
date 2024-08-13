using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float shootingInterval;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform[] bulletSpawnPoints;
    [SerializeField] private float visionRadius;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float maxTransparency;
    public Image healthBar;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    private int currentHealth;

    [Header("Upgrade Settings")]
    [SerializeField] private GameObject[] upgradePrefabs;


    private Transform player;
    private MapBoundsAndSpawn mapBounds;
    private SpriteRenderer spriteRenderer;
    private float shootingTimer;
    
    private const float AngleOffset = -90f;

    void Start()
    {
        InitializeComponents();
        InitializeSettings();
    }

    void Update()
    {
        if (player == null) return;

        HandleMovementAndShooting();
        UpdateHealthBar();
        LockHealthBarRotation();
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        mapBounds = FindObjectOfType<MapBoundsAndSpawn>();
    }

    private void InitializeSettings()
    {
        currentHealth = maxHealth;
        UpdateTransparency();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;  // Обновляем fillAmount
            UpdateHealthBarColor();
        }
    }

    private void UpdateHealthBarColor()
    {
        if (healthBar != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthBar.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
        }
    }

    private void LockHealthBarRotation()
    {
        if (healthBar != null)
        {
            healthBar.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void HandleMovementAndShooting()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= visionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            RotateTowards(direction);
            MoveTowardsPlayer(direction);
            ClampPositionWithinBounds();
            HandleShooting();
        }
    }

    private void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + AngleOffset));
    }

    private void MoveTowardsPlayer(Vector2 direction)
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void ClampPositionWithinBounds()
    {
        if (mapBounds != null)
        {
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(transform.position.x, mapBounds.minBounds.x, mapBounds.maxBounds.x),
                Mathf.Clamp(transform.position.y, mapBounds.minBounds.y, mapBounds.maxBounds.y)
            );
            transform.position = clampedPosition;
        }
    }

    private void HandleShooting()
    {
        shootingTimer += Time.deltaTime;
        if (shootingTimer >= shootingInterval)
        {
            Shoot();
            shootingTimer = 0f;
        }
    }

    private void Shoot()
    {
        foreach (Transform spawnPoint in bulletSpawnPoints)
        {
            Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            DropUpgrade();
            Destroy(gameObject);
        }
        UpdateTransparency();
    }

    private void DropUpgrade()
    {
        if (upgradePrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, upgradePrefabs.Length);
            Instantiate(upgradePrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }


    private void UpdateTransparency()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        float transparency = Mathf.Lerp(1f, maxTransparency, 1f - healthPercentage);
        Color color = spriteRenderer.color;
        color.a = transparency;
        spriteRenderer.color = color;

        UpdateHealthBarColor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BulletPlayer"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                Destroy(collision.gameObject);
            }
        }
    }
}

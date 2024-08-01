using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float decelerationRate;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Stats Settings")]
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private int maxStrength;
    [SerializeField] private int maxSpeed;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Map Settings")]
    [SerializeField] private MapBounds mapBounds;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float speed;
    private int currentHealth;
    private int strength;
    private int playerSpeed;
    private float nextFireTime;
    private Vector2 movement;

    private const float SpeedMultiplier = 2f;
    private const float StrengthMultiplier = 3f;

    void Start()
    {
        InitializeComponents();
        InitializePlayerSettings();
        UpdateUI();
    }

    void Update()
    {
        HandleMovementInput();
        RotateTowardsMovement();
        DetectAndShootEnemies();
    }

    void FixedUpdate()
    {
        MovePlayer();
        ClampPlayerPosition();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void InitializePlayerSettings()
    {
        speed = baseSpeed;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector2(moveHorizontal, moveVertical).normalized;
    }

    private void RotateTowardsMovement()
    {
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
    }

    private void MovePlayer()
    {
        if (movement != Vector2.zero)
        {
            rb.velocity = movement * speed;
        }
        else
        {
            rb.velocity *= decelerationRate;
        }
    }

    private void ClampPlayerPosition()
    {
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(transform.position.x, mapBounds.minBounds.x, mapBounds.maxBounds.x),
            Mathf.Clamp(transform.position.y, mapBounds.minBounds.y, mapBounds.maxBounds.y)
        );
        rb.position = clampedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BulletEnemy"))
        {
            TakeDamage(5);
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        healthSlider.value = currentHealth;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpgradeStrength()
    {
        UpgradeStat(ref strength, maxStrength, UpdateStrengthText, UpdateBulletDamage);
    }

    public void UpgradeSpeed()
    {
        UpgradeStat(ref playerSpeed, maxSpeed, UpdateSpeedText, () =>
        {
            speed = baseSpeed + (playerSpeed - 1) * SpeedMultiplier;
        });
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthSlider.value = currentHealth;
        UpdateHealthText();
    }

    public void UpgradeStrength(int amount)
    {
        strength = Mathf.Clamp(strength + amount, 0, maxStrength);
        UpdateStrengthText();
        UpdateBulletDamage();
    }

    public void UpgradeSpeed(int amount)
    {
        playerSpeed = Mathf.Clamp(playerSpeed + amount, 0, maxSpeed);
        speed = baseSpeed + (playerSpeed - 1) * SpeedMultiplier;
        UpdateSpeedText();
    }

    private void UpgradeStat(ref int stat, int maxStat, System.Action updateText, System.Action additionalAction = null)
    {
        if (stat < maxStat)
        {
            stat++;
            updateText?.Invoke();
            additionalAction?.Invoke();
        }
    }

    private void UpdateUI()
    {
        UpdateHealthText();
        UpdateStrengthText();
        UpdateSpeedText();
    }

    private void UpdateHealthText()
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

    private void UpdateStrengthText()
    {
        strengthText.text = $"{strength}/{maxStrength}";
    }

    private void UpdateSpeedText()
    {
        speedText.text = $"{playerSpeed}/{maxSpeed}";
    }

    private void DetectAndShootEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            Transform closestEnemy = GetClosestEnemy(enemies);
            RotateTowards(closestEnemy);

            if (Time.time > nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    private Transform GetClosestEnemy(Collider2D[] enemies)
    {
        Transform closestEnemy = enemies[0].transform;
        float closestDistance = Vector2.Distance(transform.position, closestEnemy.position);

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private void RotateTowards(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = 10 + (strength * 5);
        }
    }

    private void UpdateBulletDamage()
    {
        Bullet bulletScript = bulletPrefab.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = 5 + (strength * 5);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Cinemachine;

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
    public int currentHealth;

    [Header("Stats Settings")]
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private int maxStrength;
    public int maxSpeed;
    public float speed;
    public int strength;
    public int playerSpeed;

    [Header("Combat Settings")]
    [SerializeField] private GameObject[] bulletPrefabs;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float fireRate;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask enemyLayer;
    private int nextFirePointIndex = 1;

    [Header("Map Settings")]
    [SerializeField] private MapBounds mapBounds;

    [Header("Upgrade Panel")]
    [SerializeField] private GameObject upgradePanel;
    public GameObject UpgradePanel => upgradePanel;
    public bool IsUpgradePanelActive { get; set; } = false;

    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraZoomSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float nextFireTime;
    private Vector2 movement;

    private int currentBulletIndex = 0;
    private GameObject bulletPrefab;

    private const float StrengthMultiplier = 3f;

    void Start()
    {
        InitializeComponents();
        InitializePlayerSettings();
        UpdateUI();

        if (firePoints.Length > 0)
        {
            firePoints[0].gameObject.SetActive(true);
        }

        StartCoroutine(ShowUpgradePanelCoroutine());
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

        if (bulletPrefabs == null || bulletPrefabs.Length == 0)
        {
            Debug.LogError("Bullet Prefabs array is not initialized or is empty.");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogError("Fire Points array is not initialized or is empty.");
            return;
        }

        foreach (var firePoint in firePoints)
        {
            firePoint.gameObject.SetActive(false);
        }

        if (firePoints.Length > 0)
        {
            firePoints[0].gameObject.SetActive(true);
        }

        if (currentBulletIndex < 0 || currentBulletIndex >= bulletPrefabs.Length)
        {
            currentBulletIndex = 0;
        }

        UpdateBulletPrefab();
    }

    private void OnUpgradeSelected(System.Action upgradeAction)
    {
        upgradeAction?.Invoke();
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        IsUpgradePanelActive = false;
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
        AdjustSpeedOnDamage();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void AdjustSpeedOnDamage()
    {
        if (speed > 7)
        {
            playerSpeed--;
            speed = Mathf.Max(baseSpeed + (playerSpeed - 1), 7);
            UpdateSpeedText();
        }
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
        speed = baseSpeed + (playerSpeed - 1);
        UpdateSpeedText();
    }

    public void UpgradeWeapon()
    {
        if (nextFirePointIndex < firePoints.Length)
        {
            if (!firePoints[nextFirePointIndex].gameObject.activeSelf)
            {
                firePoints[nextFirePointIndex].gameObject.SetActive(true);
            }

            nextFirePointIndex++;
        }
    }

    public void UpgradeBulletType()
    {
        currentBulletIndex = Mathf.Clamp(currentBulletIndex + 1, 0, bulletPrefabs.Length - 1);
        UpdateBulletPrefab();
    }

    public void IncreasingHealthScale(int amount)
    {
        maxHealth += amount;
        healthSlider.maxValue = maxHealth;
        UpdateHealthText();
    }

    public void UpgradeFireRate(int amount)
    {
        fireRate += amount;
    }

    public void UpgradeDetectionRadius(int amount)
    {
        detectionRadius += amount;

        if (virtualCamera != null)
        {
            var currentSize = virtualCamera.m_Lens.OrthographicSize;
            var targetSize = currentSize + amount;
            StartCoroutine(SmoothCameraZoom(currentSize, targetSize));
        }
    }

    public void UpgradeStrengthScale(int amount)
    {
        maxStrength += amount;
        UpdateStrengthText();
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthSlider.value = currentHealth;
        UpdateHealthText();
    }

    private void UpdateBulletPrefab()
    {
        if (bulletPrefabs == null || bulletPrefabs.Length == 0)
        {
            Debug.LogError("Bullet Prefabs array is empty when trying to update bullet prefab.");
            return;
        }

        if (currentBulletIndex < 0 || currentBulletIndex >= bulletPrefabs.Length)
        {
            currentBulletIndex = 0;
        }

        bulletPrefab = bulletPrefabs[currentBulletIndex];
    }

    private IEnumerator SmoothCameraZoom(float startSize, float endSize)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cameraZoomSpeed)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, endSize, elapsedTime / cameraZoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = endSize;
    }

    private IEnumerator ShowUpgradePanelCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            if (!IsUpgradePanelActive) 
            {
                upgradePanel.SetActive(true);
                Time.timeScale = 0f;
                IsUpgradePanelActive = true;
                yield return new WaitUntil(() => !upgradePanel.activeSelf);
                Time.timeScale = 1f;
                IsUpgradePanelActive = false;
            }
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
        if (bulletPrefab != null)
        {
            foreach (var firePoint in firePoints)
            {
                if (firePoint.gameObject.activeSelf)
                {
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.damage = 10 + (strength * 5);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Bullet prefab is not set when trying to shoot.");
        }
    }

    private void UpdateBulletDamage()
    {
        if (bulletPrefab != null)
        {
            Bullet bulletScript = bulletPrefab.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = 5 + (strength * 5);
            }
        }
        else
        {
            Debug.LogError("Bullet prefab is not set when trying to update bullet damage.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
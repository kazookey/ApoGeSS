using UnityEngine;

public class HiredDefender : MonoBehaviour
{
    [Header("Patrol Y Range")]
    public float minY = -3f;
    public float maxY =  3f;
    public float moveSpeed = 2f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float shootInterval = 1.2f;
    private float shootTimer = 0f;

    [Header("Enemy Targeting")]
    public string enemyTag = "Enemy";
    public float detectionRange = 12f;

    [Header("Panic / Flee State")]
    public float fleeSpeed = 5f;
    private bool isFleeing = false;

    private Transform targetEnemy;

    void Start()
    {
        // Register self with the Manager
        if (NightManager.Instance != null)
            NightManager.Instance.RegisterDefender(this);
    }

    void Update()
    {
        if (isFleeing)
        {
            // Move right rapidly to escape
            transform.Translate(Vector3.right * fleeSpeed * Time.unscaledDeltaTime);
            return;
        }

        FindLeftEnemy();
        MoveVertical();
        TryShoot();
    }

    // -----------------------
    // PANIC MODE
    // -----------------------
    public void StartRunningAway()
    {
        isFleeing = true;
        Destroy(gameObject, 3f);
    }

    // -----------------------
    // ENEMY DETECTION (LEFT ONLY)
    // -----------------------
    void FindLeftEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            // Only detect enemies LEFT of the defender
            if (e.transform.position.x > transform.position.x)
                continue;

            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < closestDist && dist < detectionRange)
            {
                closestDist = dist;
                closest = e.transform;
            }
        }

        targetEnemy = closest;
    }

    // -----------------------
    // VERTICAL MOVEMENT
    // Reduced smoothing = snappier/harder movements (retro feel)
    // -----------------------
    void MoveVertical()
    {
        if (targetEnemy == null)
        {
            // Simple PATROL (up/down bouncing)
            float newY = transform.position.y + moveSpeed * Time.deltaTime;

            // If going past range, reverse direction
            if (newY > maxY || newY < minY)
                moveSpeed *= -1;

            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(newY, minY, maxY),
                transform.position.z
            );

            return;
        }

        // Move up/down directly toward the enemy with minimal smoothing
        float direction = Mathf.Sign(targetEnemy.position.y - transform.position.y);

        transform.position += new Vector3(
            0,
            direction * moveSpeed * Time.deltaTime,
            0
        );

        // Clamp to allowed Y range
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    // -----------------------
    // SHOOTING (LEFT ONLY)
    // -----------------------
    void TryShoot()
    {
        if (targetEnemy == null) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();

        // ALWAYS shoot left
        rb.linearVelocity = Vector2.left * bulletSpeed;
    }
}

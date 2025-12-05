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
    private float patrolDirection = 1f; // 1 = up, -1 = down


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
            Vector3 pos = e.transform.position;

            // Must be LEFT of the defender
            if (pos.x >= transform.position.x)
                continue;

            // Must be within detection range (horizontal or full 2D distance)
            float dist = Vector3.Distance(transform.position, pos);
            if (dist > detectionRange)
                continue;

            // Track the closest valid enemy
            if (dist < closestDist)
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
            // PATROL (up/down bouncing)
            float newY = transform.position.y + moveSpeed * patrolDirection * Time.deltaTime;

            // If going past range, reverse direction and clamp
            if (newY > maxY)
            {
                newY = maxY;
                patrolDirection = -1f;
            }
            else if (newY < minY)
            {
                newY = minY;
                patrolDirection = 1f;
            }

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            return;
        }

        // Move toward enemy vertically
        float direction = Mathf.Sign(targetEnemy.position.y - transform.position.y);
        transform.position += new Vector3(0, direction * moveSpeed * Time.deltaTime, 0);

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

using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 12f;

    [Header("Ammo")]
    public ItemData ammoItem;
    public bool useAmmo = false;

    [Header("Fire Cooldown")]
    public float fireCooldown = 0.25f;
    private float fireTimer = 0f;

    [Header("Input Buffer")]
    public float inputBufferTime = 0.15f; // how long a buffered input is stored
    private float bufferedInput = 0f;

    void Update()
    {
        // Tick timers
        fireTimer -= Time.deltaTime;
        bufferedInput -= Time.deltaTime;

        // Check mouse input and store it in buffer
        if (Input.GetMouseButtonDown(0))
            bufferedInput = inputBufferTime;

        // Try to shoot if buffered input exists and cooldown is ready
        if (bufferedInput > 0f && fireTimer <= 0f)
        {
            TryShoot();
            bufferedInput = 0f; // consume buffered input
        }
    }

    void TryShoot()
    {
        // Safety check
        if (!NightManager.Instance.CanPlayerShoot())
            return;

        if (!useAmmo)
        {
            Shoot();
            fireTimer = fireCooldown;
            return;
        }

        if (ammoItem == null)
        {
            Debug.LogError("Assign Ammo ItemData!");
            return;
        }

        if (GameManager.Instance.TryConsumeItem(ammoItem, 1))
        {
            Shoot();
            fireTimer = fireCooldown;
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * bulletSpeed;
    }
}
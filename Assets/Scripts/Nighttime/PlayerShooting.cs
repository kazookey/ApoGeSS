using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 12f;

    [Header("Ammo (optional until daytime is finished)")]
    public ItemData ammoItem;   // will be assigned later
    public bool useAmmo = false; // testing toggle

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    void TryShoot()
    {
        // inf ammo mode for testing
        if (!useAmmo)
        {
            Shoot();
            return;
        }

        if (ammoItem == null)
        {
            Debug.LogError("Assign the Ammo ItemData in the PlayerShooting Inspector!");
            return;
        }

        // Use the new safe function
        if (GameManager.Instance.TryConsumeItem(ammoItem, 1))
        {
            Shoot();
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

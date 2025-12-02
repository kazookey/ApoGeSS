using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 12f;

    public ItemData ammoItem;
    public bool useAmmo = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    void TryShoot()
    {
        
        if (!NightManager.Instance.CanPlayerShoot())
            return;

        if (!useAmmo)
        {
            Shoot();
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

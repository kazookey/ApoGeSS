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
        // inf ammo mode for tesitng
        if (!useAmmo)
        {
            Shoot();
            return;
        }

        // real ammo mode for when daytime is finished
        if (ammoItem == null)
        {
            Debug.LogWarning("Ammo ItemData not assigned!");
            return;
        }

        // check daytime inv
        int ammoCount = GameManager.Instance.GetItemCount(ammoItem);

        if (ammoCount > 0)
        {
           
            GameManager.Instance.inventory[ammoItem]--;
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

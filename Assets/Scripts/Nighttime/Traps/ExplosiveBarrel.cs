using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float radius = 1.5f;
    public int damage = 10;
    public GameObject explosionEffect;

    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>().TakeDamage(damage);
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

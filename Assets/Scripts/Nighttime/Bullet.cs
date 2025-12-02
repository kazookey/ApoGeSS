using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }

        var barrel = other.GetComponent<ExplosiveBarrel>();
            if (barrel != null)
            {
                barrel.ActivateTrap();
                Destroy(gameObject);
            }

    }

}

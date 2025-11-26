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

         if (other.GetComponent<ExplosiveBarrel>())
{
    other.GetComponent<ExplosiveBarrel>().Explode();
    Destroy(gameObject);
}
    }

}

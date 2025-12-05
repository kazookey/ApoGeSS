using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    public int damage = 1;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody2D>();
         transform.rotation = Quaternion.Euler(0, 0, 90f); 
    
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

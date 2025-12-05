using UnityEngine;

public class Molotov : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float launchSpeed = 5f;
    public float lifetime = 3f;
    public int damageAmount = 2; 

    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        GameObject barricadeObject = GameObject.FindGameObjectWithTag("Barricade");
        if (barricadeObject != null)
        {
            target = barricadeObject.transform;
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        Destroy(gameObject, lifetime); 
    }

    void Start()
    {
        
        if (target != null && rb != null)
        {
            Vector2 targetPosition = target.position;
            Vector2 launchVelocity = CalculateLaunchVelocity(targetPosition, launchSpeed);
            rb.linearVelocity = launchVelocity;
        }
    }

    
    private Vector2 CalculateLaunchVelocity(Vector2 targetPos, float speed)
    {
        Vector2 displacement = targetPos - (Vector2)transform.position;
        
        displacement.y += 1.0f; 
        
        return displacement.normalized * speed;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
    
        if (collision.gameObject.CompareTag("Barricade")) 
        {
            Barricade barricade = collision.gameObject.GetComponent<Barricade>();
            if (barricade != null && barricade.CanTakeDamage())
            {
                barricade.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
        
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }

}
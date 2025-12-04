using UnityEngine;

public class Molotov : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float launchSpeed = 5f;
    public float lifetime = 3f;
    public float damageRadius = 2f;
    public int damageAmount = 2;

    [Header("Visuals")]
    public GameObject fireEffectPrefab; // Assign an explosion/fire particle effect prefab

    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Barricade").transform; // Target the Barricade

     
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
       
        if (collision.gameObject.CompareTag("Barricade") || collision.gameObject.CompareTag("Ground")) 
        {
            ExplodeAndDamage();
        }
    }

    void ExplodeAndDamage()
    {
        
        if (fireEffectPrefab != null)
        {
            Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
        }

       
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (var hit in hitColliders)
        {
           
            Barricade barricade = hit.GetComponent<Barricade>();
            if (barricade != null && barricade.CanTakeDamage())
            {
                barricade.TakeDamage(damageAmount);
            }
            
           
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
        
        
        Destroy(gameObject);
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
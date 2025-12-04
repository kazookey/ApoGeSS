using UnityEngine;

public class Raider : Enemy
{
    public float throwRange = 4f;
    public float throwCooldown = 3f;

    private float cooldownTimer = 0;
    public GameObject molotovPrefab; 
    
    
    private Transform barricadeTarget;
   
    
    void Start()
    {
        base.Start();
        barricadeTarget = GameObject.FindGameObjectWithTag("Barricade").transform;
       
    }

    protected override void Move()
    {
      
        if (barricadeTarget == null)
        {
            base.Move();
            return;
        }
        
       
        float distance = Vector2.Distance(transform.position, barricadeTarget.position);
        

        if (distance <= throwRange)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                ThrowMolotov();
                cooldownTimer = throwCooldown;
            }
            return;
        }

        
        base.Move();

        Vector2 dir = (barricadeTarget.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    void ThrowMolotov()
    {
       
        Instantiate(molotovPrefab, transform.position, Quaternion.identity);
        
    }
}
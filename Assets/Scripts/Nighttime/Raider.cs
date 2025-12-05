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
        barricadeTarget = GameObject.FindGameObjectWithTag("Barricade")?.transform;
    }

    protected override void Move()
    {
        // If no barricade is found, move right (fallback)
        if (barricadeTarget == null)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            return;
        }

        // Distance check for throwing range
        float distance = Vector2.Distance(transform.position, barricadeTarget.position);

        // Inside throw range → stop moving & throw on cooldown
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

        // OUTSIDE throw range → move straight right only (no homing)
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    void ThrowMolotov()
    {
        Instantiate(molotovPrefab, transform.position, Quaternion.identity);
    }
}
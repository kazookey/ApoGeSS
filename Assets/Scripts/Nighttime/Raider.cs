using UnityEngine;

public class Raider : Enemy
{
    public float throwRange = 4f;
    public float throwCooldown = 3f;

    private float cooldownTimer = 0;
    public GameObject molotovPrefab;

    Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Move()
    {
       
        float distance = Vector2.Distance(transform.position, player.position);

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
    }

    void ThrowMolotov()
    {
        Instantiate(molotovPrefab, transform.position, Quaternion.identity);
    }
}

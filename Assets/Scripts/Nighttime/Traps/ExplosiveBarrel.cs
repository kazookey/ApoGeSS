using UnityEngine;

public class ExplosiveBarrel : TrapBase
{
    public float radius = 1.5f;
    public int damage = 10;

    public override void ActivateTrap()
    {
        base.ActivateTrap();

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>().TakeDamage(damage);
        }

        
        Destroy(gameObject, 0.1f);
    }
}

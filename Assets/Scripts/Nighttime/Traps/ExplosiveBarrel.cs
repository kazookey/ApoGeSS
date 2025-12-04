using UnityEngine;

public class ExplosiveBarrel : TrapBase
{
    public float radius = 1.5f;
    public int damage = 10;

    [Header("Visuals")]
    public Sprite destroyedSprite;
    public GameObject explosionParticlesPrefab;

    private bool isDestroyed = false;

    public override void ActivateTrap()
    {
        
        if (isDestroyed)
        {
            return;
        }

        base.ActivateTrap();
        isDestroyed = true;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>().TakeDamage(damage);
        }

        if (explosionParticlesPrefab != null)
        {
            Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && destroyedSprite != null)
        {
            sr.sprite = destroyedSprite;
        }

        Collider2D trapCollider = GetComponent<Collider2D>();
        if (trapCollider != null)
        {
            trapCollider.enabled = false;
        }
    }
}

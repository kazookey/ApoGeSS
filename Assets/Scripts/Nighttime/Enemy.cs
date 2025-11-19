using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int health = 2;
    public int damagePerSecond = 1;

    Transform barricade;

    void Start()
    {
        barricade = GameObject.FindWithTag("Barricade").transform;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            barricade.position,
            moveSpeed * Time.deltaTime
        );
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Barricade"))
        {
            col.gameObject.GetComponent<Barricade>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
        WaveManager.EnemyKilled();
    }
}

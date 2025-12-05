using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 3;
    public float moveSpeed = 1.5f;
    public bool isStunned = false;


    protected int currentHP;
    protected Transform player;

    protected void Start()
    {
        currentHP = maxHP;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }
    }

    void Update()
    {
        if (!isStunned && player != null)
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        // Move straight to the right, ignoring the player
        Vector2 dir = Vector2.right;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        WaveManager.EnemyKilled();
        Destroy(gameObject);
    }
}

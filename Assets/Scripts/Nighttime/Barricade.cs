using UnityEngine;

public class Barricade : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;

    public float damageCooldown = 0.5f;
    float damageTimer = 0f;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("Barricade HP: " + currentHP);

        if (currentHP <= 0)
        {
            Debug.Log("Barricade destroyed!");
            
        }
    }

    public bool CanTakeDamage()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
            return false;
        }

        damageTimer = damageCooldown;
        return true;
    }
}

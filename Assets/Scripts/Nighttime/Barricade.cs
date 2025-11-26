using UnityEngine;

public class Barricade : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Debug.Log("Barricade destroyed!");
        }
    }
}

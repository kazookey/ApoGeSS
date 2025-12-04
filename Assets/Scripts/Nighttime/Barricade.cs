using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f; // Standardized to float
    public float currentHealth;

    [Header("UI")]
    public Image hpImage; // Drag your "Health Bar Fill" image here

    [Header("Invincibility")]
    public float damageCooldown = 0.5f;
    private float damageTimer = 0f;

    void Start()
    {
        // 1. LOAD Global Health (The Link)
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.barricadeHealth;
        }
        else
        {
            currentHealth = maxHealth; // Fallback for testing Night scene alone
        }
        
        UpdateHPDisplay();
    }

    void Update()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    public bool CanTakeDamage()
    {
        return damageTimer <= 0;
    }

    public void TakeDamage(float dmg)
    {
        if (!CanTakeDamage()) return;
        
        damageTimer = damageCooldown; // Reset cooldown
        currentHealth -= dmg;

        // 2. SAVE Global Health Immediately (The Link)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barricadeHealth = currentHealth;
        }

        Debug.Log($"Barricade Hit! HP: {currentHealth}");
        UpdateHPDisplay();

        // 3. Check Loss Condition
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (NightManager.Instance != null)
            {
                // Tell the manager we lost
                NightManager.Instance.EndNightSequence(false, 0);
            }
        }
    }

    void UpdateHPDisplay()
    {
        if (hpImage != null)
        {
            // Simple percentage math (0.0 to 1.0)
            hpImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }
}
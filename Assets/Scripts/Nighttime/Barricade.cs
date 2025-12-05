using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    private float startingNightHealth; // Store health at start of night

    [Header("UI")]
    public Image hpImage;

    [Header("Invincibility")]
    public float damageCooldown = 0.5f;
    private float damageTimer = 0f;

    void Start()
    {
        // Load health from GameManager
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.barricadeHealth;
        }
        else
        {
            currentHealth = maxHealth; 
        }

        // Save the starting health for this night
        startingNightHealth = currentHealth;

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

        damageTimer = damageCooldown;
        currentHealth -= dmg;

        // Save health globally
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barricadeHealth = currentHealth;
        }

        Debug.Log($"Barricade Hit! HP: {currentHealth}");
        UpdateHPDisplay();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHPDisplay();

            // Tell NightManager we lost
            if (NightManager.Instance != null)
            {
                NightManager.Instance.EndNightSequence(false, 0);
            }

            // Reset health to starting night value
            ResetHealthToStartOfNight();
        }
    }

    void UpdateHPDisplay()
    {
        if (hpImage != null)
        {
            hpImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    void ResetHealthToStartOfNight()
    {
        currentHealth = startingNightHealth;

        // Also update GameManager so next night starts with this value
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barricadeHealth = currentHealth;
        }

        Debug.Log($"Barricade health reset to start-of-night value: {currentHealth}");
        UpdateHPDisplay();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Renamed from maxHP to match NightManager
    public float currentHealth;    // Renamed from currentHP

    [Header("UI")]
    public Slider hpSlider;

    [Header("Invincibility")]
    public float damageCooldown = 0.5f;
    private float damageTimer = 0f;

    void Start()
    {
        // 1. Sync with GameManager (Rein's Day Logic)
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.barricadeHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
        
        UpdateHPSlider();
    }

    public void Update()
    {
        // 2. Ramon's Cooldown Logic (Tick down the timer)
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    // --- THIS IS THE MISSING FUNCTION ---
    public bool CanTakeDamage()
    {
        if (damageTimer > 0) return false;
        
        damageTimer = damageCooldown;
        return true;
    }
    // ------------------------------------

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        
        // Update Global State
        if (GameManager.Instance != null)
        {
            GameManager.Instance.barricadeHealth = currentHealth;
        }

        UpdateHPSlider();

        if (currentHealth <= 0)
        {
            // Trigger Loss via NightManager
            if (NightManager.Instance != null)
            {
                NightManager.Instance.EndNightSequence(false, 0);
            }
        }
    }

    
}
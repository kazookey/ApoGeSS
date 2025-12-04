using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;

    [Header("UI")]
    public Slider hpSlider;

    public float damageCooldown = 0.5f;
    float damageTimer = 0f;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPSlider();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdateHPSlider();
        if (currentHP <= 0)
        {
            
            NightManager.Instance.EndNightSequence(false, currentHP);
        }
    }

    void UpdateHPSlider()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
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
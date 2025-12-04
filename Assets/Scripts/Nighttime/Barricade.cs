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
        Debug.Log("Barricade HP: " + currentHP);
        UpdateHPSlider();
        if (currentHP <= 0)
        {
            Debug.Log("Barricade destroyed!");
            NightManager.Instance.RestartNight();
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

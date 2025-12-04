using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;

    [Header("UI")]
     public Image hpImage; 

    public float damageCooldown = 0.5f;
    float damageTimer = 0f;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPDisplay();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdateHPDisplay();
        if (currentHP <= 0)
        {
            
            NightManager.Instance.EndNightSequence(false, currentHP);
        }
    }

    void UpdateHPDisplay() 
    {
        if (hpImage != null)
        {
            
            float healthRatio = (float)Mathf.Max(0, currentHP) / maxHP;
            
            
            hpImage.fillAmount = healthRatio;
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
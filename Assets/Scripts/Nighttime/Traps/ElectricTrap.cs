using UnityEngine;

public class ElectricTrap : TrapBase
{
    public int damage = 999;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isActivated) return;
        if (!col.CompareTag("Enemy")) return;

        
        col.GetComponent<Enemy>().TakeDamage(damage);

        
        ActivateTrap();
    }
}

using UnityEngine;

public class BearTrap : TrapBase
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isActivated) return;
        if (!col.CompareTag("Enemy")) return;

        
        Enemy e = col.GetComponent<Enemy>();
        e.moveSpeed = 0;

       
        ActivateTrap();
    }
}

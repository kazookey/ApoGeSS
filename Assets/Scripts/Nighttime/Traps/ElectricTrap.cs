using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    public int damage = 999;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}

using UnityEngine;

public class BearTrap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Enemy e = col.GetComponent<Enemy>();
            e.moveSpeed = 0;
        }
    }
}

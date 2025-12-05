using UnityEngine;

public class BarricadePart : MonoBehaviour
{
    public Barricade root;

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Enemy"))
            return;

        if (root.CanTakeDamage())
        {
            root.TakeDamage(1);
            print(col.transform.name + " hit");
        }
    }
}

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Barricade mainBarricade;

    void Start()
    {
        GameObject barricadeObject = GameObject.FindGameObjectWithTag("Barricade");
        if (barricadeObject != null)
        {
            mainBarricade = barricadeObject.GetComponent<Barricade>();
        }

        if (mainBarricade == null)
        {
            Debug.LogError("PlayerHealth: Could not find Barricade object with tag 'Barricade'. Damage cannot be redirected.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (mainBarricade != null)
        {
            mainBarricade.TakeDamage(damage);
        }
      
    }

}
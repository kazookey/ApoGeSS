using UnityEngine;

// Class name must match the filename 'HiredDefender'
public class HiredDefender : MonoBehaviour
{
    public float fleeSpeed = 5f;
    private bool isFleeing = false;
    
    void Start()
    {
        // Register self with the Manager
        if (NightManager.Instance != null)
        {
            NightManager.Instance.RegisterDefender(this);
        }
    }

    void Update()
    {
        if (isFleeing)
        {
            // Move right continuously
            transform.Translate(Vector3.right * fleeSpeed * Time.unscaledDeltaTime);
        }
    }

    public void StartRunningAway()
    {
        isFleeing = true;
        Destroy(gameObject, 3f); 
    }
}
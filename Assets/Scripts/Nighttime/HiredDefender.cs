using UnityEngine;

public class Defender : MonoBehaviour
{
   

    public float fleeSpeed = 5f;
    private bool isFleeing = false;
    
    void Start()
    {
       
        NightManager.Instance.RegisterDefender(this);
    }

    void Update()
    {
        if (isFleeing)
        {
           
            transform.Translate(Vector3.right * fleeSpeed * Time.unscaledDeltaTime);
        }
        
    }

    
    
    public void Flee()
    {
        isFleeing = true;
        
        Destroy(gameObject, 3f); 
    }
}
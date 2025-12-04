using UnityEngine;

public class Defender : MonoBehaviour
{
   

    public float fleeSpeed = 5f;
    private bool isFleeing = false;
    public SpriteRenderer colorableSpritePart; 
    
    void Start()
    {
        
        if (colorableSpritePart != null)
            {
                colorableSpritePart.color = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
                );
            }

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
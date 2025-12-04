using UnityEngine;

public class HiredDefender : MonoBehaviour
{
   // daytime connector goes here
    [Header("Daytime Connection")]
    public string defenderID = "NPC_Defender_01"; 

    [Header("Visuals")]

    public SpriteRenderer colorableSpritePart; 
    public float runSpeed = 8f; 

    private Vector3 runDirection = new Vector3(1, 0, 0); 
    private bool isRunningAway = false;

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
        if (isRunningAway)
        {
            
            transform.position += runDirection * runSpeed * Time.deltaTime;
        }
    }

    // call this when the night is lost (barricade broke)
    public void StartRunningAway()
    {
       
        isRunningAway = true;
        
       
        Destroy(gameObject, 3f); 
    }
}
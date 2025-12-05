using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite activatedSprite;

    [Header("Trap Settings")]
    public bool destroyAfterActivation = false;
    public float destroyDelay = 0.1f;

    private SpriteRenderer sr;
    protected bool isActivated = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
    }

    
    public virtual void ActivateTrap()
    {
        if (isActivated) return;
        isActivated = true;

        if (activatedSprite != null)
            sr.sprite = activatedSprite;

        if (destroyAfterActivation)
            Destroy(gameObject, destroyDelay);
    }
}

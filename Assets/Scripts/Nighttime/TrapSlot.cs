using UnityEngine;

public class TrapSlot : MonoBehaviour
{
    public bool isOccupied = false;
    private SpriteRenderer highlight;

    void Awake()
    {
        highlight = GetComponentInChildren<SpriteRenderer>();
        highlight.enabled = false;
    }

    void OnMouseEnter()
    {
        if (!isOccupied && NightManager.Instance.currentState == NightState.Preparation)
            highlight.enabled = true;
    }

    void OnMouseExit()
    {
        highlight.enabled = false;
    }

    void OnMouseDown()
    {
       
        if (NightManager.Instance.currentState != NightState.Preparation)
            return;

       
        TrapPlacementManager.Instance.TryPlaceTrap(this);
    }
}

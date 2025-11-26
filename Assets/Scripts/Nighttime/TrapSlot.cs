using UnityEngine;

public class TrapSlot : MonoBehaviour
{
    public bool occupied = false;
    public GameObject currentTrap;

    public void PlaceTrap(GameObject trapPrefab)
    {
        if (occupied) return;

        currentTrap = Instantiate(trapPrefab, transform.position, Quaternion.identity);
        occupied = true;
    }
    
    void OnMouseDown()
{
    if (NightManager.Instance.currentState != NightState.Preparation)
        return;

    TrapPlacementManager.Instance.TryPlaceTrap(this);
}

}

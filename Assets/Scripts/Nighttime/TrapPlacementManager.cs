using UnityEngine;

public class TrapPlacementManager : MonoBehaviour
{
    public static TrapPlacementManager Instance;

    public GameObject selectedTrap;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTrap(GameObject trap)
    {
        if (NightManager.Instance.currentState != NightState.Preparation)
            return;

        selectedTrap = trap;
    }

    public void TryPlaceTrap(TrapSlot slot)
    {
        if (selectedTrap == null) return;

        slot.PlaceTrap(selectedTrap);
        selectedTrap = null;
    }
}

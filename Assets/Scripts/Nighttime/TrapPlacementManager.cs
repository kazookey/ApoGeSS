using UnityEngine;

public class TrapPlacementManager : MonoBehaviour
{
    public static TrapPlacementManager Instance;
    public void SelectBarrel(GameObject prefab)   { selectedTrap = prefab; }
    public void SelectBearTrap(GameObject prefab) { selectedTrap = prefab; }
    public void SelectBattery(GameObject prefab)  { selectedTrap = prefab; }

    public GameObject selectedTrap;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTrap(GameObject trapPrefab)
    {
        if (NightManager.Instance.currentState != NightState.Preparation)
            return;

        selectedTrap = trapPrefab;
    }

    public void TryPlaceTrap(TrapSlot slot)
    {
        if (selectedTrap == null) return;

        slot.PlaceTrap(selectedTrap);
        selectedTrap = null;
    }
}

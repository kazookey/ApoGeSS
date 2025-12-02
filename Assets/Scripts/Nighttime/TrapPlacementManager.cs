using UnityEngine;

public class TrapPlacementManager : MonoBehaviour
{
    public static TrapPlacementManager Instance;

    public GameObject barrelPrefab;
    public GameObject bearTrapPrefab;
    public GameObject batteryPrefab;

    private GameObject selectedTrap;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTrap(GameObject prefab)
    {
        selectedTrap = prefab;
    }

    public void TryPlaceTrap(TrapSlot slot)
    {
        if (slot.isOccupied || selectedTrap == null)
            return;

        
        Instantiate(selectedTrap, slot.transform.position, Quaternion.identity);
        slot.isOccupied = true;
    }
}

using UnityEngine;

public class TrapPlacementManager : MonoBehaviour
{
    public static TrapPlacementManager Instance;

    public GameObject barrelPrefab;
    public GameObject bearTrapPrefab;
    public GameObject batteryPrefab;

    [Header("UI Buttons to hide after use")]
    public GameObject barrelButtonUI;
    public GameObject bearTrapButtonUI;
    public GameObject batteryButtonUI;

    private GameObject selectedTrap;

    private int barrelsUsed = 0;
    private int bearTrapsUsed = 0;
    private int batteriesUsed = 0;
    public int maxUsePerNight = 1;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTrap(GameObject prefab)
    {
       
        if (prefab == barrelPrefab && barrelsUsed >= maxUsePerNight) return;
        if (prefab == bearTrapPrefab && bearTrapsUsed >= maxUsePerNight) return;
        if (prefab == batteryPrefab && batteriesUsed >= maxUsePerNight) return;

        selectedTrap = prefab;
    }

    public void TryPlaceTrap(TrapSlot slot)
    {
        if (slot.isOccupied || selectedTrap == null)
            return;

        Instantiate(selectedTrap, slot.transform.position, Quaternion.identity);
        slot.isOccupied = true;

       
        if (selectedTrap == barrelPrefab)
        {
            barrelsUsed++;
            if (barrelButtonUI != null) barrelButtonUI.SetActive(false);
        }

        if (selectedTrap == bearTrapPrefab)
        {
            bearTrapsUsed++;
            if (bearTrapButtonUI != null) bearTrapButtonUI.SetActive(false);
        }

        if (selectedTrap == batteryPrefab)
        {
            batteriesUsed++;
            if (batteryButtonUI != null) batteryButtonUI.SetActive(false);
        }

        selectedTrap = null; 
    }

    public void ResetForNextNight()
    {
        barrelsUsed = 0;
        bearTrapsUsed = 0;
        batteriesUsed = 0;

        if (barrelButtonUI != null) barrelButtonUI.SetActive(true);
        if (bearTrapButtonUI != null) bearTrapButtonUI.SetActive(true);
        if (batteryButtonUI != null) batteryButtonUI.SetActive(true);
    }
}

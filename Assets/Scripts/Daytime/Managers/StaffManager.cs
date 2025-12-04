using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaffManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject staffPanel;
    public TextMeshProUGUI availableText;   // "Unassigned: 1"
    public TextMeshProUGUI scoutCountText;  // "2"
    public TextMeshProUGUI defenderCountText; // "1"

    [Header("Buttons (Optional for linking)")]
    public Button closeButton;

    // Variables to track changes before saving
    private int currentScouts;
    private int currentDefenders;

    void Start()
    {
        if(staffPanel != null) staffPanel.SetActive(false);
        if(closeButton != null) closeButton.onClick.AddListener(CloseStaffMenu);
    }

    public void OpenStaffMenu()
    {
        // 1. Get fresh data from GameManager
        currentScouts = GameManager.Instance.assignedScouts;
        currentDefenders = GameManager.Instance.assignedDefenders;
        
        // 2. Show Panel
        staffPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseStaffMenu()
    {
        // 3. Save data BACK to GameManager
        GameManager.Instance.assignedScouts = currentScouts;
        GameManager.Instance.assignedDefenders = currentDefenders;
        
        staffPanel.SetActive(false);
    }

    // Link this to your [+] and [-] buttons
    public void ModifyScouts(int amount)
    {
        int unassigned = GetUnassignedCount();
        
        // Rules:
        // Can't add if nobody is available
        if (amount > 0 && unassigned <= 0) return;
        // Can't remove if we have 0 scouts
        if (amount < 0 && currentScouts <= 0) return;

        currentScouts += amount;
        UpdateUI();
    }

    public void ModifyDefenders(int amount)
    {
        int unassigned = GetUnassignedCount();

        if (amount > 0 && unassigned <= 0) return;
        if (amount < 0 && currentDefenders <= 0) return;

        currentDefenders += amount;
        UpdateUI();
    }

    int GetUnassignedCount()
    {
        return GameManager.Instance.totalSurvivors - (currentScouts + currentDefenders);
    }

    void UpdateUI()
    {
        if (availableText == null) return;

        // Visual feedback
        availableText.text = GetUnassignedCount().ToString();
        scoutCountText.text = currentScouts.ToString();
        defenderCountText.text = currentDefenders.ToString();
    }
}
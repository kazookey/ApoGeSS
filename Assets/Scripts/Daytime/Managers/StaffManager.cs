using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaffManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject staffPanel;
    public TextMeshProUGUI availableText; // "Unassigned: 1"
    public TextMeshProUGUI scoutCountText;
    public TextMeshProUGUI defenderCountText;

    // Local tracking before we commit to GameManager
    private int currentScouts;
    private int currentDefenders;

    void Start()
    {
        if(staffPanel != null) staffPanel.SetActive(false);
        UpdateUI();
    }

    public void OpenStaffMenu()
    {
        // Load current state
        currentScouts = GameManager.Instance.assignedScouts;
        currentDefenders = GameManager.Instance.assignedDefenders;
        
        staffPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseStaffMenu()
    {
        // Save changes to GameManager
        GameManager.Instance.assignedScouts = currentScouts;
        GameManager.Instance.assignedDefenders = currentDefenders;
        
        staffPanel.SetActive(false);
    }

    public void ModifyScouts(int amount)
    {
        int unassigned = GetUnassignedCount();
        
        // If adding, check if we have unassigned people
        if (amount > 0 && unassigned <= 0) return;
        
        // If removing, check if we have scouts
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

        availableText.text = $"Unassigned Staff: {GetUnassignedCount()}";
        scoutCountText.text = currentScouts.ToString();
        defenderCountText.text = currentDefenders.ToString();
    }
}
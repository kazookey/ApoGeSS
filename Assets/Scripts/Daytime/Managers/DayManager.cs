using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ensure you have TextMeshPro installed

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int maxTimeSlots = 4; // e.g., Morning, Noon, Afternoon, Evening
    public int currentTimeSlot = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI timeText; // Displays "Morning", "Afternoon"
    public Button endDayButton; // Only active when the day is over
    
    [Header("Dependencies")]
    public SalesManager salesManager;

    private string[] timeLabels = { "Morning", "Noon", "Afternoon", "Evening" };

    void Start()
    {
        UpdateTimeUI();
        endDayButton.interactable = false; // Cannot leave early
    }

    // Call this function when an action (Negotiation, Restock) is finished
    public void AdvanceTime(int cost = 1)
    {
        currentTimeSlot += cost;

        if (currentTimeSlot >= maxTimeSlots)
        {
            currentTimeSlot = maxTimeSlots;
            EndOfDayReached();
        }
        
        if (salesManager != null)
        {
            salesManager.ProcessPassiveSales();
        }

        if (currentTimeSlot >= maxTimeSlots)

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        if (currentTimeSlot < timeLabels.Length)
            timeText.text = timeLabels[currentTimeSlot];
        else
            timeText.text = "Night Fall";
    }

    void EndOfDayReached()
    {
        Debug.Log("The sun has set. Prepare for night.");
        endDayButton.interactable = true;
    }

    // Link this to your "End Day" button in the Inspector
    public void GoToPreparationPhase()
    {
        // Add scene transition logic here later
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nighttime"); 
    }
}
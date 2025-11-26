using UnityEngine;
using UnityEngine.UI; // For Image/Text UI
using TMPro; // Assuming you use TextMeshPro

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int maxTimeSlots = 4; // e.g., Morning, Noon, Afternoon, Evening
    public int currentTimeSlot = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI timeText; // Displays "Morning", "Afternoon", etc.
    public Button endDayButton; // Only active when time is out

    private string[] timeLabels = { "Morning", "Noon", "Afternoon", "Evening" };

    void Start()
    {
        UpdateTimeUI();
        endDayButton.interactable = false; // Cannot leave early (optional)
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
        // You might auto-trigger the transition here if you prefer
    }

    // Linked to the "End Day" button
    public void GoToPreparationPhase()
    {
        // Save logic here if needed
        UnityEngine.SceneManagement.SceneManager.LoadScene("PreparationScene"); // Or whatever Ramon names it
    }
}
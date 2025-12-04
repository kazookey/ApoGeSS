using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement; // Needed for scene reloading if we test loop

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int maxTimeSlots = 4;
    public int currentTimeSlot = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI creditsText;
    public Button endDayButton; 
    
    [Header("Morning Report UI")]
    public GameObject reportPanel;        // The Popup Window
    public TextMeshProUGUI reportText;    // The text body inside
    public Button closeReportButton;      // "OK" button
    
    [Header("Dependencies")]
    public SalesManager salesManager;

    private string[] timeLabels = { "Morning", "Noon", "Afternoon", "Evening" };
    private int displayedCredits = -1; 

    void Start()
    {
        UpdateTimeUI();
        endDayButton.interactable = false;
        
        // Setup Report Button
        if(closeReportButton != null) closeReportButton.onClick.AddListener(CloseReport);
        if(reportPanel != null) reportPanel.SetActive(false);

        // 1. Check if GameManager has a report waiting from the Night
        if (!string.IsNullOrEmpty(GameManager.Instance.pendingMorningReport))
        {
            ShowMorningReport(GameManager.Instance.pendingMorningReport);
        }
    }
    
    void Update()
    {
        // Credit Counter Logic
        if (creditsText != null)
        {
            int actualCredits = GameManager.Instance.credits;
            if (displayedCredits != actualCredits)
            {
                displayedCredits = actualCredits;
                creditsText.text = $"Credits: ${actualCredits}";
            }
        }

        // --- DEBUG TESTING ---
        // Press 'M' to simulate a Morning Report instantly
        if (Application.isEditor && Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Simulating Morning...");
            GameManager.Instance.assignedScouts = 2; // Pretend we sent 2 people
            GameManager.Instance.ProcessMorningResults(); // Roll dice
            ShowMorningReport(GameManager.Instance.pendingMorningReport); // Show UI
        }
    }

    public void ShowMorningReport(string message)
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            reportText.text = message;
            
            // Clear the message so it doesn't show again next time
            GameManager.Instance.pendingMorningReport = ""; 
        }
    }

    public void CloseReport()
    {
        if (reportPanel != null) reportPanel.SetActive(false);
    }

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

    public void GoToPreparationPhase()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nighttime"); 
    }
}
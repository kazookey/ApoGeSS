using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
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
    
    [Header("Effects")]
    public GameObject moneyPopupPrefab; // Drag your prefab here
    public Transform popupSpawnPoint;   // Where the text appears (usually over the credits)
    private int lastCreditsValue = -1;  // To track changes
    
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
        
        lastCreditsValue = GameManager.Instance.credits;
    }
    
    void Update()
    {
        // 1. Check for Money CHANGES (Logic)
        if (GameManager.Instance.credits != lastCreditsValue)
        {
            int diff = GameManager.Instance.credits - lastCreditsValue;
            SpawnMoneyPopup(diff);
            lastCreditsValue = GameManager.Instance.credits;
        }

        // 2. Update the Visual Counter (Animation)
        if (creditsText != null)
        {
            // If the visual number hasn't caught up to the real number yet...
            if (displayedCredits != GameManager.Instance.credits)
            {
                // First frame init
                if (displayedCredits == -1) displayedCredits = GameManager.Instance.credits;
                
                // DOTween the number for the "ticking" effect
                DOTween.To(() => displayedCredits, x => displayedCredits = x, GameManager.Instance.credits, 0.5f)
                    .OnUpdate(() => creditsText.text = $"Credits: ${displayedCredits}");
            }
        }
        
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
    
    void SpawnMoneyPopup(int amount)
    {
        if (moneyPopupPrefab != null && popupSpawnPoint != null)
        {
            GameObject popup = Instantiate(moneyPopupPrefab, popupSpawnPoint);
            // Reset scale just in case
            popup.transform.localScale = Vector3.one; 
            
            MoneyPopup script = popup.GetComponent<MoneyPopup>();
            if (script != null) script.Setup(amount);
        }
    }
}
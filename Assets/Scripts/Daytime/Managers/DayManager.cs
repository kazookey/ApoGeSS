using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // For Animations
using UnityEngine.SceneManagement; // For Scene Loading

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int maxTimeSlots = 4;
    public int currentTimeSlot = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI creditsText;
    public Button endDayButton;       // Button to start Night
    public Button nextEventButton;    // Button to trigger customers
    
    [Header("Effects (Juice)")]
    public GameObject moneyPopupPrefab; // The floating text prefab
    public Transform popupSpawnPoint;   // Position near the credits text
    private int lastCreditsValue = -1;  // Tracks changes to detect spending/earning
    private int displayedCredits = -1;  // Used for the ticking animation
    
    [Header("Morning Report UI")]
    public GameObject reportPanel;        
    public TextMeshProUGUI reportText;    
    public Button closeReportButton;      
    
    [Header("Dependencies")]
    public SalesManager salesManager;

    private string[] timeLabels = { "Morning", "Noon", "Afternoon", "Evening" };

    void Start()
    {
        UpdateTimeUI();
        
        // 1. Initialize Buttons
        if(endDayButton != null) endDayButton.interactable = false;
        if(nextEventButton != null) nextEventButton.interactable = true;
        
        // 2. Setup Report UI
        if(closeReportButton != null) closeReportButton.onClick.AddListener(CloseReport);
        if(reportPanel != null) reportPanel.SetActive(false);

        // 3. Check for a pending report from Night (Scouts/Survival)
        if (!string.IsNullOrEmpty(GameManager.Instance.pendingMorningReport))
        {
            ShowMorningReport(GameManager.Instance.pendingMorningReport);
        }
        
        // 4. Init Credit Tracking (prevent popup on start)
        lastCreditsValue = GameManager.Instance.credits;
    }
    
    void Update()
    {
        // --- MONEY POPUP LOGIC ---
        if (GameManager.Instance.credits != lastCreditsValue)
        {
            int diff = GameManager.Instance.credits - lastCreditsValue;
            SpawnMoneyPopup(diff);
            lastCreditsValue = GameManager.Instance.credits;
        }

        // --- TICKING COUNTER LOGIC ---
        if (creditsText != null)
        {
            int actualCredits = GameManager.Instance.credits;
            if (displayedCredits != actualCredits)
            {
                // First frame: Set instantly
                if (displayedCredits == -1) 
                {
                    displayedCredits = actualCredits;
                    creditsText.text = $"Credits: ${displayedCredits}";
                }
                else
                {
                    // Animation: Count up/down over 0.5 seconds
                    // We assume this runs frequently enough that creating a tween here is fine,
                    // but for heavy optimization, you'd kill previous tweens first.
                    DOTween.To(() => displayedCredits, x => displayedCredits = x, actualCredits, 0.5f)
                        .OnUpdate(() => creditsText.text = $"Credits: ${displayedCredits}");
                }
            }
        }

        // --- DEBUG: Test Morning Report ---
        if (Application.isEditor && Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Debug: Simulating Morning...");
            GameManager.Instance.assignedScouts = 2; 
            GameManager.Instance.ProcessMorningResults(); 
            ShowMorningReport(GameManager.Instance.pendingMorningReport);
        }
    }

    // --- TIME SYSTEM ---

    public void AdvanceTime(int cost = 1)
    {
        // If already night, do nothing
        if (currentTimeSlot >= maxTimeSlots) return;

        currentTimeSlot += cost;

        // Process Passive Sales (Money from shelves)
        if (salesManager != null)
        {
            salesManager.ProcessPassiveSales();
        }

        // Check if Day is Over
        if (currentTimeSlot >= maxTimeSlots)
        {
            currentTimeSlot = maxTimeSlots;
            EndOfDayReached();
        }
        
        UpdateTimeUI(); 
    }

    void UpdateTimeUI()
    {
        if (timeText == null) return;

        if (currentTimeSlot < timeLabels.Length)
            timeText.text = timeLabels[currentTimeSlot];
        else
            timeText.text = "Night Fall";
    }

    void EndOfDayReached()
    {
        Debug.Log("The sun has set. Prepare for night.");
        
        // Disable Event Button (Can't trade anymore)
        if(nextEventButton != null) nextEventButton.interactable = false;
        
        // Enable End Day Button (Can go to Night now)
        if(endDayButton != null) endDayButton.interactable = true;
    }

    // Called by "End Day" Button
    public void GoToPreparationPhase()
    {
        SceneManager.LoadScene("Nighttime"); 
    }

    // --- REPORT SYSTEM ---

    public void ShowMorningReport(string message)
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            reportText.text = message;
            
            // Clear message so it doesn't appear again if we reload scene
            GameManager.Instance.pendingMorningReport = ""; 
        }
    }

    public void CloseReport()
    {
        if (reportPanel != null) reportPanel.SetActive(false);
    }
    
    // --- EFFECTS SYSTEM ---

    void SpawnMoneyPopup(int amount)
    {
        if (moneyPopupPrefab != null && popupSpawnPoint != null)
        {
            GameObject popup = Instantiate(moneyPopupPrefab, popupSpawnPoint);
            // Reset scale to 1 (sometimes UI prefabs spawn with weird scales)
            popup.transform.localScale = Vector3.one; 
            
            MoneyPopup script = popup.GetComponent<MoneyPopup>();
            if (script != null) script.Setup(amount);
        }
    }
}
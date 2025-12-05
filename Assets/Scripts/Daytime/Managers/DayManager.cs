using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Needed for DOTween
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public int maxTimeSlots = 4;
    public int currentTimeSlot = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI creditsText;
    public Button endDayButton; 
    public Button nextEventButton;
    
    [Header("Effects")]
    public GameObject moneyPopupPrefab;
    public Transform popupSpawnPoint;
    private int lastCreditsValue = -1;
    private int displayedCredits = -1;
    
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
        
        if(endDayButton != null) endDayButton.interactable = false;
        if(nextEventButton != null) nextEventButton.interactable = true;
        
        if(closeReportButton != null) closeReportButton.onClick.AddListener(CloseReport);
        if(reportPanel != null) reportPanel.SetActive(false);

        if (!string.IsNullOrEmpty(GameManager.Instance.pendingMorningReport))
        {
            ShowMorningReport(GameManager.Instance.pendingMorningReport);
        }
        
        lastCreditsValue = GameManager.Instance.credits;
    }
    
    void Update()
    {
        if (GameManager.Instance.credits != lastCreditsValue)
        {
            int diff = GameManager.Instance.credits - lastCreditsValue;
            SpawnMoneyPopup(diff);
            lastCreditsValue = GameManager.Instance.credits;
        }

        if (creditsText != null)
        {
            int actualCredits = GameManager.Instance.credits;
            if (displayedCredits != actualCredits)
            {
                if (displayedCredits == -1) displayedCredits = actualCredits;
                DOTween.To(() => displayedCredits, x => displayedCredits = x, actualCredits, 0.5f)
                    .OnUpdate(() => creditsText.text = $"Credits: ${displayedCredits}");
            }
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Debug: Simulating Morning...");
            GameManager.Instance.assignedScouts = 2; 
            GameManager.Instance.ProcessMorningResults(); 
            ShowMorningReport(GameManager.Instance.pendingMorningReport);
        }
    }

    public void ShowMorningReport(string message)
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            reportText.text = message;
            
            // --- ANIMATION START ---
            reportPanel.transform.localScale = Vector3.zero;
            reportPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            // -----------------------
            
            GameManager.Instance.pendingMorningReport = ""; 
        }
    }

    public void CloseReport()
    {
        // --- ANIMATION END ---
        if (reportPanel != null)
        {
            reportPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
            {
                reportPanel.SetActive(false);
            });
        }
    }

    // ... (Keep AdvanceTime, UpdateTimeUI, EndOfDayReached, GoToPreparationPhase, SpawnMoneyPopup SAME as before) ...
    public void AdvanceTime(int cost = 1)
    {
        if (currentTimeSlot >= maxTimeSlots) return;
        currentTimeSlot += cost;
        if (salesManager != null) salesManager.ProcessPassiveSales();
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
        if (currentTimeSlot < timeLabels.Length) timeText.text = timeLabels[currentTimeSlot];
        else timeText.text = "Night Fall";
    }

    void EndOfDayReached()
    {
        Debug.Log("The sun has set. Prepare for night.");
        if(endDayButton != null) endDayButton.interactable = true;
        if(nextEventButton != null) nextEventButton.interactable = false;
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
            popup.transform.localScale = Vector3.one; 
            MoneyPopup script = popup.GetComponent<MoneyPopup>();
            if (script != null) script.Setup(amount);
        }
    }
}
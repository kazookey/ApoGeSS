using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum NightState
{
    Preparation,
    Wave,
    Finished, 
    GameOver, 
    Victory   
}

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    [Header("UI References")]
    public GameObject startNightButton;
    public GameObject trapUI;
    
    // Ramon's Timer UI
    public TextMeshProUGUI timerText;
    public GameObject timerContainer;
    
    // Ramon's End Screen
    public GameObject endNightPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI endButtonText;
    
    [Header("Defender Spawning")]
    public GameObject defenderPrefab;       
    public Transform[] defenderSpawnPoints; 

    [Header("Night Settings")]
    public float nightdurationSeconds = 300f; 
    public NightState currentState = NightState.Preparation;
    private Coroutine timerCoroutine;

    private List<HiredDefender> activeDefenders = new List<HiredDefender>();
    
    private PlayerMovement playerMovement;
    private Barricade mainBarricade;

    void Awake()
    {
        Instance = this;
        if (timerContainer != null) timerContainer.SetActive(false);
        if (endNightPanel != null) endNightPanel.SetActive(false);

        // Find Scene References
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) playerMovement = playerObject.GetComponent<PlayerMovement>();
        
        GameObject barricadeObject = GameObject.FindGameObjectWithTag("Barricade");
        if (barricadeObject != null) mainBarricade = barricadeObject.GetComponent<Barricade>();
        
        Time.timeScale = 1f;
    }
    
    void Start()
    {
        // 1. SPAWN DEFENDERS
        SpawnDefenders();

        // 2. Load Barricade HP
        // FIX: Now uses 'currentHealth' (float) to match Barricade.cs
        if (mainBarricade != null)
        {
            if (GameManager.Instance != null)
                mainBarricade.currentHealth = GameManager.Instance.barricadeHealth;
            else
                mainBarricade.currentHealth = 100f; 
                
            mainBarricade.maxHealth = 100f; 
        }
    }

    void SpawnDefenders()
    {
        if (GameManager.Instance == null) return;

        int count = GameManager.Instance.assignedDefenders;
        
        if (defenderSpawnPoints == null || defenderSpawnPoints.Length == 0) return;

        int limit = Mathf.Min(count, defenderSpawnPoints.Length);

        for (int i = 0; i < limit; i++)
        {
            if (defenderPrefab != null && defenderSpawnPoints[i] != null)
            {
                Instantiate(defenderPrefab, defenderSpawnPoints[i].position, Quaternion.identity);
            }
        }
        Debug.Log($"Spawning {limit} Defenders.");
    }

    public void RegisterDefender(HiredDefender defender)
    {
        activeDefenders.Add(defender);
    }

    public void OnStartNightButton()
    {
        if (currentState != NightState.Preparation) return;

        currentState = NightState.Wave;
        
        if (WaveManager.Instance != null) WaveManager.Instance.BeginWaves();

        timerCoroutine = StartCoroutine(NightTimer());
        if (timerContainer != null) timerContainer.SetActive(true);

        if (startNightButton != null) startNightButton.SetActive(false);
        if (trapUI != null) trapUI.SetActive(false);
    }

    IEnumerator NightTimer()
    {
        float timeLeft = nightdurationSeconds;

        while (timeLeft > 0 && currentState == NightState.Wave)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerUI(timeLeft);
            yield return null;
        }

        if (currentState == NightState.Wave)
        {
            timeLeft = 0; 
            UpdateTimerUI(timeLeft);
            // FIX: Uses float 'currentHealth'
            EndNightSequence(true, mainBarricade != null ? mainBarricade.currentHealth : 0f); 
        }
    }

    void UpdateTimerUI(float timeToDisplay)
    {
        if (timerText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // FIX: Changed finalBarricadeHP to float
    public void EndNightSequence(bool victory, float finalBarricadeHP)
    {
        if (currentState == NightState.GameOver || currentState == NightState.Victory) return;
        
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (WaveManager.Instance != null) WaveManager.Instance.StopSpawning();

        Time.timeScale = 0f;
        
        if (playerMovement != null) playerMovement.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (victory)
        {
            currentState = NightState.Victory;
            if (WaveManager.Instance != null) WaveManager.Instance.DespawnAllEnemies();
            
            if(resultText != null) resultText.text = "YOU SURVIVED!";
            if(endButtonText != null) endButtonText.text = "NEXT DAY";
            
            // --- SYNC WITH GAMEMANAGER ---
            if (GameManager.Instance != null)
            {
                GameManager.Instance.barricadeHealth = finalBarricadeHP; // Save Float HP
                GameManager.Instance.ProcessMorningResults(); 
                GameManager.Instance.AdvanceDay(); 
            }
        }
        else 
        {
            currentState = NightState.GameOver;
            if(resultText != null) resultText.text = "YOU GOT OVERRUN!";
            if(endButtonText != null) endButtonText.text = "RETRY NIGHT";

            foreach (var defender in activeDefenders)
            {
                if (defender != null) defender.StartRunningAway(); 
            }
            activeDefenders.Clear();
            
            if (GameManager.Instance != null) GameManager.Instance.barricadeHealth = 0;
        }
        
        if (timerContainer != null) timerContainer.SetActive(false);
        if (endNightPanel != null) endNightPanel.SetActive(true);
    }
    
    public void OnEndNightButton()
    {
        Time.timeScale = 1f;
        
        if (currentState == NightState.Victory)
        {
            SceneManager.LoadScene("Daytime"); 
        }
        else if (currentState == NightState.GameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    public bool CanPlayerShoot()
    {
        return currentState == NightState.Wave;
    }
}
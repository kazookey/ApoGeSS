using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening; // Added for Animations

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
    
    [Header("Defender Spawning (Rein's Logic)")]
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

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) playerMovement = playerObject.GetComponent<PlayerMovement>();
        
        GameObject barricadeObject = GameObject.FindGameObjectWithTag("Barricade");
        if (barricadeObject != null) mainBarricade = barricadeObject.GetComponent<Barricade>();
        
        Time.timeScale = 1f;
    }
    
    void Start()
    {
        SpawnDefenders();

        if (mainBarricade != null)
        {
            if (GameManager.Instance != null)
                mainBarricade.currentHealth = GameManager.Instance.barricadeHealth;
            else
                mainBarricade.currentHealth = 100f; 
                
            mainBarricade.maxHealth = 100f; 
        }

        // --- ANIMATION: Intro ---
        // Make the Start button and Trap UI pop in
        if (startNightButton != null)
        {
            startNightButton.transform.localScale = Vector3.zero;
            startNightButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
        if (trapUI != null)
        {
            trapUI.transform.localScale = Vector3.zero;
            trapUI.transform.DOScale(new Vector3(0.24f, 0.16f, 0.20f), 0.5f).SetEase(Ease.OutBack).SetDelay(0.1f);
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
        
        // --- ANIMATION: Show Timer ---
        if (timerContainer != null) 
        {
            timerContainer.SetActive(true);
            timerContainer.transform.localScale = Vector3.zero;
            timerContainer.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        // --- ANIMATION: Hide Prep UI ---
        if (startNightButton != null) 
            startNightButton.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => startNightButton.SetActive(false));
            
        if (trapUI != null) 
            trapUI.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => trapUI.SetActive(false));
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

    public void EndNightSequence(bool victory, float finalBarricadeHP)
    {
        if (currentState == NightState.GameOver || currentState == NightState.Victory) return;
        
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (WaveManager.Instance != null) WaveManager.Instance.StopSpawning();

        Time.timeScale = 0f; // PAUSE GAME
        
        if (playerMovement != null) playerMovement.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (victory)
        {
            currentState = NightState.Victory;
            if (WaveManager.Instance != null) WaveManager.Instance.DespawnAllEnemies();
            
            if(resultText != null) resultText.text = "YOU SURVIVED!";
            if(endButtonText != null) endButtonText.text = "NEXT DAY";
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.barricadeHealth = finalBarricadeHP;
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
        
        // --- ANIMATION: Hide Timer ---
        if (timerContainer != null)
        {
             // Use SetUpdate(true) because TimeScale is 0!
             timerContainer.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => 
             {
                 timerContainer.SetActive(false);
             });
        }
        
        // --- ANIMATION: Show Results ---
        if (endNightPanel != null) 
        {
            endNightPanel.SetActive(true);
            endNightPanel.transform.localScale = Vector3.zero;
            // CRITICAL: .SetUpdate(true) makes it animate even when paused
            endNightPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }
    
    public void OnEndNightButton()
    {
        Time.timeScale = 1f; // Unpause before leaving!
        
        // --- ANIMATION: Close Panel ---
        if (endNightPanel != null)
        {
            endNightPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() => 
            {
                if (currentState == NightState.Victory)
                {
                    SceneTransitionManager.Instance.LoadNightScene();
                }
                else if (currentState == NightState.GameOver)
                {
                    SceneTransitionManager.Instance.LoadNightScene();
                }
            });
        }
        else
        {
            // Fallback if panel is missing
            if (currentState == NightState.Victory) SceneTransitionManager.Instance.LoadNightScene();
            else SceneTransitionManager.Instance.LoadNightScene();
        }
    }

    public bool CanPlayerShoot()
    {
        return currentState == NightState.Wave;
    }
}
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI timerText;
    public GameObject timerContainer;
    
    [Header("End Night UI")]
    public GameObject endNightPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI endButtonText;
    
    [Header("Night Settings")]
    public float nightdurationSeconds = 300f; 
    public NightState currentState = NightState.Preparation;
    private Coroutine timerCoroutine;

    [Header("Defender Management")]
 
    private System.Collections.Generic.List<Defender> activeDefenders = new System.Collections.Generic.List<Defender>();
    
    private PlayerMovement playerMovement;
    private Barricade mainBarricade;

    void Awake()
    {
        Instance = this;
        if (timerContainer != null)
            timerContainer.SetActive(false);
            
        if (endNightPanel != null)
            endNightPanel.SetActive(false);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }
        
        GameObject barricadeObject = GameObject.FindGameObjectWithTag("Barricade");
        if (barricadeObject != null)
        {
            mainBarricade = barricadeObject.GetComponent<Barricade>();
        }
        
        Time.timeScale = 1f;
    }

    public bool CanPlayerShoot()
    {
        return currentState == NightState.Wave;
    }

    public void OnStartNightButton()
    {
        if (currentState != NightState.Preparation) return;

        currentState = NightState.Wave;
        WaveManager.Instance.BeginWaves();

        timerCoroutine = StartCoroutine(NightTimer());

        if (timerContainer != null)
            timerContainer.SetActive(true);

        if (startNightButton != null) startNightButton.SetActive(false);
        if (trapUI != null) trapUI.SetActive(false);
        
        // --- LOAD INITIAL BARRICADE HEALTH FROM PLAYERPREFS ---
        if (mainBarricade != null)
        {
            // Load previous HP, default to 100 if never set (new game/day 1)
            int initialHP = PlayerPrefs.GetInt("BarricadeHP", 100); 
            mainBarricade.currentHP = initialHP;
            mainBarricade.maxHP = initialHP; 
        }
        
    }

    IEnumerator NightTimer()
    {
        float timeLeft = nightdurationSeconds;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerUI(timeLeft);
            yield return null;
        }

        timeLeft = 0; 
        UpdateTimerUI(timeLeft);
        
        // Victory! Timer ran out. Pass current HP.
        EndNightSequence(true, mainBarricade != null ? mainBarricade.currentHP : 0); 
    }

    void UpdateTimerUI(float timeToDisplay)
    {
        float timeElapsed = nightdurationSeconds - timeToDisplay;
        float totalGameMinutes = 8f * 60f; 
        float progressNormalized = timeElapsed / nightdurationSeconds;
        float gameTimeMinutesPassed = progressNormalized * totalGameMinutes;

        float gameStartMinutes = 22f * 60f; 
        float totalCurrentGameMinutes = gameStartMinutes + gameTimeMinutesPassed;
        float minutesInDay = 24f * 60f;
        totalCurrentGameMinutes %= minutesInDay;

        int displayHours = Mathf.FloorToInt(totalCurrentGameMinutes / 60f);
        int displayMinutes = Mathf.FloorToInt(totalCurrentGameMinutes % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", displayHours, displayMinutes);
    }

    public void EndNightSequence(bool victory, int finalBarricadeHP)
    {
        if (currentState == NightState.GameOver || currentState == NightState.Victory) return;
        
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        WaveManager.Instance.StopSpawning();

        Time.timeScale = 0f;

        if (playerMovement != null) playerMovement.enabled = false;
        
        
        if (victory)
        {
            currentState = NightState.Victory;
            WaveManager.Instance.DespawnAllEnemies();
            
            resultText.text = "YOU SURVIVED!";
            endButtonText.text = "END NIGHT";
            
            // --- SAVE VICTORY DATA TO PLAYERPREFS ---
            PlayerPrefs.SetInt("BarricadeHP", finalBarricadeHP);
            PlayerPrefs.SetInt("Guards", activeDefenders.Count);
            // Assuming 0 for now until ammo tracking is implemented
            PlayerPrefs.SetInt("AmmoAmount", 0); 
            // --- END SAVE ---
        }
        else // Loss
        {
            currentState = NightState.GameOver;
            
            resultText.text = "YOU GOT OVERRUN!";
            endButtonText.text = "RETRY NIGHT";

            
            foreach (var defender in activeDefenders)
            {
                if (defender != null)
                {
                    
                    defender.Flee(); 
                }
            }
            
            
            PlayerPrefs.SetInt("BarricadeHP", 0); 
            PlayerPrefs.SetInt("Guards", 0); 
            PlayerPrefs.SetInt("AmmoAmount", 0); 
            
            
            activeDefenders.Clear();
        }
        
        PlayerPrefs.Save(); 

        if (timerContainer != null) timerContainer.SetActive(false);
        if (endNightPanel != null) endNightPanel.SetActive(true);
    }
    
    public void OnEndNightButton()
    {
        Time.timeScale = 1f;
        
        if (currentState == NightState.Victory)
        {
            SceneManager.LoadScene("DayScene"); 
        }
        else if (currentState == NightState.GameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    
    public void RegisterDefender(Defender defender)
    {
        activeDefenders.Add(defender);
    }
}
using UnityEngine;
using System.Collections;
using TMPro;

public enum NightState
{
    Preparation,
    Wave,
    Finished, 
    Restarting
}

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    [Header("UI References")]
    public GameObject startNightButton;
    public GameObject trapUI;
    public TextMeshProUGUI timerText;
    public GameObject timerContainer;
    [Header("Night Settings")]
    public float nightdurationSeconds = 300f; // 5 min default
    public NightState currentState = NightState.Preparation;
    private Coroutine timerCoroutine;

     [Header("Defender Management")]
    private System.Collections.Generic.List<HiredDefender> activeDefenders = new System.Collections.Generic.List<HiredDefender>();
    void Awake()
    {
        Instance = this;
        if (timerContainer != null)
            timerContainer.SetActive(false);
    }

    public bool CanPlayerShoot()
    {
        return currentState == NightState.Wave;
    }

    public void StartWave()
    {
        currentState = NightState.Wave;
        WaveManager.Instance.BeginWaves();
        timerCoroutine = StartCoroutine(NightTimer());

        if (timerContainer != null)
            timerContainer.SetActive(true);
    }

    public void EndNight()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        WaveManager.Instance.StopSpawning();

        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        if (timerContainer != null)
            timerContainer.SetActive(false);

        currentState = NightState.Finished;
        Debug.Log("Night Finished!");
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
            
            EndNight(); // night sruvived
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

    public void RegisterDefender(HiredDefender defender)
    {
        activeDefenders.Add(defender);
    }

    public void RestartNight()
    {

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
       
        currentState = NightState.Restarting;
        
       
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.StopSpawning();
        }
        
        
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        foreach (var defender in activeDefenders)
        {
            if (defender != null)
            {
                defender.StartRunningAway();
            }
        }
        activeDefenders.Clear();

        if (timerContainer != null)
            timerContainer.SetActive(false);
        
        Debug.Log("Loss condition met. Prompting player to restart the night!");
        
        // restart night ui screen here
    }
    
}

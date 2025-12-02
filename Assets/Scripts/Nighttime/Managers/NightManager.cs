using UnityEngine;

public enum NightState
{
    Preparation,
    Wave,
    Finished
}

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    [Header("UI References")]
    public GameObject startNightButton;
    public GameObject trapUI;

    public NightState currentState = NightState.Preparation;

    void Awake()
    {
        Instance = this;
    }

    public bool CanPlayerShoot()
    {
        return currentState == NightState.Wave;
    }

    public void StartWave()
    {
        currentState = NightState.Wave;
        WaveManager.Instance.BeginWaves();
    }

    public void EndNight()
    {
        currentState = NightState.Finished;
        Debug.Log("Night Finished!");
    }

    public void OnStartNightButton()
    {
        if (currentState != NightState.Preparation) return;

        currentState = NightState.Wave;
        WaveManager.Instance.BeginWaves();

        if (startNightButton != null) startNightButton.SetActive(false);
        if (trapUI != null) trapUI.SetActive(false);
    }
}

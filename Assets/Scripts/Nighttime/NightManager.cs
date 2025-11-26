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

    public NightState currentState = NightState.Preparation;

    void Awake()
    {
        Instance = this;
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
}

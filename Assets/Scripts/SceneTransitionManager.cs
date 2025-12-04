using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

   
    private const string DaySceneName = "Daytime";
    private const string NightSceneName = "Nighttime";
   
    private const string MainMenuSceneName = "MainMenuScene"; 

    // PlayerPrefs Keys (matching those in NightManager)
    private const string BarricadeHPKey = "BarricadeHP";
    private const string GuardsKey = "Guards";
    private const string AmmoAmountKey = "AmmoAmount";
    private const string IsInitializedKey = "IsGameInitialized";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
          
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       
        if (PlayerPrefs.GetInt(IsInitializedKey, 0) == 0)
        {
            InitializeGameData();
        }
    }

  
    private void InitializeGameData()
    {
        Debug.Log("Initializing fresh game data (Day 1 defaults).");
        
        
        PlayerPrefs.SetInt(BarricadeHPKey, 100); 
        PlayerPrefs.SetInt(GuardsKey, 0);       
        PlayerPrefs.SetInt(AmmoAmountKey, 0);   
        
     
        PlayerPrefs.SetInt(IsInitializedKey, 1);
        PlayerPrefs.Save();
    }

   
    public void StartNewGame()
    {
        
        InitializeGameData(); 
        LoadDayScene();
    }
    
   
    public void LoadNightScene()
    {
        
        SceneManager.LoadScene(NightSceneName);
    }

   
    public void LoadDayScene()
    {

        SceneManager.LoadScene(DaySceneName);
    }
}
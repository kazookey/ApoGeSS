using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Scene Names")]
    private const string DaySceneName = "Daytime";
    private const string NightSceneName = "Nighttime";
    private const string MainMenuSceneName = "MainMenuScene";

    [Header("PlayerPrefs Keys")]
    private const string BarricadeHPKey = "BarricadeHP";
    private const string GuardsKey = "Guards";
    private const string AmmoAmountKey = "AmmoAmount";
    private const string IsInitializedKey = "IsGameInitialized";

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    private Image fadeImage;
    private Canvas fadeCanvas;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateFadeCanvas();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize game data if needed
        if (PlayerPrefs.GetInt(IsInitializedKey, 0) == 0)
        {
            InitializeGameData();
        }
    }

    #region Game Initialization
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
    #endregion

    #region Scene Loading
    private void CreateFadeCanvas()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("SceneLoaderCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 1000;

        // Add CanvasScaler (optional) and CanvasGroup
        canvasGO.AddComponent<CanvasScaler>();
        CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false; // Make it non-interactable
        cg.interactable = false;

        DontDestroyOnLoad(canvasGO);

        // Create Image
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        // Fullscreen
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }


    public void LoadDayScene()
    {
        LoadScene(DaySceneName);
    }

    public void LoadNightScene()
    {
        LoadScene(NightSceneName);
    }

    public void LoadMainMenu()
    {
        LoadScene(MainMenuSceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade to black
        yield return fadeImage.DOFade(1f, fadeDuration).SetUpdate(true).WaitForCompletion();

        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Fade back in
        yield return fadeImage.DOFade(0f, fadeDuration).SetUpdate(true).WaitForCompletion();
    }
    #endregion
}

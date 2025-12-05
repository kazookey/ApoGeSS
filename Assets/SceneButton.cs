using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [Header("Scene to Load")]
    public string sceneName;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("SceneButton requires a Button component.");
            return;
        }

        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneButton: Scene name is empty.");
            return;
        }

        // Call the SceneTransitionManager to load the scene with fade
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager instance not found in the scene.");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StaffManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject staffPanel;
    public TextMeshProUGUI availableText;
    public TextMeshProUGUI scoutCountText;
    public TextMeshProUGUI defenderCountText;

    [Header("Buttons")]
    public Button toggleButton;

    private int currentScouts;
    private int currentDefenders;

    void Start()
    {
        if (staffPanel != null) staffPanel.SetActive(false);
        if (toggleButton != null) toggleButton.onClick.AddListener(ToggleStaffMenu);
    }

    public void ToggleStaffMenu()
    {
        if (staffPanel.activeSelf)
            CloseStaffMenu();
        else
            OpenStaffMenu();
    }

    void OpenStaffMenu()
    {
        currentScouts = GameManager.Instance.assignedScouts;
        currentDefenders = GameManager.Instance.assignedDefenders;

        staffPanel.SetActive(true);

        // --- ANIMATION ---
        staffPanel.transform.localScale = Vector3.zero;
        staffPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        UpdateUI();
    }

    public void CloseStaffMenu()
    {
        GameManager.Instance.assignedScouts = currentScouts;
        GameManager.Instance.assignedDefenders = currentDefenders;

        // --- ANIMATION ---
        staffPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            staffPanel.SetActive(false);
        });
    }

    public void ModifyScouts(int amount)
    {
        int unassigned = GetUnassignedCount();
        if (amount > 0 && unassigned <= 0) return;
        if (amount < 0 && currentScouts <= 0) return;
        currentScouts += amount;
        UpdateUI();
    }

    public void ModifyDefenders(int amount)
    {
        int unassigned = GetUnassignedCount();
        if (amount > 0 && unassigned <= 0) return;
        if (amount < 0 && currentDefenders <= 0) return;
        currentDefenders += amount;
        UpdateUI();
    }

    int GetUnassignedCount()
    {
        return GameManager.Instance.totalSurvivors - (currentScouts + currentDefenders);
    }

    void UpdateUI()
    {
        if (availableText == null) return;
        availableText.text = GetUnassignedCount().ToString();
        scoutCountText.text = currentScouts.ToString();
        defenderCountText.text = currentDefenders.ToString();
    }
}

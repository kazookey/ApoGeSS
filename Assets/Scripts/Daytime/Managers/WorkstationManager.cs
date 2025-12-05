using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Keep this for the bounce animation

public class WorkstationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject workstationPanel;
    public TextMeshProUGUI barricadeStatusText;

    [Header("Buttons")]
    public Button toggleButton; // Replaces closeButton
    public Button repairButton;
    public Button buyAmmoButton;
    public Button hireDefenderButton; 

    [Header("Costs & Values")]
    public int repairCost = 50;
    public float repairAmount = 25f;

    public ItemData ammoItem; 
    public int ammoCost = 30;
    public int ammoPackSize = 10;

    public int hireCost = 100;

    void Start()
    {
        if (workstationPanel != null) workstationPanel.SetActive(false);

        if (toggleButton != null) toggleButton.onClick.AddListener(ToggleWorkstation);
        if (repairButton != null) repairButton.onClick.AddListener(BuyRepair);
        if (buyAmmoButton != null) buyAmmoButton.onClick.AddListener(BuyAmmo);
        if (hireDefenderButton != null) hireDefenderButton.onClick.AddListener(BuyDefender);
    }

    public void ToggleWorkstation()
    {
        if (workstationPanel.activeSelf)
            CloseWorkstation();
        else
            OpenWorkstation();
    }

    void OpenWorkstation()
    {
        workstationPanel.SetActive(true);

        // DOTween Bounce Effect
        workstationPanel.transform.localScale = Vector3.zero; 
        workstationPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        UpdateUI();
    }

    public void CloseWorkstation()
    {
        workstationPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            workstationPanel.SetActive(false);
        });
    }

    void UpdateUI()
    {
        // Update Barricade Text
        if (barricadeStatusText != null)
        {
            float hp = GameManager.Instance.barricadeHealth;
            barricadeStatusText.text = $"Barricade Integrity: {hp}%";
        }

        // Repair Button
        if (repairButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= repairCost;
            bool needsRepair = GameManager.Instance.barricadeHealth < 100f;
            repairButton.interactable = canAfford && needsRepair;
            
            TextMeshProUGUI btnText = repairButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Repair Wall \n(-${repairCost})";
        }

        // Ammo Button
        if (buyAmmoButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= ammoCost;
            buyAmmoButton.interactable = canAfford;
            
            TextMeshProUGUI btnText = buyAmmoButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Buy Ammo x{ammoPackSize} \n(-${ammoCost})";
        }

        // Hire Button
        if (hireDefenderButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= hireCost;
            bool notFull = GameManager.Instance.totalSurvivors < GameManager.Instance.maxSurvivors;
            
            hireDefenderButton.interactable = canAfford && notFull;
            
            TextMeshProUGUI btnText = hireDefenderButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) 
            {
                btnText.text = notFull ? $"Recruit Survivor \n(-${hireCost})" : "Max Staff Reached";
            }
        }
    }

    public void BuyRepair()
    {
        if (GameManager.Instance.ModifyCredits(-repairCost))
        {
            GameManager.Instance.barricadeHealth += repairAmount;
            if (GameManager.Instance.barricadeHealth > 100f) 
                GameManager.Instance.barricadeHealth = 100f;
            
            Debug.Log("Barricade Repaired!");
            UpdateUI(); 
        }
    }

    public void BuyAmmo()
    {
        if (ammoItem == null)
        {
            Debug.LogError("Forgot to assign Ammo Item in Inspector!");
            return;
        }

        if (GameManager.Instance.ModifyCredits(-ammoCost))
        {
            GameManager.Instance.AddItem(ammoItem, ammoPackSize);
            Debug.Log("Ammo Purchased!");
            UpdateUI();
        }
    }

    public void BuyDefender()
    {
        if (GameManager.Instance.totalSurvivors >= GameManager.Instance.maxSurvivors)
        {
            Debug.Log("Max Survivors Reached!");
            return;
        }

        if (GameManager.Instance.ModifyCredits(-hireCost))
        {
            GameManager.Instance.totalSurvivors++;
            Debug.Log("Survivor Hired! Total: " + GameManager.Instance.totalSurvivors);
            UpdateUI();
        }
    }
}

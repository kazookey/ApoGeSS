using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Keep this for the bounce animation

public class WorkstationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject workstationPanel;
    public TextMeshProUGUI barricadeStatusText;
    // Note: We removed the local creditsText because DayManager handles it now!

    [Header("Buttons")]
    public Button repairButton;
    public Button buyAmmoButton;
    public Button hireDefenderButton; // The button to recruit a survivor
    public Button closeButton;

    [Header("Costs & Values")]
    public int repairCost = 50;
    public float repairAmount = 25f;
    
    public ItemData ammoItem; 
    public int ammoCost = 30;
    public int ammoPackSize = 10;

    // --- THIS WAS MISSING ---
    public int hireCost = 100; 
    // ------------------------

    void Start()
    {
        if(workstationPanel != null) workstationPanel.SetActive(false);

        if(repairButton != null) repairButton.onClick.AddListener(BuyRepair);
        if(buyAmmoButton != null) buyAmmoButton.onClick.AddListener(BuyAmmo);
        if(hireDefenderButton != null) hireDefenderButton.onClick.AddListener(BuyDefender); // Link the new button
        if(closeButton != null) closeButton.onClick.AddListener(CloseWorkstation);
    }

    public void OpenWorkstation()
    {
        workstationPanel.SetActive(true);
        
        // DOTween Bounce Effect
        workstationPanel.transform.localScale = Vector3.zero; 
        workstationPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        UpdateUI();
    }

    public void CloseWorkstation()
    {
        // DOTween Close Effect
        workstationPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            workstationPanel.SetActive(false);
        });
    }

    void UpdateUI()
    {
        // 1. Update Barricade Text
        if (barricadeStatusText != null)
        {
            float hp = GameManager.Instance.barricadeHealth;
            barricadeStatusText.text = $"Barricade Integrity: {hp}%";
        }

        // 2. Update Repair Button
        if (repairButton != null) 
        {
            bool canAfford = GameManager.Instance.credits >= repairCost;
            bool needsRepair = GameManager.Instance.barricadeHealth < 100f;
            repairButton.interactable = canAfford && needsRepair;
            
            TextMeshProUGUI btnText = repairButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Repair Wall (-${repairCost})";
        }

        // 3. Update Ammo Button
        if (buyAmmoButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= ammoCost;
            buyAmmoButton.interactable = canAfford;
            
            TextMeshProUGUI btnText = buyAmmoButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Buy Ammo x{ammoPackSize} (-${ammoCost})";
        }

        // 4. Update Hire Button (New Staff Logic)
        if (hireDefenderButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= hireCost;
            bool notFull = GameManager.Instance.totalSurvivors < GameManager.Instance.maxSurvivors;
            
            hireDefenderButton.interactable = canAfford && notFull;
            
            TextMeshProUGUI btnText = hireDefenderButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) 
            {
                if (!notFull) btnText.text = "Max Staff Reached";
                else btnText.text = $"Recruit Survivor (-${hireCost})";
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

    // This adds a generic survivor to the pool
    public void BuyDefender()
    {
        // Double Check Limit
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
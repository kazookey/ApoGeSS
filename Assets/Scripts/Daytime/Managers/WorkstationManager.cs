using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkstationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject workstationPanel;
    public TextMeshProUGUI barricadeStatusText;

    [Header("Buttons")]
    public Button repairButton;
    public Button buyAmmoButton;
    public Button closeButton;

    [Header("Costs & Values")]
    public int repairCost = 50;
    public float repairAmount = 25f;
    
    public ItemData ammoItem; 
    public int ammoCost = 30;
    public int ammoPackSize = 10;

    void Start()
    {
        if(workstationPanel != null) workstationPanel.SetActive(false);

        if(repairButton != null) repairButton.onClick.AddListener(BuyRepair);
        if(buyAmmoButton != null) buyAmmoButton.onClick.AddListener(BuyAmmo);
        if(closeButton != null) closeButton.onClick.AddListener(CloseWorkstation);
    }

    public void OpenWorkstation()
    {
        workstationPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseWorkstation()
    {
        workstationPanel.SetActive(false);
    }

    void UpdateUI()
    {
        // 1. Update Barricade Text (Only shows Health)
        if (barricadeStatusText != null)
        {
            float hp = GameManager.Instance.barricadeHealth;
            barricadeStatusText.text = $"Barricade Integrity: {hp}%";
        }

        // 3. Update Buttons
        if (repairButton != null) 
        {
            bool canAfford = GameManager.Instance.credits >= repairCost;
            bool needsRepair = GameManager.Instance.barricadeHealth < 100f;
            repairButton.interactable = canAfford && needsRepair;
            
            // Optional: Update button label
            TextMeshProUGUI btnText = repairButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Repair Wall (-{repairCost})";
        }

        if (buyAmmoButton != null)
        {
            bool canAfford = GameManager.Instance.credits >= ammoCost;
            buyAmmoButton.interactable = canAfford;
            
            // Optional: Update button label
            TextMeshProUGUI btnText = buyAmmoButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = $"Buy Ammo x{ammoPackSize} (-{ammoCost})";
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
}
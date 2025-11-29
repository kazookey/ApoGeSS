using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SalesManager : MonoBehaviour
{
    public enum PricingPolicy { Normal, Discount, Luxury }

    [Header("Settings")]
    public PricingPolicy currentPolicy = PricingPolicy.Normal;
    [Range(0f, 1f)] public float baseSellChance = 0.3f; // 30% chance per item to sell

    [Header("UI References")]
    public TMP_Dropdown policyDropdown; // To select the policy
    public GameObject summaryPanel;     // The popup
    public TextMeshProUGUI summaryText; // "Sold 5 items for $50"
    public Button closeSummaryButton;

    // Tracking for the summary
    private int itemsSoldInPhase = 0;
    private int revenueInPhase = 0;

    void Start()
    {
        // Setup UI
        if(summaryPanel != null) summaryPanel.SetActive(false);
        if(closeSummaryButton != null) closeSummaryButton.onClick.AddListener(CloseSummary);
        
        // Setup Dropdown listener
        if(policyDropdown != null) 
        {
            policyDropdown.onValueChanged.AddListener(delegate { SetPolicy(policyDropdown.value); });
            // Ensure dropdown matches current setting
            policyDropdown.value = (int)currentPolicy;
        }
    }

    public void SetPolicy(int index)
    {
        currentPolicy = (PricingPolicy)index;
        Debug.Log("Pricing Policy set to: " + currentPolicy);
    }

    // This is called AUTOMATICALLY when time advances
    public void ProcessPassiveSales()
    {
        // 1. Calculate Multipliers based on Policy
        float priceMultiplier = 1.0f;
        float demandMultiplier = 1.0f;

        switch (currentPolicy)
        {
            case PricingPolicy.Discount:
                priceMultiplier = 0.7f; // Sell for 30% off...
                demandMultiplier = 2.0f; // ...but sell TWICE as fast
                break;
            case PricingPolicy.Luxury:
                priceMultiplier = 1.5f; // Sell for 50% markup...
                demandMultiplier = 0.4f; // ...but sell less than half as often
                break;
        }

        // 2. Roll dice for inventory
        List<ItemData> itemsToSell = new List<ItemData>();
        
        // We look at everything in the GameManager's inventory
        foreach (var kvp in GameManager.Instance.inventory)
        {
            ItemData item = kvp.Key;
            int quantity = kvp.Value;

            // Optional: Skip Ammo if you don't want to sell your bullets
            // if(item.isAmmo) continue;

            for (int i = 0; i < quantity; i++)
            {
                // Roll the dice
                float chance = baseSellChance * demandMultiplier;
                if (Random.value < chance)
                {
                    itemsToSell.Add(item);
                }
            }
        }

        // 3. Process the sales
        foreach (var item in itemsToSell)
        {
            int salePrice = Mathf.RoundToInt(item.baseValue * priceMultiplier);
            
            // Give money, Take item
            GameManager.Instance.ModifyCredits(salePrice);
            GameManager.Instance.AddItem(item, -1);

            // Track stats
            itemsSoldInPhase++;
            revenueInPhase += salePrice;
        }

        Debug.Log($"Passive Sales: Sold {itemsToSell.Count} items.");
        
        // If you want the summary to pop up IMMEDIATELY after every time change:
        if(itemsToSell.Count > 0) ShowSalesSummary();
    }

    public void ShowSalesSummary()
    {
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(true);
            summaryText.text = $"<b>Passive Sales Report</b>\n\n" +
                               $"Items Sold: {itemsSoldInPhase}\n" +
                               $"Revenue: <color=green>${revenueInPhase}</color>";
        }
        
        // Reset counters after showing
        itemsSoldInPhase = 0;
        revenueInPhase = 0;
    }

    void CloseSummary()
    {
        summaryPanel.SetActive(false);
    }
}
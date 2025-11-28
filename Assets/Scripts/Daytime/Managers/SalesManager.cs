using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SalesManager : MonoBehaviour
{
    public enum PricingPolicy { Discount, Normal, Luxury }

    [Header("Settings")]
    public PricingPolicy currentPolicy = PricingPolicy.Normal;
    [Range(0f, 1f)] public float baseSellChance = 0.3f; // 30% chance per item slot to sell

    [Header("UI References")]
    public TMP_Dropdown policyDropdown; // To select Discount/Normal/Luxury
    public GameObject summaryPanel;     // The popup at end of phase
    public TextMeshProUGUI summaryText; // "Sold 5 items for $50"
    public Button closeSummaryButton;

    private int pendingRevenue = 0;
    private int itemsSold = 0;

    void Start()
    {
        summaryPanel.SetActive(false);
        // Link dropdown if set in inspector
        if(policyDropdown != null) 
            policyDropdown.onValueChanged.AddListener(delegate { SetPolicy(policyDropdown.value); });
    }

    public void SetPolicy(int index)
    {
        currentPolicy = (PricingPolicy)index;
        Debug.Log("Pricing Policy set to: " + currentPolicy);
    }

    // Call this whenever DayManager advances time!
    public void ProcessPassiveSales()
    {
        // Calculate multipliers
        float priceMultiplier = 1.0f;
        float demandMultiplier = 1.0f;

        switch (currentPolicy)
        {
            case PricingPolicy.Discount:
                priceMultiplier = 0.8f; // Sell for less...
                demandMultiplier = 1.5f; // ...but sell MORE often
                break;
            case PricingPolicy.Luxury:
                priceMultiplier = 1.3f; // Sell for more...
                demandMultiplier = 0.5f; // ...but sell LESS often
                break;
        }

        // We need a temporary list because we can't modify the Dictionary while looping through it
        List<ItemData> itemsToSell = new List<ItemData>();
        
        foreach (var kvp in GameManager.Instance.inventory)
        {
            ItemData item = kvp.Key;
            int quantity = kvp.Value;

            // Only sell regular items, not Ammo/Traps (unless you want to!)
            // We'll check the "IsAmmo" flag from your ItemData if you want to exclude them.
            
            for (int i = 0; i < quantity; i++)
            {
                if (Random.value < (baseSellChance * demandMultiplier))
                {
                    itemsToSell.Add(item);
                }
            }
        }

        // Apply sales
        int revenueThisTick = 0;
        foreach (var item in itemsToSell)
        {
            int salePrice = Mathf.RoundToInt(item.baseValue * priceMultiplier);
            revenueThisTick += salePrice;
            GameManager.Instance.AddItem(item, -1); // Remove from inventory
        }

        // If we sold anything, add to the pending total
        if (revenueThisTick > 0)
        {
            pendingRevenue += revenueThisTick;
            itemsSold += itemsToSell.Count;
            GameManager.Instance.ModifyCredits(revenueThisTick);
        }
    }

    // Call this at the start of a new Phase (e.g. Morning -> Noon)
    public void ShowSalesSummary()
    {
        if (itemsSold > 0)
        {
            summaryPanel.SetActive(true);
            summaryText.text = $"While you were busy, the store sold:\n\n" +
                               $"<b>{itemsSold} Items</b>\n" +
                               $"Profit: <color=green>${pendingRevenue}</color>";
            
            // Reset counters
            itemsSold = 0;
            pendingRevenue = 0;
        }
    }

    public void CloseSummary()
    {
        summaryPanel.SetActive(false);
    }
}
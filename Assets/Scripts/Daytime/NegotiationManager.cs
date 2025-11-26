using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NegotiationManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DayManager dayManager; // To deduct time

    [Header("UI Elements")]
    public GameObject panel;
    public TextMeshProUGUI promptText; // "I want to sell you this Scrap"
    public Slider priceSlider;
    public TextMeshProUGUI offerValueText; // The number on the slider
    public TextMeshProUGUI resultText;
    public Button confirmButton;

    private ItemData currentItem;
    private bool isPlayerSelling; // True = Customer buying from us. False = We buy from Trader.
    private int basePrice;

    void Start()
    {
        panel.SetActive(false);
        priceSlider.onValueChanged.AddListener(UpdateSliderText);
    }

    // Call this to open the window
    public void StartNegotiation(ItemData item, bool playerSelling)
    {
        currentItem = item;
        isPlayerSelling = playerSelling;
        basePrice = item.baseValue;

        panel.SetActive(true);
        confirmButton.interactable = true;
        resultText.text = "";

        // Set Slider Limits (50% to 200% of value)
        priceSlider.minValue = basePrice * 0.5f;
        priceSlider.maxValue = basePrice * 2.0f;
        priceSlider.value = basePrice; // Default to fair price

        if (isPlayerSelling)
            promptText.text = $"Customer wants to buy {item.itemName}.";
        else
            promptText.text = $"Trader offers {item.itemName}.";
            
        UpdateSliderText(priceSlider.value);
    }

    void UpdateSliderText(float val)
    {
        offerValueText.text = "$" + Mathf.RoundToInt(val).ToString();
    }

    public void SubmitOffer()
    {
        int offer = Mathf.RoundToInt(priceSlider.value);
        bool success = CheckIfAccepted(offer);

        if (success)
        {
            ProcessTransaction(offer);
            resultText.text = "<color=green>Offer Accepted!</color>";
        }
        else
        {
            resultText.text = "<color=red>They refused and left!</color>";
        }

        confirmButton.interactable = false;
        
        // Close window after short delay and advance time
        Invoke(nameof(CloseNegotiation), 2f);
    }

    bool CheckIfAccepted(int offer)
    {
        float ratio = (float)offer / basePrice;
        float chance = 1.0f;

        // Logic: 
        // If we are Buying, offering LOW decreases chance.
        // If we are Selling, asking HIGH decreases chance.
        
        if (!isPlayerSelling) // Buying
        {
            if (ratio < 1.0f) chance = ratio; // Simple linear drop-off
        }
        else // Selling
        {
            if (ratio > 1.0f) chance = 1.0f - (ratio - 1.0f);
        }

        // Reputation Bonus (Optional)
        chance += (GameManager.Instance.reputation / 100f) * 0.1f; 

        return Random.value <= chance;
    }

    void ProcessTransaction(int price)
    {
        if (isPlayerSelling)
        {
            GameManager.Instance.ModifyCredits(price);
            GameManager.Instance.AddItem(currentItem, -1); // Remove from stock
        }
        else
        {
            if (GameManager.Instance.ModifyCredits(-price))
            {
                GameManager.Instance.AddItem(currentItem, 1); // Add to stock
            }
            else
            {
                resultText.text = "Not enough cash!";
                return;
            }
        }
    }

    void CloseNegotiation()
    {
        panel.SetActive(false);
        dayManager.AdvanceTime(1); // Takes 1 "Time Slot" to negotiate
    }
}
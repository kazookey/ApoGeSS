using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NegotiationManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DayManager dayManager;

    [Header("UI Elements")]
    public GameObject panel;
    public TextMeshProUGUI promptText; // "I have a [Item] to sell."
    public TextMeshProUGUI marketValueText; // "Market Value: $100"
    public Slider offerSlider;
    public TextMeshProUGUI offerValueText; // "Your Offer: $80"
    public TextMeshProUGUI resultText;
    
    public Button confirmButton;
    public Button declineButton;

    private ItemData currentItem;
    private int basePrice;

    void Start()
    {
        panel.SetActive(false);
        offerSlider.onValueChanged.AddListener(UpdateSliderText);
    }

    public void OpenNegotiation(ItemData item)
    {
        currentItem = item;
        basePrice = item.baseValue;

        panel.SetActive(true);
        confirmButton.interactable = true;
        declineButton.interactable = true;
        resultText.text = "";

        // Setup UI
        promptText.text = $"A Scavenger wants to sell <b>{item.itemName}</b>.";
        marketValueText.text = $"Market Value: ${basePrice}";

        // Slider limits: You can offer between $1 and 150% of value
        offerSlider.minValue = 1;
        offerSlider.maxValue = basePrice * 1.5f;
        offerSlider.value = basePrice * 0.8f; // Default to a slight "deal" (80%)

        UpdateSliderText(offerSlider.value);
    }

    void UpdateSliderText(float val)
    {
        offerValueText.text = $"Your Offer: ${Mathf.RoundToInt(val)}";
    }

    public void SubmitOffer()
    {
        int offerAmount = Mathf.RoundToInt(offerSlider.value);

        // Check if we even have the money
        if (GameManager.Instance.credits < offerAmount)
        {
            resultText.text = "<color=red>You don't have enough credits!</color>";
            return;
        }

        bool accepted = CheckIfAccepted(offerAmount);

        if (accepted)
        {
            // Transaction: Minus Money, Plus Item
            GameManager.Instance.ModifyCredits(-offerAmount);
            GameManager.Instance.AddItem(currentItem, 1);
            
            resultText.text = "<color=green>Deal Accepted!</color>";
            EndInteraction();
        }
        else
        {
            resultText.text = "<color=red>They refused your low offer and left.</color>";
            EndInteraction();
        }
    }

    public void DeclineTrade()
    {
        resultText.text = "You rejected the offer.";
        EndInteraction();
    }

    private void EndInteraction()
    {
        confirmButton.interactable = false;
        declineButton.interactable = false;
        
        // Wait 2 seconds, then close and advance time
        Invoke(nameof(ClosePanel), 2f);
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        dayManager.AdvanceTime(1); // Spending time negotiating
    }

    // --- THE LOGIC ---
    // If you offer Market Value (100%), chance is 100%.
    // If you offer 50%, chance is 50%.
    // If you offer 1%, chance is 1%.
    bool CheckIfAccepted(int offer)
    {
        float ratio = (float)offer / basePrice;
        
        // Bonus: If you have high reputation, add 10% to the success chance
        float reputationBonus = (GameManager.Instance.reputation / 100f) * 0.10f;
        
        float successChance = ratio + reputationBonus;

        return Random.value <= successChance;
    }
}
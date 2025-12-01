using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; 

public class NegotiationManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DayManager dayManager;

    [Header("UI Elements")]
    public GameObject panel;
    public CanvasGroup negotiationCanvasGroup; 
    
    public TextMeshProUGUI promptText; 
    public TextMeshProUGUI marketValueText; 
    public Slider offerSlider;
    public TextMeshProUGUI offerValueText; 
    public TextMeshProUGUI resultText;
    
    public Button confirmButton;
    public Button declineButton;

    private ItemData currentItem;
    private int askingPrice; // What THEY want
    private int marketValue; // What it's WORTH

    void Start()
    {
        // Ensure panel is hidden
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.alpha = 0;
            negotiationCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            panel.SetActive(false);
        }
        
        offerSlider.onValueChanged.AddListener(UpdateSliderText);
    }

    // UPDATED: Now takes a multiplier!
    public void OpenNegotiation(ItemData item, float priceMultiplier)
    {
        currentItem = item;
        marketValue = item.baseValue;
        
        // Calculate the Trader's personal price
        askingPrice = Mathf.RoundToInt(marketValue * priceMultiplier);

        // 1. Enable GameObject
        panel.SetActive(true);
        
        // 2. Reset UI State
        confirmButton.interactable = true;
        declineButton.interactable = true;
        resultText.text = "";

        // 3. Setup Text/Slider
        promptText.text = $"A Scavenger wants to sell <b>{item.itemName}</b>.";
        
        // SHOW BOTH PRICES so the player sees the "Deal"
        marketValueText.text = $"Market Value: ${marketValue}\nAsking Price: ${askingPrice}";

        // Slider logic: Center it around the ASKING price
        offerSlider.minValue = 1;
        offerSlider.maxValue = askingPrice * 1.5f; 
        offerSlider.value = askingPrice; // Default to what they want

        UpdateSliderText(offerSlider.value);

        // 4. Fade In Animation
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.alpha = 0; 
            negotiationCanvasGroup.blocksRaycasts = true;
            negotiationCanvasGroup.DOFade(1f, 0.5f);
        }
    }

    void UpdateSliderText(float val)
    {
        offerValueText.text = $"Your Offer: ${Mathf.RoundToInt(val)}";
    }

    public void SubmitOffer()
    {
        int offerAmount = Mathf.RoundToInt(offerSlider.value);

        if (GameManager.Instance.credits < offerAmount)
        {
            resultText.text = "<color=red>Not enough credits!</color>";
            return;
        }

        bool accepted = CheckIfAccepted(offerAmount);

        if (accepted)
        {
            GameManager.Instance.ModifyCredits(-offerAmount);
            GameManager.Instance.AddItem(currentItem, 1);
            
            resultText.text = "<color=green>Deal Accepted!</color>";
            EndInteraction();
        }
        else
        {
            resultText.text = "<color=red>Refused!</color>";
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
        
        Invoke(nameof(ClosePanel), 1.5f);
    }

    private void ClosePanel()
    {
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.blocksRaycasts = false;
            negotiationCanvasGroup.DOFade(0f, 0.5f).OnComplete(() => 
            {
                panel.SetActive(false);
                if(dayManager != null) dayManager.AdvanceTime(1);
            });
        }
        else
        {
            panel.SetActive(false);
            if(dayManager != null) dayManager.AdvanceTime(1);
        }
    }

    bool CheckIfAccepted(int offer)
    {
        // The success depends on how close you are to THEIR Asking Price
        // (Not the market value)
        float ratio = (float)offer / askingPrice;
        
        float reputationBonus = (GameManager.Instance.reputation / 100f) * 0.10f;
        float successChance = ratio + reputationBonus;

        // Hard Limit: If you offer >= Asking Price, it's 100% success
        if (ratio >= 1.0f) return true;

        return Random.value <= successChance;
    }
}
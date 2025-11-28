using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // IMPORTANT: Requires DOTween

public class NegotiationManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DayManager dayManager;

    [Header("UI Elements")]
    public GameObject panel;
    public CanvasGroup negotiationCanvasGroup; // Used for fading
    
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

    public void OpenNegotiation(ItemData item)
    {
        currentItem = item;
        basePrice = item.baseValue;

        // 1. Enable GameObject
        panel.SetActive(true);
        
        // 2. Reset UI State
        confirmButton.interactable = true;
        declineButton.interactable = true;
        resultText.text = "";

        // 3. Setup Text/Slider
        promptText.text = $"A Scavenger wants to sell <b>{item.itemName}</b>.";
        marketValueText.text = $"Market Value: ${basePrice}";

        // Slider limits: You can offer between $1 and 150% of value
        offerSlider.minValue = 1;
        offerSlider.maxValue = basePrice * 1.5f;
        offerSlider.value = basePrice * 0.8f; // Default to a slight "deal" (80%)

        UpdateSliderText(offerSlider.value);

        // 4. Fade In Animation
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.alpha = 0; // Start transparent
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
        
        // Wait 1.5 seconds, then close
        Invoke(nameof(ClosePanel), 1.5f);
    }

    private void ClosePanel()
    {
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.blocksRaycasts = false;
            // Fade out, THEN disable object and advance time
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
        float ratio = (float)offer / basePrice;
        
        // Bonus: Reputation helps slightly
        float reputationBonus = (GameManager.Instance.reputation / 100f) * 0.10f;
        float successChance = ratio + reputationBonus;

        return Random.value <= successChance;
    }
}
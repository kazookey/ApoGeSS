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
    private int basePrice;
    private int askingPrice; 

    void Start()
    {
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

    public void OpenNegotiation(ItemData item, float priceMultiplier)
    {
        currentItem = item;
        basePrice = item.baseValue;
        askingPrice = Mathf.RoundToInt(basePrice * priceMultiplier);

        panel.SetActive(true);
        
        confirmButton.interactable = true;
        declineButton.interactable = true;
        resultText.text = "";

        promptText.text = $"A Scavenger wants to sell <b>{item.itemName}</b>.";
        marketValueText.text = $"Market Value: ${basePrice}\nAsking Price: ${askingPrice}";

        offerSlider.minValue = 1;
        offerSlider.maxValue = askingPrice * 1.5f;
        offerSlider.value = askingPrice; 

        UpdateSliderText(offerSlider.value);

        // --- ANIMATION UPDATE ---
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.alpha = 0; 
            negotiationCanvasGroup.blocksRaycasts = true;
            
            // Combine Fade AND Scale
            negotiationCanvasGroup.DOFade(1f, 0.3f);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
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
            
            // Scale down AND Fade out
            negotiationCanvasGroup.DOFade(0f, 0.3f);
            panel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
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
        float ratio = (float)offer / askingPrice;
        float reputationBonus = (GameManager.Instance.reputation / 100f) * 0.10f;
        float successChance = ratio + reputationBonus;

        if (ratio >= 1.0f) return true;
        return Random.value <= successChance;
    }
}
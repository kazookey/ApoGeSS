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
    public TextMeshProUGUI resultText; // Top text

    // --- NEW ITEM UI ---
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIcon;

    public Button confirmButton;
    public Button declineButton;

    private ItemData currentItem;
    private int basePrice;
    private int askingPrice; 
    private string initialResultText;

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

        // --- Text setup ---
        initialResultText = $"They are looking to sell it for ${askingPrice}";
        resultText.text = initialResultText;
        marketValueText.text = $"Market Value: ${basePrice}";
        offerSlider.minValue = 1;
        offerSlider.maxValue = Mathf.RoundToInt(askingPrice * 1.5f);
        offerSlider.value = askingPrice; 
        UpdateSliderText(offerSlider.value);

        // --- ITEM DISPLAY ---
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemDescriptionText != null) itemDescriptionText.text = item.description;
        if (itemIcon != null) itemIcon.sprite = item.icon;

        // --- Animation ---
        if (negotiationCanvasGroup != null)
        {
            negotiationCanvasGroup.alpha = 0; 
            negotiationCanvasGroup.blocksRaycasts = true;
            negotiationCanvasGroup.DOFade(1f, 0.3f);
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
            resultText.text = $"<color=green>Deal Accepted for ${offerAmount}!</color>";
            EndInteraction();
        }
        else
        {
            resultText.text = $"<color=red>Refused at ${offerAmount}!</color>";
            EndInteraction();
        }
    }

    public void DeclineTrade()
    {
        resultText.text = $"<color=red>You rejected the offer of ${askingPrice}.</color>";
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
            negotiationCanvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
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

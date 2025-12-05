using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI nameText; // Optional, if you want the name visible

    public void Setup(ItemData item, int count)
    {
        if (item == null) return;

        // Set Icon
        if (item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false; // Hide if no sprite
        }

        // Set Text
        countText.text = count.ToString(); // e.g., "15"
        
        if (nameText != null)
            nameText.text = item.itemName;
    }
}
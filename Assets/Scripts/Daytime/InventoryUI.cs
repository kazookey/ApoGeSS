using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Transform itemsContainer;   // The object holding the slots (Content)
    public GameObject slotPrefab;      // The template we just made

    // This runs automatically whenever you set the GameObject to Active
    void OnEnable()
    {
        UpdateInventoryDisplay();
    }

    public void UpdateInventoryDisplay()
    {
        // 1. Clear existing slots (to prevent duplicates)
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Loop through the Global Inventory
        // We use GameManager.Instance because it holds the real data
        foreach (KeyValuePair<ItemData, int> entry in GameManager.Instance.inventory)
        {
            ItemData item = entry.Key;
            int count = entry.Value;

            if (count > 0)
            {
                // Create a new slot
                GameObject newSlot = Instantiate(slotPrefab, itemsContainer);
                
                // Setup the data
                InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
                if (slotScript != null)
                {
                    slotScript.Setup(item, count);
                }
            }
        }
    }
}
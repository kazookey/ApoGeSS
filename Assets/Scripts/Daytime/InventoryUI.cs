using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // Added

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Transform itemsContainer;
    public GameObject slotPrefab;
    
    // Drag the "Inventory Window" (the parent panel) here
    public GameObject inventoryWindow; 

    void OnEnable()
    {
        UpdateInventoryDisplay();
    }

    // --- NEW METHODS FOR BUTTONS ---
    public void OpenInventory()
    {
        GameObject target = inventoryWindow != null ? inventoryWindow : gameObject;
        
        target.SetActive(true);
        target.transform.localScale = Vector3.zero;
        target.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        
        // Force refresh just in case
        UpdateInventoryDisplay();
    }

    public void CloseInventory()
    {
        GameObject target = inventoryWindow != null ? inventoryWindow : gameObject;
        
        target.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            target.SetActive(false);
        });
    }
    // -------------------------------

    public void UpdateInventoryDisplay()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<ItemData, int> entry in GameManager.Instance.inventory)
        {
            ItemData item = entry.Key;
            int count = entry.Value;

            if (count > 0)
            {
                GameObject newSlot = Instantiate(slotPrefab, itemsContainer);
                InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
                if (slotScript != null)
                {
                    slotScript.Setup(item, count);
                }
            }
        }
    }
}
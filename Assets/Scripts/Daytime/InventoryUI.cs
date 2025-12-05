using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Transform itemsContainer;
    public GameObject slotPrefab;
    
    // Drag the "Inventory Window" (the parent panel) here
    public GameObject inventoryWindow; 

    [Header("Buttons")]
    public UnityEngine.UI.Button toggleButton; // Button to open/close the inventory

    void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleInventory);

        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);
    }

    void OnEnable()
    {
        UpdateInventoryDisplay();
    }

    public void ToggleInventory()
    {
        if (inventoryWindow == null) return;

        if (inventoryWindow.activeSelf)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (inventoryWindow == null) return;

        inventoryWindow.SetActive(true);
        inventoryWindow.transform.localScale = Vector3.zero;
        inventoryWindow.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        UpdateInventoryDisplay();
    }

    public void CloseInventory()
    {
        if (inventoryWindow == null) return;

        inventoryWindow.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            inventoryWindow.SetActive(false);
        });
    }

    public void UpdateInventoryDisplay()
    {
        if (itemsContainer == null) return;

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

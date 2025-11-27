using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public NegotiationManager negotiationManager;
    
    [Header("Potential Sellers")]
    // Drag all your ScriptableObjects (Food, Scrap, Ammo) here in the Inspector
    public List<ItemData> possibleItemsToBuy; 

    // Call this via a UI Button labeled "Wait for Customers"
    public void TriggerRandomEvent()
    {
        // 1. Pick a random item
        int randomIndex = Random.Range(0, possibleItemsToBuy.Count);
        ItemData selectedItem = possibleItemsToBuy[randomIndex];

        // 2. Start the negotiation
        negotiationManager.OpenNegotiation(selectedItem);
    }
}
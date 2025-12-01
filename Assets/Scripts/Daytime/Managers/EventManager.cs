using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("Dependencies")]
    public NegotiationManager negotiationManager;
    public DialogueManager dialogueManager;
    
    [Header("Event Pool")]
    public List<TraderData> possibleTraders; // Drag your Trader Profiles here!

    public void TriggerRandomEvent()
    {
        // 1. Pick a random Trader Type
        if (possibleTraders.Count == 0) 
        {
            Debug.LogWarning("No Traders in the list!");
            return;
        }
        TraderData selectedTrader = possibleTraders[Random.Range(0, possibleTraders.Count)];

        // 2. Pick a random Item from THAT trader
        if (selectedTrader.possibleItems.Count == 0)
        {
            Debug.LogWarning($"{selectedTrader.traderName} has no items!");
            return;
        }
        ItemData selectedItem = selectedTrader.possibleItems[Random.Range(0, selectedTrader.possibleItems.Count)];

        // 3. Pick a random Intro Line
        string randomLine = "I have something to trade.";
        if (selectedTrader.introLines.Length > 0)
            randomLine = selectedTrader.introLines[Random.Range(0, selectedTrader.introLines.Length)];

        // 4. Start the Dialogue using the TRADER'S data
        dialogueManager.StartInteraction(
            selectedTrader.traderName, 
            selectedTrader.portrait, 
            randomLine, 
            () => {
                // 5. Pass the Item AND the Price Multiplier to Negotiation
                negotiationManager.OpenNegotiation(selectedItem, selectedTrader.priceMultiplier);
            }
        );
    }
}
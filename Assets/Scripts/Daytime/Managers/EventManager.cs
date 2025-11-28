using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("Dependencies")]
    public NegotiationManager negotiationManager;
    public DialogueManager dialogueManager;
    
    [Header("Potential Sellers")]
    public List<ItemData> possibleItemsToBuy;

    [Header("NPC Data")]
    // Placeholder sprite until you have Stanley's assets
    public Sprite defaultNpcPortrait; 
    
    public string[] npcIntroLines = {
        "Hey... got some good stuff here.",
        "Found this out in the wastes. You buying?",
        "Don't ask where I got this. Just give me a price.",
        "Need some quick cash. Take a look."
    };

    public void TriggerRandomEvent()
    {
        // 1. Pick a random item
        if (possibleItemsToBuy.Count == 0) 
        {
            Debug.LogWarning("No items in 'Possible Items To Buy' list!");
            return;
        }
        
        ItemData selectedItem = possibleItemsToBuy[Random.Range(0, possibleItemsToBuy.Count)];
        string randomLine = npcIntroLines[Random.Range(0, npcIntroLines.Length)];

        // 2. Start the Dialogue FIRST
        dialogueManager.StartInteraction(
            "Scavenger", 
            defaultNpcPortrait, 
            randomLine, 
            () => {
                // 3. This runs AFTER dialogue finishes:
                negotiationManager.OpenNegotiation(selectedItem);
            }
        );
    }
}
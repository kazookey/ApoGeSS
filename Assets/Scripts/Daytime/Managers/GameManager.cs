using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Stats")]
    public int currentDay = 1;
    public int credits = 150; // Starting money [cite: 114]
    public int reputation = 50; // Affects customer patience/offers
    public float barricadeHealth = 100f;
    public ItemData testItem; //temporary
    
    [Header("Staff System")]
    public int totalSurvivors = 0;
    public int maxSurvivors = 4;
    
    // Assigned roles for the UPCOMING night
    public int assignedDefenders = 0;
    public int assignedScouts = 0;
    
    [Header("Scouting Data")]
    public List<ItemData> scoutLootTable; // Drag items here in Inspector!
    [Range(0f, 1f)] public float scoutDeathChance = 0.2f; // 20% chance to die
    
    public string pendingMorningReport = "";

    // Inventory: Item -> Quantity
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    
    
    
    void Start()  //temporary
    {
        if(testItem != null) AddItem(testItem, 10);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool ModifyCredits(int amount)
    {
        if (credits + amount < 0) return false;
        credits += amount;
        return true;
    }

    public void AddItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += count;
        else
            inventory.Add(item, count);
            
        Debug.Log($"Added {count} of {item.itemName} to Shelves.");
    }
    
   
    public bool TryConsumeItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item) && inventory[item] >= count)
        {
            inventory[item] -= count;
            Debug.Log($"Consumed {count} of {item.itemName}. Remaining: {inventory[item]}");
            return true;
        }
        return false;
    }

    public int GetItemCount(ItemData item)
    {
        if (inventory.ContainsKey(item)) return inventory[item];
        return 0;
    }
    
    public void ProcessMorningResults()
    {
        pendingMorningReport = "";
        int survivorsDied = 0;
        List<string> lootFound = new List<string>();

        // 1. Process Scouts
        for (int i = 0; i < assignedScouts; i++)
        {
            if (Random.value < scoutDeathChance)
            {
                survivorsDied++;
                totalSurvivors--; 
            }
            else
            {
                // Survivor lived! Give loot.
                if (scoutLootTable.Count > 0)
                {
                    ItemData foundItem = scoutLootTable[Random.Range(0, scoutLootTable.Count)];
                    AddItem(foundItem, 1); // Add to inventory
                    lootFound.Add(foundItem.itemName);
                }
            }
        }

        // 2. Build the Report String
        if (assignedScouts > 0)
        {
            pendingMorningReport += $"<b>Scouting Report:</b>\n";
            if (survivorsDied > 0) 
                pendingMorningReport += $"<color=red>- {survivorsDied} Scout(s) never returned.</color>\n";
            
            if (lootFound.Count > 0)
                pendingMorningReport += $"<color=green>+ Found: {string.Join(", ", lootFound)}</color>\n";
            
            if (survivorsDied == 0 && lootFound.Count == 0)
                pendingMorningReport += "Scouts found nothing, but returned safely.\n";
        }

        // Reset assignments for the new day (optional, or keep them sticky)
        assignedScouts = 0;
        assignedDefenders = 0;
    }

}
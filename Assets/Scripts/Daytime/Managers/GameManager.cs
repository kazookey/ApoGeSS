using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Stats")]
    public int currentDay = 1;
    public int credits = 150; 
    public int reputation = 50; 
    public float barricadeHealth = 100f;
    
    // --- NEW STAFF VARIABLES ---
    [Header("Staff System")]
    public int totalSurvivors = 0;
    public int maxSurvivors = 4;
    
    // Roles for the UPCOMING night
    public int assignedDefenders = 0;
    public int assignedScouts = 0;

    [Header("Scouting Data")]
    public List<ItemData> scoutLootTable; // Drag items here in Inspector!
    [Range(0f, 1f)] public float scoutDeathChance = 0.2f; // 20% chance to die
    
    [HideInInspector]
    public string pendingMorningReport = ""; 
    // ---------------------------

    // Inventory Dictionary
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    
    [Header("Debug/Testing")]
    public ItemData testItem; 

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

    void Start()
    {
        if(testItem != null) AddItem(testItem, 10);
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
    }
    
    public bool TryConsumeItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item) && inventory[item] >= count)
        {
            inventory[item] -= count;
            return true;
        }
        return false;
    }

    public void AdvanceDay()
    {
        currentDay++;
        Debug.Log($"Day {currentDay} Started!");
    }

    // --- NEW FUNCTION: Calculates Scout Results ---
    public void ProcessMorningResults()
    {
        pendingMorningReport = "";
        int survivorsDied = 0;
        List<string> lootFound = new List<string>();

        // 1. Process Scouts
        for (int i = 0; i < assignedScouts; i++)
        {
            // Roll for death
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
                    AddItem(foundItem, 1);
                    lootFound.Add(foundItem.itemName);
                }
            }
        }

        // 2. Build the Report Text
        if (assignedScouts > 0)
        {
            pendingMorningReport += $"<b>Scouting Report:</b>\n";
            
            if (survivorsDied > 0) 
                pendingMorningReport += $"<color=red>- {survivorsDied} Scout(s) never returned.</color>\n";
            else
                pendingMorningReport += "<color=green>All scouts returned safely.</color>\n";

            if (lootFound.Count > 0)
                pendingMorningReport += $"<color=yellow>+ Found: {string.Join(", ", lootFound)}</color>\n";
            else if (survivorsDied == 0)
                pendingMorningReport += "They found nothing of value.\n";
        }

        // Reset assignments for the new day
        assignedScouts = 0;
        assignedDefenders = 0;
    }
}
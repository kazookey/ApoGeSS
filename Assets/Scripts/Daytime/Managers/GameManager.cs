using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Stats")]
    public int currentDay = 1;
    public int credits = 150; // Starting money [cite: 114]
    public int reputation = 50; // Affects customer patience/offers
    public ItemData testItem; //temporary

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

    public int GetItemCount(ItemData item)
    {
        if (inventory.ContainsKey(item)) return inventory[item];
        return 0;
    }
}
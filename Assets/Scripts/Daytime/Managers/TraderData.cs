using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Trader", menuName = "Shop/Trader Profile")]
public class TraderData : ScriptableObject
{
    [Header("Identity")]
    public string traderName; // e.g. "Desperate Survivor"
    public Sprite portrait;   // Their face
    
    [Header("Personality")]
    [TextArea] public string[] introLines; // Random things they say
    
    [Header("Inventory")]
    public List<ItemData> possibleItems; // What they might carry

    [Header("Economics")]
    [Range(0.1f, 3.0f)] 
    public float priceMultiplier = 1.0f; 
    // 0.5 = Sells for HALF market value (Cheap!)
    // 1.5 = Sells for 150% market value (Expensive!)
}
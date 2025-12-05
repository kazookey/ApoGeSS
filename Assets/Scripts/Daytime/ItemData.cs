using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Economy")]
    public int baseValue; // The "Standard" market price

    [Header("Night-Time Effects")]
    public bool isAmmo;
    public bool isBarricadeRepair;
    public float effectAmount; // e.g., +10 Ammo or +20 Health
}
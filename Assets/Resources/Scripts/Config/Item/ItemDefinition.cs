using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id; 
    public string itemName;
    public ItemType type;

    [Header("Stats")]
    public int maxStack = 1;
    public float weight;
    public Sprite icon;
}

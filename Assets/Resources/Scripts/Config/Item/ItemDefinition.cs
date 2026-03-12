using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]

public class ItemDefinition : ScriptableObject
{
    public string id;
    public string itemName;
    public ItemType type;

    public int maxStack = 1;
    public float weight;

    public Sprite icon;

}


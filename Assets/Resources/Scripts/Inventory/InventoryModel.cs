using System.Collections.Generic;

[System.Serializable]
public class InventoryModel
{
    public List<InventorySlot> slots = new List<InventorySlot>();

    public int coins;

    public float totalWeight = 0;
}
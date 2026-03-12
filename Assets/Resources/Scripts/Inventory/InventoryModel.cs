using System.Collections.Generic;

[System.Serializable]
public class InventoryModel
{
    public int coins;

    public List<InventorySlot> slots = new List<InventorySlot>();
}
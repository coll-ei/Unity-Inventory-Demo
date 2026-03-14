using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class InventoryModel
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public int coins;

    public float TotalWeight => slots.Sum(s => s.itemSlot != null ? s.itemSlot.weight * s.quantity : 0f);
}
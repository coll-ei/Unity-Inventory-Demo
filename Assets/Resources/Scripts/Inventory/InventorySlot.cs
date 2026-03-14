using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InventorySlot
{
    public bool isLocked;
    public string itemId; 
    public int quantity;
    public int slotsNumber;

    [NonSerialized] public ItemDefinition itemSlot;
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> _initialItems = new List<ItemDefinition>();

    private GameConfig _config;
    private InventoryModel inventory = new InventoryModel();

    private void Start()
    {
        _config = GameManager.instance.config;

        CreateInventory();

        FillInitialItems();

        gameObject.GetComponent<UIController>().OnStart();
    }
    private void FillInitialItems()
    {
        if (_initialItems == null || _initialItems.Count == 0) return;

        foreach (var item in _initialItems)
        {
            if (item != null)
            {
                AddItem(item, 1);
            }
        }
    }
    private void CreateInventory()
    {
        inventory.slots.Clear();
        for (int i = 0; i < _config.maxSlots; i++)
        {

            InventorySlot slot = new InventorySlot();

            slot.isLocked = i >= _config.defaultUnlocked;
            slot.slotsNumber = i;
            slot.itemSlot = null;
            slot.quantity = 0;

            inventory.slots.Add(slot);
        }
    }

    public InventoryModel GetModel() => inventory;

    public bool AddItem(ItemDefinition def, int amount = 1)
    {
        if (def == null || amount <= 0) return false;

        if (def.maxStack <= 1)
        {
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                var s = inventory.slots[i];
                if (!s.isLocked && s.itemSlot == null)
                {
                    s.itemSlot = def;
                    s.quantity = 1;
                    return true;
                }
            }
            Debug.Log("AddItem: no free slot for non-stackable item");
            return false;
        }

        else
        {
            int remaining = amount;
            // сначала докидываем в существующие стеки
            for (int i = 0; i < inventory.slots.Count && remaining > 0; i++)
            {
                var s = inventory.slots[i];
                if (!s.isLocked && s.itemSlot == def && s.quantity < def.maxStack)
                {
                    int can = def.maxStack - s.quantity;
                    int add = Mathf.Min(can, remaining);
                    s.quantity += add;
                    remaining -= add;
                }
            }
            // потом используем пустые слоты
            for (int i = 0; i < inventory.slots.Count && remaining > 0; i++)
            {
                var s = inventory.slots[i];
                if (!s.isLocked && s.itemSlot == null)
                {
                    int add = Mathf.Min(def.maxStack, remaining);
                    s.itemSlot = def;
                    s.quantity = add;
                    remaining -= add;
                }
            }

            if (remaining > 0)
            {
                Debug.Log("AddItem: not enough space for all items");
                return false;
            }
            return true;
        }
    }

    public bool RemoveFromSlot(int slotIndex, int qty = 1)
    {
        if (slotIndex < 0 || slotIndex >= inventory.slots.Count) return false;
        var s = inventory.slots[slotIndex];
        if (s.itemSlot == null) return false;

        s.quantity -= qty;
        if (s.quantity <= 0) { s.itemSlot = null; s.quantity = 0; }
        return true;
    }

    public bool UnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.slots.Count) return false;

        var s = inventory.slots[slotIndex];

        if (!s.isLocked) return false;

        if (GameManager.instance.playerCoins >= _config.unlockPrice) 
        {
            GameManager.instance.playerCoins -= _config.unlockPrice;
            s.isLocked = false;
            return true;
        }

        return false;
    }
}
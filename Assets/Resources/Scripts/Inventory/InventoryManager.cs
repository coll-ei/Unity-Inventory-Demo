using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryManager : MonoBehaviour
{
    private GameConfig _config;
    private InventoryModel _inventory = new InventoryModel();
    public Action inventoryChanged;

    [Inject]
    public void Initialize(GameConfig config)
    {
        _config = config;
        CreateInventory();
        _inventory.coins = _config.initialCoins;
        _inventory.totalWeight = 0; // Изначально вес 0

        Debug.Log($"<color=green>Inventory Initialized!</color> Coins: {_inventory.coins}, Slots: {_inventory.slots.Count}");
        inventoryChanged?.Invoke();
    }

    private void CreateInventory()
    {
        _inventory.slots.Clear();
        for (int i = 0; i < _config.maxSlots; i++)
        {
            InventorySlot slot = new InventorySlot();
            slot.isLocked = i >= _config.defaultUnlocked;
            slot.slotsNumber = i;
            slot.itemSlot = null;
            slot.quantity = 0;
            _inventory.slots.Add(slot);
        }
    }

    public InventoryModel GetModel() => _inventory;

    public bool AddItem(ItemDefinition def, int amount = 1)
    {
        if (def == null || amount <= 0) return false;

        bool changed = false;
        int totalAdded = 0;

        if (def.maxStack <= 1)
        {
            for (int i = 0; i < _inventory.slots.Count; i++)
            {
                var s = _inventory.slots[i];
                if (!s.isLocked && s.itemSlot == null)
                {
                    s.itemSlot = def;
                    s.quantity = 1;
                    totalAdded = 1;
                    changed = true;
                    break;
                }
            }
        }
        else
        {
            int remaining = amount;
            for (int i = 0; i < _inventory.slots.Count && remaining > 0; i++)
            {
                var s = _inventory.slots[i];
                if (!s.isLocked && s.itemSlot == def && s.quantity < def.maxStack)
                {
                    int canAdd = def.maxStack - s.quantity;
                    int added = Mathf.Min(canAdd, remaining);
                    s.quantity += added;
                    remaining -= added;
                    totalAdded += added;
                    changed = true;
                }
            }
            for (int i = 0; i < _inventory.slots.Count && remaining > 0; i++)
            {
                var s = _inventory.slots[i];
                if (!s.isLocked && s.itemSlot == null)
                {
                    int added = Mathf.Min(def.maxStack, remaining);
                    s.itemSlot = def;
                    s.quantity = added;
                    remaining -= added;
                    totalAdded += added;
                    changed = true;
                }
            }
        }

        if (changed)
        {
            _inventory.totalWeight += totalAdded * def.weight;

            Debug.Log($"<color=cyan>Item Added:</color> {def.name} x{totalAdded}. Total Weight: {_inventory.totalWeight:F2}");
            inventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"<color=red>Failed to add {def.name}!</color> No free slots or inventory full.");
        }

        return changed;
    }

    public bool RemoveFromSlot(int slotIndex, int qty = 1)
    {
        if (slotIndex < 0 || slotIndex >= _inventory.slots.Count) return false;
        var s = _inventory.slots[slotIndex];

        if (s.itemSlot == null)
        {
            Debug.LogWarning($"Slot {slotIndex} is already empty.");
            return false;
        }

        string itemName = s.itemSlot.name;
        int actualRemoved = Mathf.Min(qty, s.quantity);

        _inventory.totalWeight -= actualRemoved * s.itemSlot.weight;

        if (_inventory.totalWeight < 0.0001f)
        {
            _inventory.totalWeight = 0f;
        }

        s.quantity -= actualRemoved;
        bool isCleared = s.quantity <= 0;

        if (isCleared)
        {
            s.itemSlot = null;
            s.quantity = 0;
        }

        Debug.Log($"<color=orange>Item Removed:</color> {itemName} x{actualRemoved} from slot {slotIndex}. Total Weight: {_inventory.totalWeight:F2}");
        if (isCleared) Debug.Log($"Slot {slotIndex} is now completely empty.");

        inventoryChanged?.Invoke();
        return true;
    }

    public bool UnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventory.slots.Count) return false;
        var s = _inventory.slots[slotIndex];

        if (!s.isLocked) return false;

        if (_inventory.coins >= _config.unlockPrice)
        {
            _inventory.coins -= _config.unlockPrice;
            s.isLocked = false;

            Debug.Log($"<color=yellow>Slot Unlocked!</color> Index: {slotIndex}. Spent: {_config.unlockPrice}. Remaining: {_inventory.coins}");
            inventoryChanged?.Invoke();
            return true;
        }

        Debug.LogWarning($"Not enough coins to unlock slot {slotIndex}!");
        return false;
    }

    public void AddCoins()
    {
        _inventory.coins += _config.defaultCoinsAdd;
        Debug.Log($"<color=yellow>Coins Added:</color> +{_config.defaultCoinsAdd}. Total: {_inventory.coins}");
        inventoryChanged?.Invoke();
    }
}
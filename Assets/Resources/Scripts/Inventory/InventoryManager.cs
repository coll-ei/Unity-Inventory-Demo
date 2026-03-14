using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class InventoryManager : MonoBehaviour
{
    private GameConfig _config;
    private ItemDatabase _database;
    private InventoryModel _inventory = new InventoryModel();

    public Action inventoryChanged;

    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory_save.json");

    [Inject]
    public void Initialize(GameConfig config, ItemDatabase database)
    {
        _config = config;
        _database = database;

        if (File.Exists(SavePath))
        {
            LoadInventory();
            Debug.Log("<color=green>Inventory Loaded from File!</color>");
        }
        else
        {
            CreateInitialInventory();
            _inventory.coins = _config.initialCoins;
            SaveInventory();
        }

        inventoryChanged += SaveInventory;

        Debug.Log($"<color=green>Inventory Initialized!</color> Coins: {_inventory.coins}, Slots: {_inventory.slots.Count}");
        inventoryChanged?.Invoke();
    }


    private void CreateInitialInventory()
    {
        if (_inventory == null) _inventory = new InventoryModel();

        if (_inventory.slots == null) _inventory.slots = new List<InventorySlot>();

        _inventory.slots.Clear();

        for (int i = 0; i < _config.maxSlots; i++)
        {
            _inventory.slots.Add(new InventorySlot
            {
                isLocked = i >= _config.defaultUnlocked,
                slotsNumber = i,
                itemSlot = null,
                quantity = 0
            });
        }
    }


    public bool AddItem(ItemDefinition def, int amount = 1)
    {
        if (def == null || amount <= 0) return false;

        int remaining = amount;
        bool changed = false;
        List<int> affectedSlots = new List<int>();

        if (def.maxStack > 1)
        {
            foreach (var s in _inventory.slots.Where(s => !s.isLocked && s.itemSlot == def && s.quantity < def.maxStack))
            {
                int added = Mathf.Min(def.maxStack - s.quantity, remaining);
                s.quantity += added;
                remaining -= added;
                changed = true;
                if (!affectedSlots.Contains(s.slotsNumber)) affectedSlots.Add(s.slotsNumber);
                if (remaining <= 0) break;
            }
        }

        while (remaining > 0)
        {
            var emptySlot = _inventory.slots.FirstOrDefault(s => !s.isLocked && s.itemSlot == null);
            if (emptySlot == null) break;

            int added = Mathf.Min(def.maxStack, remaining);
            emptySlot.itemSlot = def;
            emptySlot.quantity = added;
            remaining -= added;
            changed = true;
            affectedSlots.Add(emptySlot.slotsNumber);
        }

        if (changed)
        {
            string slotsInfo = string.Join(", ", affectedSlots);
            Debug.Log($"<color=cyan>Item Added:</color> {def.itemName} x{amount - remaining} into slots [{slotsInfo}]. Total Weight: {_inventory.TotalWeight:F2}");
            inventoryChanged?.Invoke();
            return true;
        }

        Debug.LogWarning("<color=red>Error:</color> No free slots available!");
        return false;
    }


    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        var slotFrom = _inventory.slots[fromIndex];
        var slotTo = _inventory.slots[toIndex];

        if (slotFrom.isLocked || slotTo.isLocked) return;
        if (slotFrom.itemSlot == null) return; 

        if (slotTo.itemSlot != null && slotTo.itemSlot == slotFrom.itemSlot && slotTo.itemSlot.maxStack > 1)
        {
            int canAdd = slotTo.itemSlot.maxStack - slotTo.quantity;
            int toAdd = Mathf.Min(canAdd, slotFrom.quantity);

            slotTo.quantity += toAdd;
            slotFrom.quantity -= toAdd;

            if (slotFrom.quantity <= 0) slotFrom.itemSlot = null;
        }
        else 
        {
            var tempItem = slotTo.itemSlot;
            int tempQty = slotTo.quantity;

            slotTo.itemSlot = slotFrom.itemSlot;
            slotTo.quantity = slotFrom.quantity;

            slotFrom.itemSlot = tempItem;
            slotFrom.quantity = tempQty;
        }

        inventoryChanged?.Invoke();
    }



    public void TryShoot()
    {
        var weapons = _inventory.slots
            .Where(s => s.itemSlot is WeaponItem)
            .Select(s => s.itemSlot as WeaponItem)
            .ToList();

        var ammoSlots = _inventory.slots
            .Where(s => s.itemSlot is AmmoDefinition && s.quantity > 0)
            .ToList();

        if (!ammoSlots.Any()) { Debug.LogError("<color=red>Shoot Error:</color> No ammo in inventory!"); return; }
        if (!weapons.Any()) { Debug.LogError("<color=red>Shoot Error:</color> No weapons in inventory!"); return; }

        var randomAmmoSlot = ammoSlots[UnityEngine.Random.Range(0, ammoSlots.Count)];
        var ammoDef = randomAmmoSlot.itemSlot as AmmoDefinition;

        var weapon = weapons.FirstOrDefault(w => w.ammosType == ammoDef.type);

        if (weapon != null)
        {
            randomAmmoSlot.quantity--;
            if (randomAmmoSlot.quantity <= 0) randomAmmoSlot.itemSlot = null;

            Debug.Log($"<color=orange>SHOT!</color> Weapon: {weapon.itemName} | Ammo: {ammoDef.itemName} | Damage: {weapon.damage}");
            inventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogError($"<color=red>Shoot Error:</color> No weapon found for {ammoDef.itemName}!");
        }
    }


    public void RemoveFromSlot(int index, int qty = -1)
    {
        if (index < 0 || index >= _inventory.slots.Count) return;
        var slot = _inventory.slots[index];

        if (slot.itemSlot == null) return;

        if (qty == -1 || qty >= slot.quantity)
        {
            slot.itemSlot = null;
            slot.quantity = 0;
        }
        else
        {
            slot.quantity -= qty;
        }

        inventoryChanged?.Invoke();
    }

    public void RemoveRandomItem()
    {
        var busySlots = _inventory.slots.Where(s => s.itemSlot != null).ToList();
        if (!busySlots.Any())
        {
            Debug.LogError("<color=red>Error:</color> All slots are empty!");
            return;
        }

        var randomSlot = busySlots[UnityEngine.Random.Range(0, busySlots.Count)];
        string removedName = randomSlot.itemSlot.itemName;
        int removedIndex = randomSlot.slotsNumber;

        randomSlot.itemSlot = null;
        randomSlot.quantity = 0;

        Debug.Log($"<color=orange>Removed:</color> {removedName} from slot {removedIndex}");
        inventoryChanged?.Invoke();
    }



    public void UnlockSlot(int index)
    {
        if (index < 0 || index >= _inventory.slots.Count) return;
        var slot = _inventory.slots[index];

        if (slot.isLocked && _inventory.coins >= _config.unlockPrice)
        {
            _inventory.coins -= _config.unlockPrice;
            slot.isLocked = false;
            Debug.Log($"<color=yellow>Slot {index} Unlocked!</color> Remaining coins: {_inventory.coins}");
            inventoryChanged?.Invoke();
        }
        else if (_inventory.coins < _config.unlockPrice)
        {
            Debug.LogWarning("<color=red>Error:</color> Not enough coins!");
        }
    }



    public void AddCoins()
    {
        _inventory.coins += _config.defaultCoinsAdd;
        Debug.Log($"<color=yellow>Coins Added:</color> +{_config.defaultCoinsAdd}. Total: {_inventory.coins}");
        inventoryChanged?.Invoke();
    }

    public InventoryModel GetModel() => _inventory;



    public void SaveInventory()
    {
        foreach (var slot in _inventory.slots)
        {
            slot.itemId = slot.itemSlot != null ? slot.itemSlot.id : "";
        }

        string json = JsonUtility.ToJson(_inventory, true);
        File.WriteAllText(SavePath, json);
    }

    private void LoadInventory()
    {
        string json = File.ReadAllText(SavePath);
        _inventory = JsonUtility.FromJson<InventoryModel>(json);

        foreach (var slot in _inventory.slots)
        {
            if (!string.IsNullOrEmpty(slot.itemId))
            {
                slot.itemSlot = _database.GetItemById(slot.itemId);
            }
        }
    }

    public void DeleteSaveData()
    {
        inventoryChanged -= SaveInventory;

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("<color=red>Save File Deleted!</color>");
        }

        CreateInitialInventory();

        if (_config != null) _inventory.coins = _config.initialCoins;

        inventoryChanged?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ButtonsFunction : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> _itemsConfig = new List<ItemDefinition>();
    [SerializeField] private List<ItemDefinition> _ammosConfig = new List<ItemDefinition>();

    private InventoryManager _inventory;
    private GameConfig _config;


    [Inject]
    public void Initialize(GameConfig config, InventoryManager inventory)
    {
        _config = config;
        _inventory = inventory;
    }

    private void FillInitialItems()
    {

        if (_itemsConfig != null && _itemsConfig.Count > 0)
        {

            foreach (var item in _itemsConfig)
            {
                if (item != null)
                {
                    _inventory.AddItem(item, 1);
                }
            }
        }

        if (_ammosConfig != null && _ammosConfig.Count > 0)
        {
            foreach (var ammo in _ammosConfig)
            {

                if (ammo != null)
                {
                    _inventory.AddItem(ammo, 1);
                }
            }
        }
    }

    public void AddRandomItem()
    {
        if (_itemsConfig == null || _itemsConfig.Count <= 0) return;
        var item = _itemsConfig[Random.Range(0, _itemsConfig.Count)];
        _inventory.AddItem(item, 1);
    }

    public void AddBullets()
    {
        foreach (var ammo in _ammosConfig)
        {
            _inventory.AddItem(ammo, _config.defaultAmmoAdd);
        }
    }


    public void RemoveRandomItem()
    {
        var allSlots = _inventory.GetModel().slots;
        List<int> busyIndices = new List<int>();

        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i].itemSlot != null)
                busyIndices.Add(i);
        }

        if (busyIndices.Count == 0)
        {
            Debug.LogWarning("<color=red>Error:</color> Cannot remove item! All slots are empty.");
            return;
        }

        int randomIdx = busyIndices[UnityEngine.Random.Range(0, busyIndices.Count)];
        var targetSlot = allSlots[randomIdx];


        _inventory.RemoveFromSlot(randomIdx, targetSlot.quantity);
    }

    public void Shoot()
    {
        var slots = _inventory.GetModel().slots;

        List<AmmoDefinition> availableAmmo = new List<AmmoDefinition>();
        List<WeaponItem> availableWeapons = new List<WeaponItem>();

        foreach (var slot in slots)
        {
            if (slot.itemSlot == null || slot.quantity <= 0)
                continue;

            if (slot.itemSlot is AmmoDefinition ammo)
                availableAmmo.Add(ammo);

            if (slot.itemSlot is WeaponItem weapon)
                availableWeapons.Add(weapon);
        }

        if (availableAmmo.Count == 0)
        {
            Debug.LogWarning("<color=red>Shoot failed!</color> No ammo in inventory.");
            return;
        }

        if (availableWeapons.Count == 0)
        {
            Debug.LogWarning("<color=red>Shoot failed!</color> No weapons in inventory.");
            return;
        }

        AmmoDefinition randomAmmo = availableAmmo[Random.Range(0, availableAmmo.Count)];

        WeaponItem validWeapon = null;

        foreach (var weapon in availableWeapons)
        {
            if (weapon.ammosType == randomAmmo.type)
            {
                validWeapon = weapon;
                break;
            }
        }

        if (validWeapon == null)
        {
            Debug.LogError($"<color=red>Shoot failed!</color> No weapon supports ammo type {randomAmmo.type}");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];

            if (slot.itemSlot == randomAmmo)
            {
                _inventory.RemoveFromSlot(i, 1);
                break;
            }
        }

        Debug.Log(
            $"<color=orange>SHOT!</color> " +
            $"Weapon: {validWeapon.itemName} | " +
            $"Ammo: {randomAmmo.itemName} | " +
            $"Damage: {validWeapon.damage}"
        );
    }
}

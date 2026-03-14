using UnityEngine;
using Zenject;

public class ButtonsFunction : MonoBehaviour
{
    private InventoryManager _inventory;
    private ItemDatabase _db;
    private GameConfig _config;

    [Inject]
    public void Construct(InventoryManager inventory, ItemDatabase db, GameConfig config)
    {
        _inventory = inventory;
        _db = db;
        _config = config;
    }

    public void AddRandomItem() => _inventory.AddItem(_db.GetRandomItem());

    public void AddBullets()
    {
        foreach (var ammo in _db.GetAllAmmoDefinitions())
            _inventory.AddItem(ammo, _config.defaultAmmoAdd);
    }

    public void Shoot() => _inventory.TryShoot();
    public void RemoveRandom() => _inventory.RemoveRandomItem();
    public void AddCoins() => _inventory.AddCoins();
}
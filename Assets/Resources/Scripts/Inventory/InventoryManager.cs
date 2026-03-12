using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] private GameConfig _config;
    private InventoryModel inventory = new InventoryModel();

    private void Start()
    {
        CreateInventory();
    }

    void CreateInventory()
    {
        for (int i = 0; i < _config.maxSlots; i++)
        {
            InventorySlot slot = new InventorySlot();

            if (i < _config.defaultUnlocked)
                slot.isLocked = false;
            else
                slot.isLocked = true;

            inventory.slots.Add(slot);
        }
    }
}
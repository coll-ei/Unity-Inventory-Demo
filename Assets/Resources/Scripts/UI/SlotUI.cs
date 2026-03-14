using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image _slotIcon;
    [SerializeField] private Text _slotName;
    [SerializeField] private Text _quantityText;
    [SerializeField] private GameObject _lockOverlay;

    private int _slotIndex;
    private InventoryManager _inventory;
    private GameConfig _config; 
    private PopUpPanel _popUp;


    [Inject]
    public void Initialize(GameConfig config, InventoryManager inventory, PopUpPanel popUp)
    {
        _config = config;
        _inventory = inventory;
        _popUp = popUp;
    }


    public void OnSlotClick()
    {
        var model = _inventory.GetModel();
        var slot = model.slots[_slotIndex];

        if (slot.itemSlot != null && !slot.isLocked)
        {
            _popUp.ShowTable(slot.itemSlot);
        }
    }


    public void Setup(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (_slotIcon != null) _slotIcon.sprite = null;
        if (_slotName != null) _slotName.text = "";
        if (_quantityText != null) _quantityText.text = "";
        if (_lockOverlay != null) _lockOverlay.SetActive(false);

        if (slot == null)
        {
            Debug.LogWarning("SlotUI.UpdateSlot: slot is null");
            return;
        }

        if (slot.isLocked)
        {
            if (_lockOverlay != null) _lockOverlay.SetActive(true);
            if (_slotName != null) _slotName.text = "Closed";
            if (_quantityText != null)
                _quantityText.text = $"Cost: {_config.unlockPrice}";

            //Debug.Log($"Slot {slot.slotsNumber} is locked");
            return;
        }

        if (slot.itemSlot == null)
        {
            if (_slotName != null) _slotName.text = "Empty";
            if (_quantityText != null) _quantityText.text = "";

            //Debug.Log($"Slot {slot.slotsNumber} is empty");
            return;
        }

        if (_slotIcon != null && slot.itemSlot.icon != null)
        {
            _slotIcon.sprite = slot.itemSlot.icon;
            _slotIcon.enabled = true;
        }

        if (_slotName != null) _slotName.text = slot.itemSlot.itemName ?? slot.itemSlot.name;

        if (_quantityText != null)
        {
            if (slot.itemSlot.maxStack > 1)
                _quantityText.text = $"Amount: {slot.quantity}";
            
            else
                _quantityText.text = "";
        }

        //Debug.Log($"Slot {slot.slotsNumber} updated: {slot.itemSlot.itemName}");
    }

    public void Unlock()
    {
        _inventory.UnlockSlot(_slotIndex);
    }
}
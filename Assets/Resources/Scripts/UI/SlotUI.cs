using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image _slotIcon;
    [SerializeField] private Text _slotName;
    [SerializeField] private Text _quantityText;
    [SerializeField] private GameObject _lockOverlay;

    private int _slotIndex;

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
                _quantityText.text = $"Cost: {GameManager.instance.config.unlockPrice}";

            Debug.Log($"Slot {slot.slotsNumber} is locked");
            return;
        }

        if (slot.itemSlot == null)
        {
            if (_slotName != null) _slotName.text = "Empty";
            if (_quantityText != null) _quantityText.text = "";

            Debug.Log($"Slot {slot.slotsNumber} is empty");
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

        Debug.Log($"Slot {slot.slotsNumber} updated: {slot.itemSlot.itemName}");
    }
}
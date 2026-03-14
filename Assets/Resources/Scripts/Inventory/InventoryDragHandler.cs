using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

public class InventoryDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]private Image _originalIcon;
    private Canvas _canvas;
    private GameObject _dragIcon;
    private int _slotIndex;

    private InventoryManager _inventory;

    [Inject]
    private void Initialize(InventoryManager inventory)
    {
        _inventory = inventory;
        _canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(int index) => _slotIndex = index;


    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = _inventory.GetModel().slots[_slotIndex];
        if (slot.isLocked || slot.itemSlot == null) { eventData.pointerDrag = null; return; }

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform);
        _dragIcon.transform.SetAsLastSibling(); 

        var img = _dragIcon.AddComponent<Image>();
        img.sprite = _originalIcon.sprite;
        img.raycastTarget = false;

        var rt = _dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = _originalIcon.GetComponent<RectTransform>().sizeDelta;


        _originalIcon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon != null) _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null) Destroy(_dragIcon);
        _originalIcon.color = Color.white;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool droppedOnSlot = false;

        foreach (var result in results)
        {
            var targetSlot = result.gameObject.GetComponentInParent<InventoryDragHandler>();
            if (targetSlot != null && targetSlot != this)
            {
                _inventory.SwapSlots(_slotIndex, targetSlot._slotIndex);
                droppedOnSlot = true;
                break;
            }
            if (targetSlot == this) { droppedOnSlot = true; break; }
        }

        if (!droppedOnSlot)
        {
            _inventory.RemoveFromSlot(_slotIndex, -1); // -1 или любая логика удаления всех предметов
            Debug.Log($"<color=red>Item dropped outside!</color> Slot {_slotIndex} cleared.");
        }
    }
}
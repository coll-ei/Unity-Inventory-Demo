using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform _gridParent;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Text _coinsText;
    [SerializeField] private Text _weightText;

    private List<SlotUI> _slotUIs = new List<SlotUI>();
    private InventoryManager _inventory;
    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer container, InventoryManager inventory)
    {
        _container = container;
        _inventory = inventory;

        _inventory.inventoryChanged += RefreshAll;

        BuildSlots();
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.inventoryChanged -= RefreshAll;
    }

    private void BuildSlots()
    {
        foreach (Transform t in _gridParent) Destroy(t.gameObject);
        _slotUIs.Clear();

        var model = _inventory.GetModel();
        for (int i = 0; i < model.slots.Count; i++)
        {
            var go = _container.InstantiatePrefab(_slotPrefab, _gridParent);

            var su = go.GetComponent<SlotUI>();
            su.Setup(i);
            _slotUIs.Add(su);

            var dragHandler = go.GetComponent<InventoryDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.Setup(i);
            }
        }
    }

    public void RefreshAll()
    {
        if (_inventory == null) return;

        var model = _inventory.GetModel();

        // Обновляем каждый слот данными из модели
        for (int i = 0; i < _slotUIs.Count; i++)
        {
            if (i < model.slots.Count)
                _slotUIs[i].UpdateSlot(model.slots[i]);
        }

        // Обновляем текстовые поля (деньги и общий вес)
        if (_coinsText != null)
            _coinsText.text = $"Coins: {model.coins}";

        if (_weightText != null)
            _weightText.text = $"Weight: {model.TotalWeight:F2} kg";
    }
}
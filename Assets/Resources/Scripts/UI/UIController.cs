using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform gridParent;      // content GridLayout
    [SerializeField] private GameObject slotPrefab;     // префаб с компонентом SlotUI
    [SerializeField] private UnityEngine.UI.Text coinsText;

    [SerializeField] private InventoryManager _inventory;

    private List<SlotUI> _slotUIs = new List<SlotUI>();

    public void OnStart()
    {
        BuildSlots();
        RefreshAll();
    }

    //private void FixedUpdate()
    //{
    //    RefreshAll();
    //}

    private void BuildSlots()
    {
        // Очистим контейнер, если что
        foreach (Transform t in gridParent) Destroy(t.gameObject);
        _slotUIs.Clear();

        var model = _inventory.GetModel();
        for (int i = 0; i < model.slots.Count; i++)
        {
            var go = Instantiate(slotPrefab, gridParent);
            var su = go.GetComponent<SlotUI>();
            su.Setup(i);
            _slotUIs.Add(su);
        }
    }

    // Вызывать после каждой операции
    public void RefreshAll()
    {

        if (_inventory == null) return;

        var model = _inventory.GetModel();
        for (int i = 0; i < _slotUIs.Count; i++)
        {
            _slotUIs[i].UpdateSlot(model.slots[i]);
        }

        if (coinsText != null)
            coinsText.text = $"Coins: {GameManager.instance.playerCoins}";
    }
}
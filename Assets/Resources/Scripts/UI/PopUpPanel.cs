using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPanel : MonoBehaviour
{
    [SerializeField] private GameObject _table;
    [SerializeField] private Image _popUpIcon;
    [SerializeField] private Text _popUpName;
    [SerializeField] private Text _popUpMaxStack;
    [SerializeField] private Text _popUpId;
    [SerializeField] private Text _popUpWeight;
    [SerializeField] private Text _popUpOptional;
    [SerializeField] private float _popUpTime = 3f; 

    private Coroutine _currentTimer; 

    public void ShowTable(ItemDefinition item)
    {
        if (item == null) return;

        if (_currentTimer != null) StopCoroutine(_currentTimer);

        _table.SetActive(true);

        if (_popUpIcon != null) _popUpIcon.sprite = item.icon;
        if (_popUpName != null) _popUpName.text = $"Name: {item.itemName}";
        if (_popUpId != null) _popUpId.text = $"ID: {item.id}";
        if (_popUpWeight != null) _popUpWeight.text = $"Weight: {item.weight} kg";
        if (_popUpMaxStack != null) _popUpMaxStack.text = $"Max Stack: {item.maxStack}";

        if (_popUpOptional != null)
        {
            if (item is WeaponItem weapon)
                _popUpOptional.text = $"Damage: {weapon.damage}\nAmmo: {weapon.ammosType}";
            else if (item is ArmorItem armor)
                _popUpOptional.text = $"Defense: {armor.defense}";
            else if (item is AmmoDefinition ammo)
                _popUpOptional.text = $"Type: {ammo.type}";
            else
                _popUpOptional.text = "No unique stats";
        }

        _currentTimer = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(_popUpTime);
        CloseTable();
        _currentTimer = null; 
    }

    public void CloseTable()
    {
        if (_table != null) _table.SetActive(false);
    }
}


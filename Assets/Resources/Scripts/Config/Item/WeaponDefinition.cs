using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Weapon")]
public class WeaponItem : ItemDefinition
{
    public int damage;

    public ItemType ammosType;
}

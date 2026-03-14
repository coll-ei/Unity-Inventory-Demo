using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Config/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemDefinition> allItems;

    public ItemDefinition GetItemById(string id) => allItems.FirstOrDefault(i => i.id == id);

    public ItemDefinition GetRandomItem()
    {
        var valid = allItems.Where(i => !(i is AmmoDefinition)).ToList();
        return valid.Count > 0 ? valid[Random.Range(0, valid.Count)] : null;
    }

    public List<AmmoDefinition> GetAllAmmoDefinitions() => allItems.OfType<AmmoDefinition>().ToList();
}
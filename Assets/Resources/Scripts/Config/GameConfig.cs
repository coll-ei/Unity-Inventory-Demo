using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Inventory")]
    public int maxSlots = 30;
    public int defaultUnlocked = 15;
    public int unlockPrice = 100;

    [Header("Gameplay")]
    public int initialCoins = 200;
    public int defaultAmmoAdd = 30;
}

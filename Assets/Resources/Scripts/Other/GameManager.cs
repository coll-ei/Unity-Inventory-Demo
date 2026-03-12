using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameConfig config;
    static public GameManager instance;

    public int playerCoins;

    private void Awake()
    {
        playerCoins = config.initialCoins;
        instance = this; 
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
    }
}

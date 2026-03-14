using System.ComponentModel;
using UnityEngine;
using Zenject;

public class ConfigInstaler : MonoInstaller
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private ItemDatabase _database;

    [SerializeField] private InventoryManager _inventory; 
    [SerializeField] private UIController _uiController;
    [SerializeField] private PopUpPanel _popUpPanel;

    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(_config).AsSingle();
        Container.Bind<ItemDatabase>().FromInstance(_database).AsSingle();

        Container.BindInstance(_inventory).AsSingle();
        Container.BindInstance(_popUpPanel).AsSingle();
        Container.BindInstance(_uiController).AsSingle();
    }
}
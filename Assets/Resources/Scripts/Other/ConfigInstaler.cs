using System.ComponentModel;
using UnityEngine;
using Zenject;

public class ConfigInstaler : MonoInstaller
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private InventoryManager _inventory; 
    [SerializeField] private UIController _uiController;   

    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(_config).AsSingle();

        Container.BindInstance(_inventory).AsSingle();

        Container.BindInstance(_uiController).AsSingle();
    }
}
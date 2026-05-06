using AssetLoader;
using Extensions;
using Installers;
using Saves;
using Services;
using StateMachine;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Infrastructure
{
    public class Menu_ActiveState : IState
    {
        private IStateChanger _stateChanger;
        private IGameStateChanger _gameStateChanger;
        private AllServices _services;

        private StaticDataService _staticDataService;
        private GlobalUserData _globalUserData;
        private IUiService uiServiceStack;
        private SaveLoadService _saveService;
        private AssetFactoryService _assetFactoryService;
        
        private MenuInstaller _menuInstaller;
        private MenuWindow menuWindow;
        private AsyncOperationHandle<GameObject> _installerHandle;

        public Menu_ActiveState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _gameStateChanger = gameStateChanger;
            _services = services;
        }

        public async void Enter()
        {
            _staticDataService = _services.Single<StaticDataService>();
            _globalUserData = _services.Single<GlobalUserData>();
            uiServiceStack = _services.Single<UiService>();
            _saveService = _services.Single<SaveLoadService>();
            _assetFactoryService = _services.Single<AssetFactoryService>();

            var prefab = await _assetFactoryService.Create(_staticDataService.AddressablesKeys.MenuInstallerAddress, Vector3.zero, Quaternion.identity);
            
            _menuInstaller = prefab.GetComponent<MenuInstaller>();
            
            _menuInstaller.Init(new MenuInstaller.Ctx
            {
                globalUserData = _globalUserData,
                UiService = uiServiceStack,
                saveService = _saveService,
                gameStateChanger = _gameStateChanger,
                assetFactoryService = _assetFactoryService,
                staticDataService = _staticDataService,
            });
            
            _services.Single<LoadingScreenService>().Hide();
        }

        public void Exit()
        {
            _services.Single<LoadingScreenService>().StartShow();
            _saveService.SaveAll();
            _saveService.ClearSavedObjects();
            
            _menuInstaller?.Dispose();
            _menuInstaller = null;
            
            if (_installerHandle.IsValid())
            {
                Addressables.Release(_installerHandle);
            }
        }
        
    }
}

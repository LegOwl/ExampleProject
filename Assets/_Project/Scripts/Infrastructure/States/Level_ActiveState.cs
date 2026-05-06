using AssetLoader;
using Installers;
using Saves;
using StateMachine;
using Services;
using UnityEngine;
using UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Infrastructure
{
    public class Level_ActiveState : IState
    {
        private IStateChanger _stateChanger;
        private IGameStateChanger _gameStateChanger;
        private StaticDataService _staticDataService;
        private GlobalUserData _globalUserData;
        private LevelInstaller _levelInstaller;
        private IUiService uiServiceStack;
        private SaveLoadService _saveService;
        private AssetFactoryService _assetFactoryService;
        private ParticleService particleService;
        private LoadingScreenService _loadingScreenService;
        private AllServices _services;
        private AsyncOperationHandle<GameObject> _installerHandle;

        public Level_ActiveState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
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
            particleService = _services.Single<ParticleService>();
            _loadingScreenService = _services.Single<LoadingScreenService>();
            _assetFactoryService = _services.Single<AssetFactoryService>();
            
            uiServiceStack.uiView.GameplayUi.Show();
            uiServiceStack.uiView.GameplayUi.ExitMenuBtn.SetOnClick(() => _gameStateChanger.Enter<MenuState>());
            
            var prefab = await _assetFactoryService.Create(_staticDataService.AddressablesKeys.LevelInstallerAddress, Vector3.zero, Quaternion.identity);
            
            _levelInstaller = prefab.GetComponent<LevelInstaller>();
            
            await _levelInstaller.Init(new LevelInstaller.Ctx
            {
                globalUserData = _globalUserData,
                UiService = uiServiceStack,
                saveService = _saveService,
                assetFactoryService = _assetFactoryService,
                gameStateChanger = _gameStateChanger,
                staticDataService = _staticDataService,
            });
            
            _loadingScreenService.Hide();
        }

        public void Exit()
        {
            _services.Single<LoadingScreenService>().StartShow();
            uiServiceStack.uiView.GameplayUi.Hide();
            _saveService.SaveAll();
            _saveService.ClearSavedObjects();

            _levelInstaller?.Dispose();
            _levelInstaller = null;
            
            if (_installerHandle.IsValid())
            {
                Addressables.Release(_installerHandle);
            }
        }
    }
}
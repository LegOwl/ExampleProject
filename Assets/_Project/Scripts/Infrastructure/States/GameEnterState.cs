using AssetLoader;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Saves;
using Services;
using Sound;
using StateMachine;
using UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameEnterState : IState
    {
        private readonly IGameStateChanger _stateChanger;
        private readonly AllServices _services;

        public GameEnterState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _services = services;
        }

        public async void Enter()
        {
            DOTween.SetTweensCapacity(500, 50);

            RegisterServices();

            await InitializeCoreServices();
            InitializeUIAndSound();
            _services.Single<SoundService>().PlayBackgroundMusic();
            _stateChanger.Enter<MenuState>();
        }

        public void Exit()
        {
            _services.Single<LoadingScreenService>().StartShow();
        }
        
        private void RegisterServices()
        {
            _services.RegisterSingle(new StaticDataService());
            _services.RegisterSingle(new SaveLoadService());
            _services.RegisterSingle(new GlobalUserData());
            _services.RegisterSingle(new UiService());
            _services.RegisterSingle(new LoadingScreenService());
            _services.RegisterSingle(new ParticleService());
            _services.RegisterSingle(new MessageService());
            _services.RegisterSingle(new SoundService());
            
            var assetProvider = new AddressablesProviderService();
            _services.RegisterSingle(assetProvider);

            var prefabFactory = new AssetFactoryService(assetProvider);
            _services.RegisterSingle(prefabFactory);
        }
        
        private async UniTask InitializeCoreServices()
        {
            var staticData = _services.Single<StaticDataService>();
            staticData.Initialize();

            var saveLoad = _services.Single<SaveLoadService>();
            saveLoad.Initialize(_services);

            var userData = _services.Single<GlobalUserData>();
            userData.Initialize(saveLoad);

            await _services.Single<ParticleService>().Initialize(staticData.Presets.particleCollection.particles);
        }

        private void InitializeUIAndSound()
        {
            var staticData = _services.Single<StaticDataService>();
            var guiView = Object.Instantiate(staticData.Prefabs.guiViewPrefab);
            Object.DontDestroyOnLoad(guiView);

            var guiService = _services.Single<UiService>();
            guiService.Initialize(new UiService.Ctx
            {
                view = guiView,
                globalUserData = _services.Single<GlobalUserData>()
            });
            
            var loading = _services.Single<LoadingScreenService>();
            loading.Initialize(guiService.GetWindow<LoadingWindow>());
            loading.StartShow();
            
            var soundHolder = Object.Instantiate(staticData.Prefabs.soundHolder);
            Object.DontDestroyOnLoad(soundHolder);

            _services.Single<SoundService>().Initialize(_services.Single<GlobalUserData>(), soundHolder);
            _services.Single<SaveLoadService>().InformProgressReaders();
        }
    }
}
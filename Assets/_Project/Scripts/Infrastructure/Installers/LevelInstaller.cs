using UnityEngine;
using System;
using AssetLoader;
using Level;
using Core;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Services;
using Lean.Pool;
using Saves;
using UI;
using UniRx;


namespace Installers
{
    public class LevelInstaller : MonoBehaviour, IDisposable
    {
        [SerializeField] private LevelView _levelView;

        public struct Ctx
        {
            public GlobalUserData globalUserData;
            public IUiService UiService;
            public SaveLoadService saveService;
            public IGameStateChanger gameStateChanger;
            public StaticDataService staticDataService;
            public AssetFactoryService assetFactoryService;
            public AddressablesProviderService addressablesProviderService;
        }
        
        private Ctx _ctx;

        private CompositeDisposable _disposables = new CompositeDisposable();
        
        public async UniTask Init(Ctx ctx)
        {
            _ctx = ctx;

            var levelController = new LevelController(new LevelController.Ctx
            {
                levelView = _levelView,
                factory = _ctx.assetFactoryService,
                staticDataService = _ctx.staticDataService,
                GameplayUi = _ctx.UiService.uiView.GameplayUi,
                globalUserData = _ctx.globalUserData,
            });
            await levelController.Load();
            _disposables.Add(levelController);
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
            LeanPool.DespawnAll();
            Destroy(gameObject);
        }
    }
}
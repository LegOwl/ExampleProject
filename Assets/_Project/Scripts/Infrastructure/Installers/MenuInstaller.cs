using AssetLoader;
using Core;
using Infrastructure;
using Lean.Pool;
using Menu;
using Saves;
using Services;
using UI;
using UniRx;
using UnityEngine;


namespace Installers
{
    public class MenuInstaller : MonoBehaviour
    {
        [SerializeField] private MenuView _menuView;

        public struct Ctx
        {
            public GlobalUserData globalUserData;
            public IUiService UiService;
            public SaveLoadService saveService;
            public IGameStateChanger gameStateChanger;
            public AssetFactoryService assetFactoryService;
            public StaticDataService staticDataService;
        }

        private Ctx _ctx;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public async void Init(Ctx ctx)
        {
            _ctx = ctx;
            
            var menuController = new MenuController(new MenuController.Ctx
            {
                menuView = _menuView,
                factory = _ctx.assetFactoryService,
                staticDataService = _ctx.staticDataService,
            });
            
            await menuController.Load();
            _disposables.Add(menuController);
            
            var menuWindow = _ctx.UiService.ShowWindow<MenuWindow>();
            _disposables.Add(menuWindow.OnHidden.SubscribeWithSkip(_ => _ctx.gameStateChanger.Enter<LevelState>()));
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
            LeanPool.DespawnAll();
            Destroy(gameObject);
        }
    }
}

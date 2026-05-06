using AssetLoader;
using Core;
using Cysharp.Threading.Tasks;
using Services;

namespace Menu
{
    public class MenuController : BaseDisposable
    {
        public struct Ctx
        {
            public MenuView menuView;
            public AssetFactoryService factory;
            public AddressablesProviderService addressablesProvider;
            public StaticDataService staticDataService;
        }
        
        public MenuController(Ctx ctx)
        {
            _ctx = ctx;
        }
        
        private Ctx _ctx;

        public async UniTask Load()
        {
            await _ctx.factory.Create(_ctx.staticDataService.AddressablesKeys.EnvironmentFirstAddress, _ctx.menuView.SpawnPoint.position, _ctx.menuView.SpawnPoint.rotation, _ctx.menuView.transform);
        }

        protected override void OnDispose()
        {
            _ctx.factory.Release(_ctx.staticDataService.AddressablesKeys.EnvironmentFirstAddress);
            base.OnDispose();
        }
    }
}

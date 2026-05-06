using AssetLoader;
using Core;
using Cysharp.Threading.Tasks;
using Example;
using Saves;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Level
{
    public class LevelController : BaseDisposable
    {
        public struct Ctx
        {
            public LevelView levelView;
            public AssetFactoryService factory;
            public StaticDataService staticDataService;
            public GameplayUi GameplayUi;
            public GlobalUserData globalUserData;
        }
        
        public LevelController(Ctx ctx)
        {
            _ctx = ctx;
        }

        private Ctx _ctx;
        private AsyncOperationHandle<GameObject> _handle;
        private GameObject _instance;
        private ReactiveEvent<string> _onSpin = new ReactiveEvent<string>();
        
        public async UniTask Load()
        {
            var env = await _ctx.factory.Create(_ctx.staticDataService.AddressablesKeys.EnvironmentSecondAddress,
                _ctx.levelView.SpawnPoint.position, 
                _ctx.levelView.SpawnPoint.rotation, 
                _ctx.levelView.transform);

            var obj = await _ctx.factory.Create(
                _ctx.staticDataService.AddressablesKeys.ExampleObject,
                _ctx.levelView.SpawnPointObject.position,
                _ctx.levelView.SpawnPointObject.rotation,
                _ctx.levelView.transform
            );

            var view = obj.GetComponent<ExampleObjectView>();

            var exampleObject = new ExampleObject(new ExampleObject.Ctx
            {
                exampleObjectView = view,
                onSpin = _onSpin,
                globalUserData = _ctx.globalUserData,
            });
            
            _ctx.GameplayUi.LinkedTextUI.SetLink(_onSpin);
            
            AddDispose(exampleObject);
        }

        protected override void OnDispose()
        {
            _ctx.factory.Release(_ctx.staticDataService.AddressablesKeys.EnvironmentSecondAddress);
            _ctx.factory.Release(_ctx.staticDataService.AddressablesKeys.ExampleObject);
            base.OnDispose();
        }
    }
}
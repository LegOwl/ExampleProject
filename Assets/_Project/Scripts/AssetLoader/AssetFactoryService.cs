using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace AssetLoader
{
    public class AssetFactoryService : IService
    {
        private readonly IAssetProvider _assets;

        public AssetFactoryService(IAssetProvider assets)
        {
            _assets = assets;
        }

        public async UniTask<GameObject> Create(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var prefab = await _assets.Load<GameObject>(address);

            if (prefab == null)
            {
                Debug.LogError($"Failed to create prefab: {address}");
                return null;
            }

            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public async UniTask<GameObject> Create(string address, Transform parent)
        {
            return await Create(address, parent.position, Quaternion.identity, parent);
        }

        public void Release(string address)
        {
            _assets.Release(address);
        }
    
    }
}
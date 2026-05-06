using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetLoader
{
    public class AddressablesProviderService : BaseDisposable, IAssetProvider, IService
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        public async UniTask<T> Load<T>(string address) where T : class
        {
            if (_handles.TryGetValue(address, out var cachedHandle))
            {
                return cachedHandle.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[AssetProvider] Failed to load: {address}");
                return null;
            }

            _handles[address] = handle;

            return handle.Result;
        }

        public void Release(string address)
        {
            if (_handles.TryGetValue(address, out var handle))
            {
                Addressables.Release(handle);
                _handles.Remove(address);
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
        }
    }
}

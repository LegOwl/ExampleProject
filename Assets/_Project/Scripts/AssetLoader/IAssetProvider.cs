using Cysharp.Threading.Tasks;

namespace AssetLoader
{
    public interface IAssetProvider
    {
        UniTask<T> Load<T>(string address) where T : class;
        void Release(string address);
        void ReleaseAll();
    }
}
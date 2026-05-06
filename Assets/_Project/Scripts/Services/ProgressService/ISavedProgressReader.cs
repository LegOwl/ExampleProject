using Data;

namespace Services
{
    public interface ISavedProgressReader
    {
        void LoadProgress(SaveLoadService saveService);
    }
}


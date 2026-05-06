namespace Services
{
    public interface ISaveProgressWriter : ISavedProgressReader
    {
        void WriteProgress(SaveLoadService saveService);
    }
}


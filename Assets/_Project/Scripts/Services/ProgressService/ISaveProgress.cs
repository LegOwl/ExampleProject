namespace Services
{
    public interface ISaveProgress: ISaveProgressWriter, ISavedProgressReader
    {
        public void ResetProgress();
    }
}


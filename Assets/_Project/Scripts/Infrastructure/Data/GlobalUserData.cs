using Core;
using Services;
using UniRx;

namespace Saves
{
    public class GlobalUserData : BaseDisposable, IService, ISaveProgress
    {
        [System.Serializable]
        public class Stats : SaveData
        {
            public int spins;
            public long lastExitTime;
            public SettingsData settingsData;
            
            public Stats()
            {
                spins = 0;
                lastExitTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                settingsData = new SettingsData();
            }
        }

        [System.Serializable]
        public class SettingsData
        {
            public bool soundOn = true;
            public float soundValue = 1f;
            public bool musicOn = true;
            public float musicValue = 1f;
            public bool effectOn = true;
            public int sensitivityValue = 5;
        }
        
        public bool IsInitialStart { get; set; }
        public ReactiveProperty<int> spins { get; private set; } = new();
        public ReactiveProperty<long> lastExitTime { get; private set; } = new();
        
        private SaveLoadService _saveService;
        private Stats _data;
        private bool _loaded;
        

        public GlobalUserData()
        {
            _loaded = false;
            IsInitialStart = true;
        }

        public void Initialize(SaveLoadService saveService)
        {
            _saveService = saveService;

            _saveService.LoadData(this);

            InitValues();

            InitSettingsData();
        }

        private void InitValues()
        {
            spins.Value = _data.spins;
            lastExitTime.Value = _data.lastExitTime;

            AddDispose(spins.Skip(1).Subscribe(value =>
            {
                _data.spins = value;
                _saveService.SaveProgress(this);
            }));
            AddDispose(lastExitTime.Skip(1).Subscribe(value =>
            {
                _data.lastExitTime = value;
                _saveService.SaveProgress(this);
            }));
        }
        
        
        private void InitSettingsData()
        {
            soundOn.Value = _data.settingsData.soundOn;
            soundValue.Value = _data.settingsData.soundValue;

            musicOn.Value = _data.settingsData.musicOn;
            musicValue.Value = _data.settingsData.musicValue;

            sensivityValue.Value = _data.settingsData.sensitivityValue;

            AddDispose(soundOn.Skip(1).Subscribe(value =>
            {
                _data.settingsData.soundOn = value;
                _saveService.SaveProgress(this);
            }));
            AddDispose(musicOn.Skip(1).Subscribe(value =>
            {
                _data.settingsData.musicOn = value;
                _saveService.SaveProgress(this);
            }));
            AddDispose(soundValue.Skip(1).Subscribe(value =>
            {
                _data.settingsData.soundValue = value;
                _saveService.SaveProgress(this);
            }));
            AddDispose(musicValue.Skip(1).Subscribe(value =>
            {
                _data.settingsData.musicValue = value;
                _saveService.SaveProgress(this);
            }));
            AddDispose(sensivityValue.Skip(1).Subscribe(value =>
            {
                _data.settingsData.sensitivityValue = value;
                _saveService.SaveProgress(this);
            }));
        }

        #region Sound and vibro   

        public ReactiveProperty<bool> soundOn { get; private set; } = new();
        public ReactiveProperty<bool> musicOn { get; private set; } = new();
        public ReactiveProperty<bool> tapticOn { get; private set; } = new();
        public ReactiveProperty<int> sensivityValue { get; private set; } = new();
        public ReactiveProperty<float> soundValue { get; private set; } = new();
        public ReactiveProperty<float> musicValue { get; private set; } = new();

        #endregion
        
        #region SAVES

        public void WriteProgress(SaveLoadService saveService)
        {
            saveService.SetData(Saves.SavesDataId.GlobalUserData, _data);
        }

        public void LoadProgress(SaveLoadService saveService)
        {
            if (_loaded) return;

            if (saveService.GetSaveData(Saves.SavesDataId.GlobalUserData, out _data) == false)
            {
                _data = new();

                saveService.SaveProgress(this);
            }

            _loaded = true;
        }

        public void ResetProgress()
        {
            _data = new Stats();
            
            spins.Value = _data.spins;
            lastExitTime.Value = _data.lastExitTime;
            
            // Settings
            soundOn.Value = _data.settingsData.soundOn;
            soundValue.Value = _data.settingsData.soundValue;
            musicOn.Value = _data.settingsData.musicOn;
            musicValue.Value = _data.settingsData.musicValue;
            sensivityValue.Value = _data.settingsData.sensitivityValue;
            

            _saveService.SaveProgress(this);
        }
        #endregion
    }
}

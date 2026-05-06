using StaticData;
using UnityEngine;

namespace Services
{
    public class StaticDataService : IService
    {
        private const string PresetPath = "StaticData/PresetStaticData";
        private const string PrefabStaticDataPath = "StaticData/PrefabStaticData";
        private const string GameModeInstallersStaticDataPath = "StaticData/AddressablesKeysStaticData";
        
        public PrefabStaticData Prefabs { get; set; }
        public PresetStaticData Presets { get; set; }
        public AddressablesKeysStaticData AddressablesKeys { get; set; }
        

        public void Initialize()
        {
            LoadPrefabs();
            LoadPresets();
            LoadGameModeInstallersStaticData();
        }
        
        private void LoadPrefabs()
        {
            Prefabs = Resources.Load<PrefabStaticData>(PrefabStaticDataPath);
        }
        
        private void LoadPresets()
        {
            Presets = Resources.Load<PresetStaticData>(PresetPath);
        }
        
        private void LoadGameModeInstallersStaticData()
        {
            AddressablesKeys = Resources.Load<AddressablesKeysStaticData>(GameModeInstallersStaticDataPath);
        }
        
    }
}
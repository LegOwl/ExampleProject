using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "AddressablesKeysStaticData", menuName = "ScriptableObjects/StaticData/AddressablesKeysStaticData")]
    public class AddressablesKeysStaticData : ScriptableObject
    {
        [Header("Addressables Keys")]
        [SerializeField] private string _levelInstallerAddress = "Installers/LevelInstaller";
        [SerializeField] private string _menuInstallerAddress = "Installers/MenuInstaller";
        [SerializeField] private string environmentFirstAddres = "Example/Environment_1";
        [SerializeField] private string environmentSecondAddres = "Example/Environment_2";
        [SerializeField] private string _exampleObject = "Example/ExampleObject";
    
        public string LevelInstallerAddress => _levelInstallerAddress;
        public string MenuInstallerAddress => _menuInstallerAddress;
        public string EnvironmentFirstAddress => environmentFirstAddres;
        public string EnvironmentSecondAddress => environmentSecondAddres;
        public string ExampleObject => _exampleObject;
    
        public bool HasAddress(string address) => !string.IsNullOrWhiteSpace(address);
    }
}

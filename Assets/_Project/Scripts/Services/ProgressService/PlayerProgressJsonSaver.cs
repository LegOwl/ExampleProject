using System.IO;
using Data;
using Extensions;
using Newtonsoft.Json;
using Saves;
using UnityEngine;

namespace Services
{
    public class PlayerProgressJsonSaver : ISaverData<PlayerProgress>
    {
        private readonly string _fullPath;
        private readonly string _directory;

        public PlayerProgressJsonSaver(string profileId)
        {
            _directory = Application.persistentDataPath + "/";
            Directory.CreateDirectory(_directory);
            _fullPath = _directory + profileId + ".json";
        }

        public void Save(PlayerProgress data)
        {
            if (data == null) return;
            var dto = data.ToFileData();
            var json = JsonConvert.SerializeObject(dto, Formatting.Indented);
            File.WriteAllText(_fullPath, json);
        }

        public PlayerProgress Load()
        {
            if (!File.Exists(_fullPath))
            {
                DebugLogger.LogWarning($"PlayerProgressJsonSaver: file not found: {_fullPath}");
                return null;
            }
            var json = File.ReadAllText(_fullPath);
            var dto = JsonConvert.DeserializeObject<PlayerProgressFileData>(json);
            if (dto == null) return null;
            var progress = new PlayerProgress();
            progress.LoadFromFileData(dto);
            return progress;
        }

        public bool HasData()
        {
            return File.Exists(_fullPath);
        }

        public void DeleteSave()
        {
            if (File.Exists(_fullPath))
                File.Delete(_fullPath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Data;
using Saves;
using UnityEngine;

namespace Services
{
    [System.Serializable]
    public abstract class SaveData
    {
        public string Version;
    }

    public class SaveLoadService: IService
    {
        private const string SAVE_PROGRESS = "GLOBAL_SAVES";

        private ISaverData<PlayerProgress> _saveSystem;
        private PlayerProgress _progress;
        private AllServices _services;
        private List<ISaveProgressWriter> _savedObjects;
        private HashSet<ISaveProgressWriter> _pendingWriters;
        private IDisposable _pendingFlushDisposable;
        private const float FlushDebounceSeconds = 0.4f;
        
        public bool IsFirstPlay { get; private set; }

        public void Initialize(AllServices services, string profileId = SAVE_PROGRESS)
        {
            _services = services;
            _saveSystem = new PlayerProgressJsonSaver(profileId);
            _savedObjects = new();
            _pendingWriters = new HashSet<ISaveProgressWriter>();

            _progress = LoadProgress();
            if (_progress == null)
                _progress = TryMigrateFromLegacyDat(profileId);
            IsFirstPlay = _progress == null;
            if (IsFirstPlay)
                _progress = new PlayerProgress();
        }

        private PlayerProgress TryMigrateFromLegacyDat(string profileId)
        {
            var legacyPath = Application.persistentDataPath + "/" + profileId + ".dat";
            if (!File.Exists(legacyPath))
                return null;
            var legacySaver = new BinarySaverData<PlayerProgress>(profileId);
            var data = legacySaver.Load();
            if (data == null)
                return null;
            _saveSystem.Save(data);
            legacySaver.DeleteSave();
            return data;
        }

        public void LoadData(ISaveProgressWriter savedObject)
        {
            if (!_services.ProgressReaders.Contains(savedObject) && !_savedObjects.Contains(savedObject)) _savedObjects.Add(savedObject);

            savedObject.LoadProgress(this);
        }

        private PlayerProgress LoadProgress()
        {
            if (_saveSystem.HasData())
            {
                var data = _saveSystem.Load();
                return data;
            }
            return null;
        }

        public void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _services.ProgressReaders)
            {
                progressReader.LoadProgress(this);
            }
        }

        public void FlushPendingSaves()
        {
            _pendingFlushDisposable?.Dispose();
            _pendingFlushDisposable = null;

            foreach (ISaveProgressWriter writer in _pendingWriters)
            {
                writer.WriteProgress(this);
            }
            
            _pendingWriters.Clear();
            _saveSystem.Save(_progress);
        }

        public void SaveAll()
        {
            foreach (ISaveProgressWriter writer in _services.ProgressWriters)
            {
                if (_pendingWriters.Contains(writer)) continue;

                writer.WriteProgress(this);
            }

            for (int i = 0; i < _savedObjects.Count; i++)
            {
                if (_pendingWriters.Contains(_savedObjects[i])) continue;

                _savedObjects[i].WriteProgress(this);
            }

            FlushPendingSaves();
        }

        public void SaveProgress(ISaveProgressWriter progressWriter)
        {
            if (!_savedObjects.Contains(progressWriter) && !_services.ProgressWriters.Contains(progressWriter)) _savedObjects.Add(progressWriter);

            _pendingWriters.Add(progressWriter);
        }

        public void ResetProgress()
        {
            _pendingFlushDisposable?.Dispose();
            _pendingFlushDisposable = null;
            _pendingWriters.Clear();

            _progress.DeleteAllProgress();

            for (int i = 0; i < _services.ProgressWriters.Count; i++)
            {
                Debug.Log(_services.ProgressWriters[i]);
                if (_services.ProgressWriters[i] is ISaveProgress saveProgress)
                {
                    saveProgress.ResetProgress();
                }
            }

            for (int i = 0; i < _savedObjects.Count; i++)
            {
                if (_savedObjects[i] is ISaveProgress saveProgress)
                {
                    saveProgress.ResetProgress();
                }
            }
            
            _saveSystem.Save(_progress);
        }

        public void SetData<T>(string ID, T data) where T : SaveData
        {
            data.Version = Application.version;
            _progress.SetData(ID, data);
        }

        public bool GetSaveData<T>(string ID, out T data) where T : SaveData
        {
            return _progress.GetSaveData(ID, out data);
        }

        public void ClearAll()
        {
            _pendingFlushDisposable?.Dispose();
            _pendingFlushDisposable = null;
            _savedObjects?.Clear();
            _pendingWriters?.Clear();
        }

        public void ClearSavedObjects()
        {
            _savedObjects?.Clear();
            _pendingWriters?.Clear();
        }
    }
}
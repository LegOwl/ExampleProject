using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Extensions;
using UnityEngine;

namespace Saves
{
    public interface ISaverData<T> where T : class
    {
        void Save(T data);
        T Load();
        bool HasData();
        void DeleteSave();
    }

    public class BinarySaverData<T> : ISaverData<T> where T : class
    {
        private readonly string _fileName = "GameData.dat";
        private readonly string _saveDirectory;
        private readonly string _fullPath;
        private readonly BinaryFormatter _formatter;

        public BinarySaverData(string fileName)
        {
            _fileName = fileName + ".dat";
            _saveDirectory = Application.persistentDataPath + "/";
            Directory.CreateDirectory(_saveDirectory);
            _fullPath = _saveDirectory + _fileName;
            _formatter = GetFormatter();
        }

        public void Save(T data)
        {
            var savePath = _saveDirectory + _fileName;
            using (var stream = File.Create(savePath))
            {
                _formatter.Serialize(stream, data);
            }
        }

        public T Load()
        {
            if (File.Exists(_fullPath) == false)
            {
                DebugLogger.LogWarning($"SaverData, load, not find file: {_fullPath}");
                return null;
            }

            T data;
            using (var file = File.Open(_fullPath, FileMode.Open))
            {
                object loadData = _formatter.Deserialize(file);
                data = (T)loadData;
            }
            return data;
        }

        private BinaryFormatter GetFormatter()
        {
            var formatter = new BinaryFormatter();

            var surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Serializer());

            formatter.SurrogateSelector = surrogateSelector;
            return formatter;
        }

        public void DeleteSave()
        {
            if (File.Exists(_fullPath))
            {
                File.Delete(_fullPath);
            }
        }

        public bool HasData()
        {
            return File.Exists(_fullPath);
        }
    }



    public class Vector3Serializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 target = (Vector3)obj;
            info.AddValue("x", target.x);
            info.AddValue("y", target.y);
            info.AddValue("z", target.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 target = (Vector3)obj;
            target.x = (float)info.GetValue("x", typeof(float));
            target.y = (float)info.GetValue("y", typeof(float));
            target.z = (float)info.GetValue("z", typeof(float));
            return target;
        }
    }
}
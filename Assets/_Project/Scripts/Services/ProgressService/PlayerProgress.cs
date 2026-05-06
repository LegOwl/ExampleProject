using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class PlayerProgressFileData
    {
        public class Entry
        {
            public string Id;
            public string Json;
        }
        public List<Entry> Elements;
    }

    [System.Serializable]
    public class PlayerProgress
    {
        [System.Serializable]
        private class Element
        {
            public string ID;
            public string JSON;
        }
        private List<Element> _elements;

        public PlayerProgress()
        {
            _elements = new List<Element>();
        }

        public PlayerProgressFileData ToFileData()
        {
            var dto = new PlayerProgressFileData { Elements = new List<PlayerProgressFileData.Entry>(_elements.Count) };
            for (int i = 0; i < _elements.Count; i++)
                dto.Elements.Add(new PlayerProgressFileData.Entry { Id = _elements[i].ID, Json = _elements[i].JSON });
            return dto;
        }

        public void LoadFromFileData(PlayerProgressFileData data)
        {
            if (data?.Elements == null) return;
            _elements.Clear();
            for (int i = 0; i < data.Elements.Count; i++)
                _elements.Add(new Element { ID = data.Elements[i].Id, JSON = data.Elements[i].Json ?? string.Empty });
        }

        public bool GetSaveData<T>(string ID, out T data) where T : class
        {
            data = null;
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].ID == ID)
                {
                    data = JsonUtility.FromJson<T>(_elements[i].JSON);
                    return true;
                }
            }
            return false;
        }

        public void SetData<T>(string ID, T data) where T : class
        {
            var json = JsonUtility.ToJson(data);
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].ID == ID)
                {
                    _elements[i].JSON = json;
                    return;
                }
            }

            _elements.Add(new Element { ID = ID, JSON = json });
            return;
        }

        public void DeleteAllProgress()
        {
            for (int i = 0; i < _elements.Count; i++)
                _elements[i].JSON = null;
            _elements.Clear();
        }
    }
    
}

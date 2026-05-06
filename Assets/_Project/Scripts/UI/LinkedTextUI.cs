using System;
using Core;
using TMPro;
using UniRx;
using UnityEngine;

namespace UI
{
    public class LinkedTextUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private ReactiveEvent<string> _onChangeText;
        private IDisposable _onChangeTextDisposable;
        private string _baseText;

        protected void Awake()
        {
            _baseText = _text.text;
        }
        public void SetLink(ReactiveEvent<string> onChangeText)
        {
            _onChangeText =  onChangeText;
            _onChangeTextDisposable?.Dispose();
            _onChangeTextDisposable = _onChangeText.Subscribe(UpdateText);
        }

        private void UpdateText(string newText)
        {
            _text.SetText(newText);
        }

        public void Dispose()
        {
            _text.SetText(_baseText);
            _onChangeTextDisposable?.Dispose();
        }
    }
}

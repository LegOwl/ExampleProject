using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.UI.ControlElements;
using UniRx;

namespace UI
{
    public class BaseWindow : MonoBehaviour
    {
        public ReactiveEvent<BaseWindow> OnHidden = new ReactiveEvent<BaseWindow>();
        public ReactiveEvent OnHideBuyCloseButton = new ReactiveEvent();
        public ReactiveEvent<BaseWindow> OnHideStart = new ReactiveEvent<BaseWindow>();
        public ReactiveEvent<BaseWindow> OnBeginShow = new ReactiveEvent<BaseWindow>();
        public ReactiveEvent<BaseWindow> OnShow = new ReactiveEvent<BaseWindow>();
        public bool HideStarted => _hideStarted;
        
        [SerializeField] protected RectTransform _mainView;
        [SerializeField] protected BasicButton _closeButton;
        [SerializeField] protected float _showTime;

        protected Vector3 shownPos;
        protected Vector3 hiddenPos;
        protected WaitForSecondsRealtime waitForSecondsRealtime;
        protected bool _hideStarted = false;
        protected List<IDisposable> _disposables = new List<IDisposable>();
        protected bool _shown = false;
        
        protected virtual void Start()
        {
            shownPos = _mainView.anchoredPosition;
            hiddenPos = shownPos - new Vector3(0.0f, _mainView.rect.size.y);
            
            if (_closeButton)
                _closeButton.onClick.AddListener(() => { _closeButton.enabled = false; OnHideBuyCloseButton?.Notify(); Hide(); });

            waitForSecondsRealtime = new WaitForSecondsRealtime(_showTime);

            gameObject.SetActive(true);
        }

        protected virtual void OnDestroy()
        {
            List<IDisposable> disposables = _disposables;
            for (int i = _disposables.Count - 1; i >= 0; i--)
                _disposables[i]?.Dispose();
            disposables.Clear();
        }

        public virtual void Hide()
        {
            if (!_shown)
                return;
            
            _hideStarted = true;
            
            if (!gameObject.activeSelf)
            {
                HideInstant();
                return;
            }

            StartCoroutine(DoHide());
        }

        private void HideInstant()
        {
            if (!_shown)
                return;

            OnHideStart?.Notify(this);
            _mainView.anchoredPosition = hiddenPos;
            _shown = false;
            OnHidden?.Notify(this);
            gameObject.SetActive(false);
            _hideStarted = false;
        }

        private IEnumerator DoHide()
        {
            PrepareHide();

            if (_showTime <= 0)
            {
                _mainView.anchoredPosition = hiddenPos;
            }
            else
            {
                _mainView.DOAnchorPos(hiddenPos, _showTime).SetUpdate(true).SetLink(gameObject);
                yield return waitForSecondsRealtime;
            }
            UpdateAfterHide();
            
        }
        
        public virtual float Show()
        {
            if (_shown)
                return 0.0f;
            
            if (_closeButton)
                _closeButton.enabled = true;

            gameObject.SetActive(true);
            _shown = true;
            OnBeginShow?.Notify(this);
            Prepare();
            if (_showTime <= 0f)
            {
                _mainView.anchoredPosition = shownPos;
                OnShow?.Notify(this);
                UpdateAfterShow();

                return 0f;
            }
            else
                _mainView.DOAnchorPos(shownPos, _showTime).OnComplete(() => { OnShow?.Notify(this); UpdateAfterShow(); }).SetUpdate(true).SetLink(gameObject);

            return _showTime;
        }
        protected virtual void UpdateAfterHide()
        {
            _shown = false;
            OnHidden?.Notify(this);
            gameObject.SetActive(false);
            _hideStarted = false;
        }

        protected virtual void Prepare() { }
        protected virtual void UpdateAfterShow() { }
        protected virtual void PrepareHide() => OnHideStart?.Notify(this);        
        public virtual void ResetWindow() { }
        protected void AddDispose(IDisposable disposable) => _disposables.Add(disposable);
    }
}
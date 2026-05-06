using Core;
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
using Extensions;
using Saves;
using Services;

namespace UI
{
    public class UiService : BaseDisposable, IUiService, IService
    {

        public struct Ctx
        {
            public UIView view;    
            public GlobalUserData globalUserData;
        }

        public struct Events
        {
            public ReactiveEvent<bool> toggleBackground;
            public ReactiveEvent<bool> toggleCursor;
            public ReactiveEvent<bool> toggleInput;
            public ReactiveEvent pauseGame;
            public ReactiveEvent<BaseWindow> onWindowHidden;
        }

        private Ctx _ctx;
        private List<BaseWindow> _stackWindows = new List<BaseWindow>();
        private BaseWindow _activeWindow = null;
        private Tween _backgroundFadeTween;
        public Events events { get; private set; }
        public UIView uiView => _ctx.view;   

        private Dictionary<Type, BaseWindow> _windowInstances = new Dictionary<Type, BaseWindow>();        
        private Dictionary<Type, GameObject> _windowPrefabs = new Dictionary<Type, GameObject>();
        
        private readonly HashSet<BaseWindow> _windowsOpenForRateUsGate = new HashSet<BaseWindow>();

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;

            events = new Events
            {
                pauseGame = new(),
                toggleBackground = new(),
                toggleCursor = new(),
                toggleInput = new(),
                onWindowHidden = new ReactiveEvent<BaseWindow>()
            };

            Subscribe();
        }

        private void Subscribe()
        {
            AddDispose(events.toggleCursor?.SubscribeWithSkip(ToggleCursor));
            AddDispose(events.toggleInput?.SubscribeWithSkip(enabled =>
            {
                DebugLogger.Log("SetEnabled1: " + enabled);
            }));

        }
        
        #region Stack Windows
        private BaseWindow GetOrCreateWindow<T>() where T : BaseWindow
        {
            Type windowType = typeof(T);

            if (_windowInstances.ContainsKey(windowType))
                return _windowInstances[windowType];
            
            if (!_windowPrefabs.ContainsKey(windowType))
            {
                string prefabPath = $"Windows/{windowType.Name}";
                GameObject prefab = Resources.Load<GameObject>(prefabPath);

                if (prefab == null)
                {
                    DebugLogger.LogError($"Window prefab not found at path: Resources/{prefabPath}");
                    return null;
                }

                _windowPrefabs[windowType] = prefab;
            }

            GameObject windowObject = UnityEngine.Object.Instantiate(_windowPrefabs[windowType], _ctx.view.WindowsParent);
            BaseWindow windowInstance = windowObject.GetComponent<T>();

            if (windowInstance == null)
            {
                DebugLogger.LogError($"Window prefab {windowType.Name} doesn't have component of type {windowType.Name}");
                UnityEngine.Object.Destroy(windowObject);
                return null;
            }
            
            windowInstance.gameObject.SetActive(false);
            windowInstance.OnBeginShow.SubscribeWithSkip(TrackWindowBeganShow);
            windowInstance.OnHidden.SubscribeWithSkip(w =>
            {
                UntrackWindowClosed(w);
                ShowNextWindow(w);
            });

            windowInstance.OnHidden.SubscribeWithSkip((w) => events.onWindowHidden?.Notify(w));
                        
            _windowInstances[windowType] = windowInstance;

            return windowInstance;
        }

        public T GetWindow<T>() where T : BaseWindow
        {
            return GetOrCreateWindow<T>() as T;
        }

        public void PreloadWindow<T>() where T : BaseWindow
        {
            GetOrCreateWindow<T>();
        }

        public void DestroyWindow<T>() where T : BaseWindow
        {
            Type windowType = typeof(T);

            if (_windowInstances.ContainsKey(windowType))
            {
                BaseWindow window = _windowInstances[windowType];

                _stackWindows.Remove(window);
                
                if (_activeWindow == window)
                    _activeWindow = null;

                _windowsOpenForRateUsGate.Remove(window);

                if (window != null && window.gameObject != null)
                    UnityEngine.Object.Destroy(window.gameObject);

                _windowInstances.Remove(windowType);
            }
        }

        private void TrackWindowBeganShow(BaseWindow w)
        {
            if (w != null)
                _windowsOpenForRateUsGate.Add(w);
        }

        private void UntrackWindowClosed(BaseWindow w)
        {
            if (w != null)
                _windowsOpenForRateUsGate.Remove(w);
        }

        public bool HasOpenGuiWindow() => _windowsOpenForRateUsGate.Count > 0;

        private void ShowNextWindow(BaseWindow baseWindow = null)
        {
            if (_activeWindow != null && _activeWindow != baseWindow)
                return;

            if (_stackWindows.Count == 0)
            {
                _activeWindow = null;
                return;
            }

            _stackWindows[0].Show();
            _activeWindow = _stackWindows[0];
            _stackWindows.RemoveAt(0);
        }

        private void ToggleCursor(bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
        }

        private void ForceWindow<T>() where T : BaseWindow
        {
            ClearStackWindows();

            BaseWindow window = GetOrCreateWindow<T>();
            if (window != null)
            {
                window.Show();
                _activeWindow = window;
            }
        }

        private void ClearStackWindows()
        {
            _stackWindows.Clear();
            if (_activeWindow && !_activeWindow.HideStarted)
                _activeWindow.Hide();
        }

        private bool WindowInStack<T>() where T : BaseWindow
        {
            Type windowType = typeof(T);

            for (int i = 0; i < _stackWindows.Count; i++)
            {
                if (_stackWindows[i].GetType() == windowType)
                    return true;
            }

            return false;
        }

        public T ShowWindow<T>(bool insert = false, bool forceShow = false) where T : BaseWindow
        {
            if (WindowInStack<T>() || (_activeWindow != null && _activeWindow.GetType() == typeof(T)))
                return _activeWindow as T;
            BaseWindow window = GetOrCreateWindow<T>();
            if (window == null)
                return null;

            if (insert)
                _stackWindows.Insert(0, window);
            else
                _stackWindows.Add(window);

            if (_activeWindow == null)
                ShowNextWindow();
            else if (forceShow)
            {
                _stackWindows.Insert(1, _activeWindow);
                _activeWindow.Hide();
            }
            return _activeWindow as T;
        }
        
        public void ShowWindowOver<T>() where T : BaseWindow
        {
            if (_activeWindow != null && _activeWindow.GetType() == typeof(T))
                return;

            BaseWindow window = GetOrCreateWindow<T>();
            if (window != null)
            {
                window.Show();
            }
        }
        
        public void ClearAllWindows()
        {
            ClearStackWindows();

            foreach (var windowPair in _windowInstances)
            {
                if (windowPair.Value != null && windowPair.Value.gameObject != null)
                    UnityEngine.Object.Destroy(windowPair.Value.gameObject);
            }

            _windowInstances.Clear();
            _activeWindow = null;
            _windowsOpenForRateUsGate.Clear();
        }
        #endregion
    }
}
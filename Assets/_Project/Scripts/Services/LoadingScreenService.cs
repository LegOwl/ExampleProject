using System;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class LoadingScreenService : IService
    {
        public bool IsLoadingScreenEnabled { get; set; } = true;
        public float Progress { get; private set; }

        private LoadingWindow _window;
        private bool _isShowing;
        private bool _isHiding;
        private IDisposable _animationDisposable;

        private Material _material;
        private static readonly int AlphaID = Shader.PropertyToID("_Alpha");

        public void Initialize(LoadingWindow window)
        {
            _window = window;
            _isShowing = false;
            _isHiding = false;
            Progress = 0f;

            _animationDisposable?.Dispose();
            _animationDisposable = null;

            if (_window == null)
                return;

            if (_window.background != null)
            {
                _window.background.alpha = 1f;
                _window.background.gameObject.SetActive(false);
                _window.background.blocksRaycasts = false;
            }

            if (_window.loadingScreen != null)
            {
                var image = _window.loadingScreen.GetComponent<Image>();
                if (image != null)
                {
                    _material = image.materialForRendering;
                    image.material = _material;

                    _material.SetFloat(AlphaID, 0f);
                }
            }
        }

        public bool IsShowing => _isShowing;

        public void ShowThen(Action onAppearComplete, float appearDuration = 0.6f)
        {
            if (!IsLoadingScreenEnabled || _window == null)
            {
                onAppearComplete?.Invoke();
                return;
            }

            _animationDisposable?.Dispose();
            _animationDisposable = null;
            _isHiding = false;
            _isShowing = true;

            var root = _window.background != null ? _window.background.gameObject : _window.gameObject;
            root.SetActive(true);

            if (_window.background != null)
                _window.background.blocksRaycasts = true;

            if (_material == null)
            {
                onAppearComplete?.Invoke();
                return;
            }

            _material.SetFloat(AlphaID, 0f);

            float elapsed = 0f;

            _animationDisposable = ReactiveExtensions.StartUpdate(() =>
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / appearDuration);
                float alpha = t;

                _material.SetFloat(AlphaID, alpha);
                if (_window.background != null)
                    _window.background.alpha = alpha;

                if (t >= 1f)
                {
                    _animationDisposable?.Dispose();
                    _animationDisposable = null;
                    onAppearComplete?.Invoke();
                }
            });
        }
        public void StartShow()
        {
            _animationDisposable?.Dispose();
            _animationDisposable = null;
            _isHiding = false;
            _isShowing = true;
            Progress = 0f;

            var root = _window.background != null ? _window.background.gameObject : _window.gameObject;
            root.SetActive(true);

            if (_window.background != null)
                _window.background.blocksRaycasts = true;

            _material.SetFloat(AlphaID, 1f);
            _window.background.alpha = 1;
        }


        public void Hide(float disappearDuration = 0.6f, float hideMidAlpha = 0.95f, float hideFirstPhaseRatio = 0.4f)
        {
            if (!IsLoadingScreenEnabled || _window == null || !_isShowing)
                return;

            if (_isHiding)
                return;

            _animationDisposable?.Dispose();
            _animationDisposable = null;

            _isHiding = true;
            Progress = 1f;

            if (_window.background != null)
                _window.background.blocksRaycasts = false;

            if (_material == null)
            {
                SetHidden();
                return;
            }

            float firstPhaseRatio = Mathf.Clamp01(hideFirstPhaseRatio);
            float firstPhaseDuration = disappearDuration * firstPhaseRatio;
            float secondPhaseDuration = disappearDuration - firstPhaseDuration;

            float elapsed = 0f;
            _material.SetFloat(AlphaID, 1f);
            if (_window.background != null)
                _window.background.alpha = 1f;

            _animationDisposable = ReactiveExtensions.StartUpdate(() =>
            {
                elapsed += Time.unscaledDeltaTime;

                float alpha;
                if (elapsed <= firstPhaseDuration)
                {
                    float t = firstPhaseDuration > 0f ? Mathf.Clamp01(elapsed / firstPhaseDuration) : 1f;
                    alpha = Mathf.Lerp(1f, hideMidAlpha, t);
                }
                else
                {
                    if (secondPhaseDuration <= 0f)
                    {
                        alpha = hideMidAlpha;
                    }
                    else
                    {
                        float t = Mathf.Clamp01((elapsed - firstPhaseDuration) / secondPhaseDuration);
                        alpha = Mathf.Lerp(hideMidAlpha, 0f, t);
                    }
                }

                _material.SetFloat(AlphaID, alpha);
                if (_window.background != null)
                    _window.background.alpha = alpha;

                if (elapsed >= disappearDuration)
                {
                    _animationDisposable?.Dispose();
                    _animationDisposable = null;
                    _isHiding = false;
                    SetHidden();
                }
            });
        }

        private void SetHidden()
        {
            if (_window == null) return;

            var root = _window.background != null ? _window.background.gameObject : _window.gameObject;
            root.SetActive(false);

            if (_window.background != null)
            {
                _window.background.alpha = 1f;
                _window.background.blocksRaycasts = false;
            }

            if (_material != null)
                _material.SetFloat(AlphaID, 0f);

            _isShowing = false;
        }

        public void HideInstant()
        {
            if (!IsLoadingScreenEnabled)
                return;

            _animationDisposable?.Dispose();
            _animationDisposable = null;
            _isHiding = false;
            Progress = 1f;

            SetHidden();
        }

        public void SetProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);
        }
    }
}

using DG.Tweening;
using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ControlElements
{
	public class BasicButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
	{
		private const float DOUBLE_CLICK_DELAY = 2f;

		[SerializeField] private Button button;
		[SerializeField] private bool getButtonComponentIfHas = true;
		[SerializeField] private Transform ItemDropLocalPos;
		[SerializeField] private List<Image> imageForChangeColors;
		[SerializeField] private bool useDefaultColor;
		[SerializeField] private Color defaultColor;
		[SerializeField] private Color selectedColor;
		[SerializeField] private float scaleX = .95f;
		[SerializeField] private float scaleY = .95f;
		
		private static BasicButton _currentlySelectedButton = null;
		private RectTransform rectTransform;
		private Vector3 baseScale;
		private Action resetAction;
		private Tween appearTween;
        private event Action OnDownCallback;
		private event Action OnDoubleClickButton;
		private event Action OnDoubleDownCallback;
		private event Action OnUpCallback;
		private Tween _doubleClickTween = null;
		private IDisposable _doubleClickSubscription;
		private bool _touchable = true;
		private Button.ButtonClickedEvent _onClick = new();
		private bool isVisible = true;
		private Action _soundClickCallback;
		
		protected Sequence pressTween;
		protected Sequence selectTween;
		
        public RectTransform RectTransform
        {
	        get
	        {
		        if (!rectTransform)
			        rectTransform = GetComponent<RectTransform>();
		        return rectTransform;
	        }
        }
        public bool Touchable
        {
            get => _touchable;
            set
            {
                _touchable = value;

                if (button)
                    button.interactable = _touchable;
            }
        }
        public bool IsVisible
        {
	        get => isVisible;
	        set
	        {
		        isVisible = value;
		        SetVisible(value);
	        }
        }
        public bool Enabled
        {
	        get => !Locked;
	        set
	        {
		        SetLock(!value);
	        }
        }
        public bool NeedAlphaOnDisabled = false;
        public bool NeedAnimateOnClick = true;
        public bool NeedAnimateOnUnPress = true;
        public Button Button => button;
        public Button.ButtonClickedEvent onClick => button ? button.onClick : _onClick;
        public bool Locked { get; private set; } = false;
        public CanvasGroup CanvasGroup { private set; get; }
        
        protected virtual void Awake()
        {
	        if (!button && getButtonComponentIfHas) button = GetComponent<Button>();
	        rectTransform = GetComponent<RectTransform>();
	        baseScale = rectTransform.localScale;
	        CanvasGroup = GetComponent<CanvasGroup>();
	        onClick.AddListener(HighlightButton);
			
	        OnAwake();
        }
        protected virtual void OnAwake() { }
        protected virtual void OnDisable()
        {
            
        }
        protected virtual void OnDestroy()
        {
	        selectTween?.Kill();
	        selectTween = null;
        }
        
		#region  Animations
		
		public virtual void StartBlinkAnimation(float duration = .4f)
		{
			if (!gameObject || !rectTransform) return;
			selectTween?.Kill();
            
			selectTween = DOTween.Sequence();
			selectTween.Append(rectTransform.DOScale(new Vector3(baseScale.x * 0.95f, baseScale.y * 1.01f, baseScale.z), duration / 4));
			selectTween.Append(rectTransform.DOScale(baseScale, duration / 4));
			selectTween.Append(rectTransform.DOScale(new Vector3(baseScale.x * 1.05f, baseScale.y * 0.9f, baseScale.z), duration / 4));
			selectTween.Append(rectTransform.DOScale(baseScale, duration / 4));
			selectTween.SetLoops(-1);
			selectTween.OnComplete(() => selectTween = null);
			selectTween.SetLink(gameObject);
			selectTween.SetAutoKill(false);
			selectTween.Play();
		}

		public virtual void StartGrowAnimation()
		{
			if (!gameObject || !rectTransform) return;
			selectTween?.Kill();
            
			selectTween = DOTween.Sequence();
			selectTween.Append(rectTransform.DOScale(new Vector3(baseScale.x * 1.05f, baseScale.y * 1.05f, baseScale.z), 0.5f));
			selectTween.Append(rectTransform.DOScale(baseScale, 0.5f));
			selectTween.SetLoops(-1);
			selectTween.OnComplete(() => selectTween = null);
			selectTween.SetLink(gameObject);
			selectTween.Play();
		}
		
		private void AppearDisappear(bool appear, bool needEffect = false, float duration = .25f)
        {
			if (!gameObject)
				return;

            resetAction?.Invoke();
            appearTween?.Kill();

            Touchable = appear;

            gameObject.SetActive(appear);

            if (CanvasGroup)
            {
                var alpha = appear ? 1f : 0f;
                appearTween = CanvasGroup.DOFade(alpha, duration).SetEase(Ease.InSine).SetLink(gameObject);

                if (!appear)
                    appearTween.OnComplete(() => gameObject.SetActive(false));

                resetAction = () =>
                {
                    appearTween?.Kill();
                    if (!appear) gameObject.SetActive(false);
                    if (CanvasGroup) CanvasGroup.alpha = alpha;
                };
            }
        }
        
		public virtual void OnSelectAnimation()
		{
			if (!this || !gameObject || !rectTransform)
				return;

			selectTween?.Kill();
			selectTween = DOTween.Sequence()
				.SetLink(gameObject)
				.Append(rectTransform.DOScale(baseScale * 1.025f, 0.1f))
				.Append(rectTransform.DOScale(baseScale, 0.1f))        
				.OnComplete(() => selectTween = null);
			selectTween.Play();
		}
        public virtual void OnPressAnimationStart()
        {
            if (!gameObject) return;

            if (pressTween != null)
            {
                pressTween.Rewind();
                pressTween.Play();
                return;
            }

            pressTween = DOTween.Sequence().Append(rectTransform.DOScale(new Vector3(baseScale.x * scaleX, baseScale.y * scaleY, baseScale.z), 0.05f));
            pressTween.SetLink(gameObject);
            pressTween.SetAutoKill(false);
            pressTween.Play();
        }
        
        public virtual void OnPressAnimationFinished()
        {
	        if (!gameObject) return;

	        if (pressTween != null)
	        {
		        pressTween.Rewind();
		        return;
	        }
        }
		
		public void StopAnim()
		{
			selectTween?.Kill();
			selectTween = null;
		}
		#endregion

		public void ResetDoubleClick()
		{
			_doubleClickTween?.Kill();
			_doubleClickTween = null;
		}
        
		public void SetLock(bool val, bool touchable = false)
		{
			if (Locked == val) 
				return;
			Locked = val;

			Touchable = val ? touchable : true;
		}
        
		public void SetOnDownCallback(Action callback)
		{
			OnDownCallback = callback;
		}
		
		public void SetOnUpCallback(Action callback)
		{
			OnUpCallback = callback;
		}
		
        public void SetVisible(bool visible)
        {
            isVisible = visible;
            AppearDisappear(visible);
        }
        
		public void SetBaseScale()
		{
			if (!gameObject || !rectTransform)
				return;

			transform.localScale = baseScale;
		}
		
		public void SetSound(Action soundClickCallback)
		{
			_soundClickCallback = soundClickCallback;
		}

        public void OnPointerClick(PointerEventData eventData)
        {
	        
        }

        private void PlaySoundClick()
        {
	        if (!this || !gameObject)
		        return;

	        if (_soundClickCallback != null)
	        {
		        _soundClickCallback.Invoke();
		        return;
	        }
        }
        
        public void ChangeColor()
        {
	        if(imageForChangeColors.Count == 0) return;

			foreach (var t in imageForChangeColors)
	        {
		        t.color = selectedColor;
	        }
        }
        
        public void HighlightButton()
        {
	        if(imageForChangeColors.Count == 0) return;
	        if (_currentlySelectedButton != null && _currentlySelectedButton != this)
		        _currentlySelectedButton.ResetButtonColor();

	        _currentlySelectedButton = this;
	        foreach (var t in imageForChangeColors)
	        {
		        t.color = selectedColor;
	        }
        }

        public void ResetButtonColor()
        {
	        foreach (var t in imageForChangeColors)
	        {
				if (useDefaultColor) t.color = defaultColor;
				else t.color = Color.white;
	        }
        }
        
        public static void ResetAllHighlights()
        {
	        _currentlySelectedButton?.ResetButtonColor();
	        _currentlySelectedButton = null;
        }

		private bool _wasPointerDown = false;
		private bool _wasPointerExit = false;

        public void OnPointerDown(PointerEventData eventData)
		{
			if (!Touchable)
				return;

			_wasPointerDown = true;
			_wasPointerExit = false;

			OnDownCallback?.Invoke();

			if (_doubleClickTween != null)
				OnDoubleDownCallback?.Invoke();

            if (NeedAnimateOnClick)
                OnPressAnimationStart();
        }
        
		public void SetOnClick(Action onClick)
		{
			this.onClick.RemoveAllListeners();
			this.onClick.AddListener(() => onClick?.Invoke());
		}

		public void OnPointerUp(PointerEventData eventData)
        {
			_wasPointerDown = false;

			if (_wasPointerExit)
			{
				_wasPointerExit = false;
				if (NeedAnimateOnClick)
					OnPressAnimationFinished();
				return;
			}
			
			var isDrag = eventData.dragging;
			
            var parentCanvas = GetComponentInParent<CanvasGroup>();
            if (parentCanvas != null && !parentCanvas.interactable)
				return;

			if (!Touchable)
			{
				if (NeedAnimateOnClick)
					OnPressAnimationFinished();
				return;
			}

            if (isDrag)
            {
                if (NeedAnimateOnClick) 
                    OnPressAnimationFinished();
                return;
            }

			if (NeedAnimateOnClick)
			{
				if (NeedAnimateOnUnPress)
					OnSelectAnimation();
				else
					OnPressAnimationFinished();
			}

            if (!button)
                _onClick?.Invoke();

            PlaySoundClick();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!_wasPointerDown)
				return;

			_wasPointerExit = false;
			if (NeedAnimateOnClick)
				OnPressAnimationStart();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!_wasPointerDown)
				return;

			_wasPointerExit = true;
		}
	}
}



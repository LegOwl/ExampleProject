using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Core
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform root;

        private Tween _showTween;
        private Tween _hideTween;
        public RectTransform RectTransformMessage => messageText.rectTransform;
        public void SetMessage(string message, Color color, float maxFontSize = 0)
        {
            messageText.text = message;
            messageText.color = color;

            if (maxFontSize > 0)
            {
                messageText.enableAutoSizing = true;
                messageText.fontSizeMax = maxFontSize;
            }
            else
            {
                messageText.enableAutoSizing = true; 
                messageText.fontSizeMax = 72;
            }
        }


        public void PlayShow(float duration)
        {
            _showTween?.Kill();
            _hideTween?.Kill();
            
            root.localScale = Vector3.zero;
            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
            _showTween = root
                .DOScale(1f, duration)
                .SetEase(Ease.OutBack)
                .SetLink(root.gameObject);
        }

        public void PlayHide(float duration)
        {
            _hideTween?.Kill();
            
            _hideTween = canvasGroup
                .DOFade(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => 
                {
                    gameObject.SetActive(false);
                });
        }
    }
}
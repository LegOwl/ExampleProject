using Core;
using DG.Tweening;
using UnityEngine;

namespace Services
{
    public class MessageService : IService
    {
        private MessageView _messageView;
        private Tween _lifeTween;

        public void Init(MessageView view)
        {
            _messageView = view;
        }

        public void ShowMessage(MessageData data)
        {
            if (_messageView == null)
            {
                Debug.LogWarning("MessageView null!");
                return;
            }
            
            if(data.Force)
                _lifeTween?.Kill();
            else
                _lifeTween?.Kill(true);
            
            ApplyPosition(data.Position);
            _messageView.SetMessage(data.Text, data.Color, data.MaxFontSize);
            _messageView.gameObject.SetActive(true);

            float appearTime = data.Duration * 0.25f;
            float disappearTime = data.Duration * 0.75f;
            _messageView.PlayShow(appearTime);
            _lifeTween = DOVirtual.DelayedCall(appearTime, () =>
            {
                _messageView.PlayHide(disappearTime);
            });
        }
        
        private void ApplyPosition(Vector2? pos)
        {
            RectTransform rt = _messageView.RectTransformMessage;

            if (rt == null)
                return;

            if (pos == null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                rt.anchoredPosition = new Vector2(0, -350f);
            }
            else
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                
                rt.anchoredPosition = pos.Value;
            }
        }
    }

    public class MessageData
    {
        public string ID;
        public readonly string Text;
        public readonly float Duration;
        public readonly bool Force;
        public readonly Vector2? Position;
        public readonly Color Color;
        public readonly float MaxFontSize;
        public MessageData(string id, string text, float duration = 0, bool force = false, Vector2? position = null, Color? color = null, float maxFontSize = 0)
        {
            ID = id;
            Text = text;
            Duration = duration;
            Force = force;
            Position = position;
            Color = color ?? Color.white;
            MaxFontSize = maxFontSize;
        }
    }
}
using UnityEngine;
using DG.Tweening;
using Lean.Pool;
using TMPro;

public class FloatingPopup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _moveUpDistance = 1.5f;
    [SerializeField] private float _duration = 3.0f;
    [SerializeField] private float _fadeDuration = 1.0f;
    [SerializeField] private float _appearDuration = 0.25f;

    [Header("References")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private SpriteRenderer _iconRendererBackground;
    [SerializeField] private TextMeshPro _textMesh;

    public Transform IconTransform => _iconRenderer.transform;
    public TextMeshPro Text => _textMesh;
    private Tweener _moveTween;
    private Tweener _scaleTween;
    private Vector3 _originalScale;
    private float _originalTextSize;
    private static readonly char[] _plusTextBuffer = new char[64];
    
    private void Awake()
    {
        _originalScale = transform.localScale;
        _originalTextSize = _textMesh.fontSize;
    }
    
    private void OnEnable()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        DOTween.Kill(this);
        PlayAnimation();
    }

    public void Setup(string text, Sprite icon, float duration, float textSize)
    {
        if (_textMesh != null)
        {
            _textMesh.fontSize = textSize;
            _plusTextBuffer[0] = '+';
            if (!string.IsNullOrEmpty(text))
            {
                int copyLen = Mathf.Min(text.Length, _plusTextBuffer.Length - 1);
                text.CopyTo(0, _plusTextBuffer, 1, copyLen);
                _textMesh.SetCharArray(_plusTextBuffer, 0, copyLen + 1);
            }
            else
            {
                _textMesh.SetCharArray(_plusTextBuffer, 0, 1);
            }
        }

        if (_iconRenderer != null && _iconRendererBackground != null)
        {
            _iconRenderer.sprite = icon;
            _iconRendererBackground.sprite = icon;
        }

        _duration = duration;
        _fadeDuration = duration / 3f;
    }

    private void PlayAnimation()
    {
        _iconRenderer.color = new Color(1, 1, 1, 1);
        _iconRendererBackground.color = new Color(1, 1, 1, 1);
        _textMesh.color = new Color(1, 1, 1, 1);
        
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * _moveUpDistance;
            
        transform.localScale = Vector3.zero;
        _scaleTween = transform
            .DOScale(_originalScale, _appearDuration)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject);
        
        _moveTween = transform
            .DOLocalMove(endPos, _duration)
            .SetEase(Ease.OutQuad);
            
        FadeOut(_iconRenderer);
        FadeOut(_iconRendererBackground);
        FadeOut(_textMesh);

    }

    private void FadeOut(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null) return;

        var color = spriteRenderer.color;
        DOTween.ToAlpha(
            () => color,
            x => { spriteRenderer.color = x; },
            0f,
            _duration - _fadeDuration
        ).SetDelay(_fadeDuration);
    }

    private void FadeOut(TextMeshPro mesh)
    {
        if (mesh == null) return;

        var color = mesh.color;
        DOTween.ToAlpha(
            () => color,
            x => { mesh.color = x; },
            0f,
            _duration - _fadeDuration
        ).SetDelay(_fadeDuration);
    }
    
    private void OnDisable()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        DOTween.Kill(this);
    }
}

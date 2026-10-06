using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Juice Configuration")]
    public float hoverScale = 1.05f;
    public float pressScale = 0.94f;
    public float transitionDuration = 0.15f;
    public bool playClickSound = true;

    private Vector3 originalScale;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateTo(originalScale * hoverScale, transitionDuration, Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateTo(originalScale, transitionDuration, Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(originalScale * pressScale, 0.08f, Ease.InQuad);
        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(originalScale * hoverScale, 0.2f, Ease.OutBack);
    }

    private void AnimateTo(Vector3 target, float duration, Ease ease)
    {
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();
        currentTween = transform.DOScale(target, duration).SetEase(ease);
    }
}

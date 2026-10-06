using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class JuiceEffects : MonoBehaviour
{
    public static JuiceEffects Instance;

    [Header("FX Containers")]
    public RectTransform fxCanvasTransform;
    public Sprite starSprite;

    private readonly Color[] confettiColors = new Color[]
    {
        new Color(1f, 0.84f, 0f),      // Gold
        new Color(0.2f, 0.8f, 0.4f),    // Emerald
        new Color(0.2f, 0.6f, 1f),      // Sky Blue
        new Color(1f, 0.4f, 0.6f),      // Pink
        new Color(1f, 0.5f, 0.1f),      // Orange
        new Color(0.6f, 0.3f, 0.9f)     // Purple
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (fxCanvasTransform == null)
        {
            fxCanvasTransform = GetComponent<RectTransform>();
        }
    }

    public void TriggerConfettiBurst(Vector2 centerScreenPos, int count = 35)
    {
        if (fxCanvasTransform == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject confetti = new GameObject("ConfettiPiece", typeof(RectTransform), typeof(Image));
            RectTransform rt = confetti.GetComponent<RectTransform>();
            rt.SetParent(fxCanvasTransform, false);
            rt.position = centerScreenPos;

            // Random size & color
            float size = Random.Range(14f, 28f);
            rt.sizeDelta = new Vector2(size, size * Random.Range(0.6f, 1.4f));

            Image img = confetti.GetComponent<Image>();
            img.color = confettiColors[Random.Range(0, confettiColors.Length)];

            // Explosion vector
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float force = Random.Range(180f, 520f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * force;
            Vector2 targetPos = (Vector2)rt.position + velocity + new Vector2(0, -Random.Range(50f, 200f));

            float duration = Random.Range(1.0f, 1.6f);

            // Animate position, rotation, and fade
            rt.DOMove(targetPos, duration).SetEase(Ease.OutQuad);
            rt.DORotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 720)), duration, RotateMode.FastBeyond360);
            img.DOFade(0f, duration * 0.4f).SetDelay(duration * 0.6f).OnComplete(() =>
            {
                Destroy(confetti);
            });
        }
    }

    public void TriggerStarBurst(Vector2 centerScreenPos, int count = 12)
    {
        if (fxCanvasTransform == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject star = new GameObject("StarSparkle", typeof(RectTransform), typeof(Image));
            RectTransform rt = star.GetComponent<RectTransform>();
            rt.SetParent(fxCanvasTransform, false);
            rt.position = centerScreenPos;

            float size = Random.Range(32f, 56f);
            rt.sizeDelta = new Vector2(size, size);

            Image img = star.GetComponent<Image>();
            if (starSprite != null) img.sprite = starSprite;
            img.color = Color.white;

            float angle = (i * (360f / count) + Random.Range(-15f, 15f)) * Mathf.Deg2Rad;
            float dist = Random.Range(120f, 240f);
            Vector2 targetPos = (Vector2)rt.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            float duration = Random.Range(0.7f, 1.1f);

            rt.DOMove(targetPos, duration).SetEase(Ease.OutBack);
            rt.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), duration);
            rt.DOScale(Vector3.zero, duration * 0.4f).SetDelay(duration * 0.6f).OnComplete(() =>
            {
                Destroy(star);
            });
        }
    }

    public void ShakeTransform(RectTransform target, float duration = 0.35f, float strength = 12f)
    {
        if (target == null) return;
        target.DOShakeAnchorPos(duration, strength, 18, 90, false, true);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ClockUI : MonoBehaviour
{
    [Header("Analog Clock References")]
    public RectTransform analogClockContainer;
    public Image dialImage;
    public RectTransform hourHandRect;
    public RectTransform minuteHandRect;
    public Image centerPinImage;

    [Header("Digital Clock References")]
    public RectTransform digitalClockContainer;
    public TextMeshProUGUI digitalTimeText;
    public Image digitalFrameImage;

    [Header("Context Badge / Media")]
    public RectTransform contextMediaContainer;
    public Image contextImage;
    public TextMeshProUGUI contextSubtext;

    [Header("Hand Alignment Settings")]
    [Tooltip("If true, the hour needle always points directly at the exact hour number on the clock, avoiding in-between positions for Grade 2 clarity.")]
    public bool snapHourHandToExactNumber = true;

    private Tween hourTween;
    private Tween minuteTween;

    public void SetTime(int hour, int minute, bool animate = true)
    {
        // Standard 12-hour angles: 
        // 12 o'clock = 0 deg, 3 o'clock = -90 deg, 6 o'clock = -180 deg, etc.
        float targetMinuteAngle = -(minute * 6f);

        // Calculate target hour angle
        float targetHourAngle;
        if (snapHourHandToExactNumber)
        {
            // At 6:30, snapping the hour hand to 6 causes both hands to overlap directly at 6 (-180 deg).
            // At 6:30, offset the hour hand halfway towards 7 (-195 deg) so both the hour and minute hands are clearly visible!
            if ((hour % 12) == 6 && minute == 30)
            {
                targetHourAngle = -195f;
            }
            else
            {
                targetHourAngle = -((hour % 12) * 30f);
            }
        }
        else
        {
            targetHourAngle = -((hour % 12) * 30f + (minute / 60f) * 30f);
        }

        if (hourHandRect != null)
        {
            if (hourTween != null && hourTween.IsActive()) hourTween.Kill();

            if (animate)
            {
                // Give a playful juice overshoot
                hourHandRect.localRotation = Quaternion.Euler(0, 0, hourHandRect.localEulerAngles.z);
                hourTween = hourHandRect.DOLocalRotate(new Vector3(0, 0, targetHourAngle), 0.75f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                hourHandRect.localRotation = Quaternion.Euler(0, 0, targetHourAngle);
            }
        }

        if (minuteHandRect != null)
        {
            if (minuteTween != null && minuteTween.IsActive()) minuteTween.Kill();

            if (animate)
            {
                minuteTween = minuteHandRect.DOLocalRotate(new Vector3(0, 0, targetMinuteAngle), 0.85f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                minuteHandRect.localRotation = Quaternion.Euler(0, 0, targetMinuteAngle);
            }
        }
    }

    public void ShowAnalogMode(int hour, int minute, bool showDigitalReadout = false)
    {
        if (analogClockContainer != null) analogClockContainer.gameObject.SetActive(true);
        // Never show digital readout under the analog clock so it never gives away the answer
        if (digitalClockContainer != null) digitalClockContainer.gameObject.SetActive(false);
        if (contextMediaContainer != null) contextMediaContainer.gameObject.SetActive(false);

        SetTime(hour, minute, true);
    }

    public void ShowDigitalMode(string timeText, int correspondingHour = 0, int correspondingMinute = 0)
    {
        // Hide analog clock so ONLY the digital clock is visible for digital clock questions
        if (analogClockContainer != null) analogClockContainer.gameObject.SetActive(false);
        if (contextMediaContainer != null) contextMediaContainer.gameObject.SetActive(false);

        if (digitalClockContainer != null)
        {
            digitalClockContainer.gameObject.SetActive(true);
            digitalClockContainer.anchorMin = digitalClockContainer.anchorMax = new Vector2(0.5f, 0.5f);
            digitalClockContainer.anchoredPosition = Vector2.zero;
            digitalClockContainer.sizeDelta = new Vector2(340, 130);
        }

        if (digitalTimeText != null)
        {
            digitalTimeText.text = timeText;
            digitalTimeText.fontSize = 58;
            digitalTimeText.transform.DOPunchScale(Vector3.one * 0.12f, 0.35f, 6, 0.8f);
        }
    }

    public void ShowContextMedia(Sprite mediaSprite, string label, int optionalHour = 0, int optionalMin = 0)
    {
        if (contextMediaContainer != null)
        {
            contextMediaContainer.gameObject.SetActive(true);
            if (contextImage != null) contextImage.sprite = mediaSprite;
            if (contextSubtext != null) contextSubtext.text = label;
            contextMediaContainer.DOPunchScale(Vector3.one * 0.08f, 0.4f, 6, 0.8f);
        }

        if (optionalHour > 0)
        {
            if (analogClockContainer != null) analogClockContainer.gameObject.SetActive(true);
            SetTime(optionalHour, optionalMin, true);
        }
        else
        {
            if (analogClockContainer != null) analogClockContainer.gameObject.SetActive(false);
        }

        if (digitalClockContainer != null) digitalClockContainer.gameObject.SetActive(false);
    }

    public void PunchBounce()
    {
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.1f, 0.35f, 7, 0.7f);
    }
}

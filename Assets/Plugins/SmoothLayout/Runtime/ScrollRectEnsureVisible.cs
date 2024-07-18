#region USING

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#endregion

[RequireComponent(typeof (ScrollRect))]
public class ScrollRectEnsureVisible : MonoBehaviour
{
    public Ease AnimCurve;
    public bool Log = true;
    public RectTransform MaskTransform;

    private RectTransform _content;
    private ScrollRect _sr;
    public bool IsScrolling => _sr.velocity != Vector2.zero;

    private void Awake()
    {
        _sr = GetComponent<ScrollRect>();
        _content = _sr.content;
    }
    
    public ScrollRect scrollRect => _sr;
    [Button]
    // Method to focus on the target RectTransform
    public void FocusOnRectWithSpeed(RectTransform target, float speed)
    {
        StartCoroutine(scrollRect.FocusOnItemCoroutine(target, speed));
    }
    
    [Button]
    public void FocusOnRectTween(RectTransform target, float duration, Vector2 offset = default(Vector2))
    {
        Vector2 targetPos = scrollRect.CalculateFocusedScrollPosition(target,offset);
        if (duration == 0)
        {
            _sr.normalizedPosition = targetPos;
        }
        else
        {
            DOTween.To(() => _sr.normalizedPosition, x => _sr.normalizedPosition = x, targetPos, duration)
                .SetEase(AnimCurve);
        }
        
    }

    /// <summary>
    /// Takes a float value from a [0f,1f] range and translates it to a [-1f,1f] range
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    float Transtale01RangeToMinus11Range(float value)
    {
        return (value + ((1f - value) * -1f));
    }
}

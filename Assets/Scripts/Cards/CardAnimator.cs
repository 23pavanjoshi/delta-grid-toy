using System;
using System.Collections;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private GameObject _cardFace;

    private readonly float _flipDuration = 0.15f;

    public IEnumerator FlipToFace(Action onComplete = null)
    {
        yield return ScaleX(1f, 0f);
        _cardBack.SetActive(false);
        _cardFace.SetActive(true);
        yield return ScaleX(0f, 1f);
        onComplete?.Invoke();
    }

    public IEnumerator FlipToBack(Action onComplete = null)
    {
        yield return ScaleX(1f, 0f);
        _cardFace.SetActive(false);
        _cardBack.SetActive(true);
        yield return ScaleX(0f, 1f);
        onComplete?.Invoke();
    }

    private IEnumerator ScaleX(float from, float to)
    {
        float elapsed = 0f;
        Vector3 scale = transform.localScale;

        while (elapsed < _flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _flipDuration);
            transform.localScale = new Vector3(Mathf.Lerp(from, to, t), scale.y, scale.z);
            yield return null;
        }

        transform.localScale = new Vector3(to, scale.y, scale.z);
    }
}
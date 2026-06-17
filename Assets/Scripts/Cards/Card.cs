using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image _cardBack;
    [SerializeField] private GameObject _cardFaceObj;
    [SerializeField] private Image _cardFaceIcon;
    [SerializeField] private Button _button;

    public CardData Data { get; private set; }
    public CardState State { get; private set; }
    
    private bool _isAnimating;
    
    private CardAnimator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<CardAnimator>();
    }
    
    public void Initialize(CardData data, Sprite faceSprite)
    {
        Data = data;
        // _cardFaceIcon.sprite = faceSprite;
        SetState(CardState.FaceDown);
    }
    
    public void OnClicked()
    {
        if (State != CardState.FaceDown || _isAnimating) return;
        BoardManager.Instance.RequestFlip(this);
    }
    
    public IEnumerator DoFlip(System.Action onComplete = null)
    {
        _isAnimating = true;
        SetState(CardState.Flipping);

        yield return _animator.FlipToFace(() => {
            SetState(CardState.FaceUp);
            _isAnimating = false;
            Debug.Log($"Flip done — pairId: {Data.pairId}");
            onComplete?.Invoke();
        });
    }
    
    public IEnumerator DoReverseFlip(System.Action onComplete = null)
    {
        _isAnimating = true;
        SetState(CardState.Flipping);

        yield return _animator.FlipToBack(() => {
            SetState(CardState.FaceDown);
            _isAnimating = false;
            Debug.Log($"ReverseFlip done — pairId: {Data.pairId}");
            onComplete?.Invoke();
        });
    }
    
    private void SetState(CardState newState)
    {
        State = newState;
        _button.interactable = (newState == CardState.FaceDown);
    }
    
    public void SetMatched()
    {
        SetState(CardState.Matched);
        _button.interactable = false;
    }
}

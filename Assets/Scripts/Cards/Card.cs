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
    
    public void Initialize(CardData data, Sprite faceSprite)
    {
        Data = data;
        _cardFaceIcon.sprite = faceSprite;
        SetState(CardState.FaceDown);
    }
    
    public void OnClicked()
    {
        if (State != CardState.FaceDown || _isAnimating) return;
    
        Debug.Log($"Card tapped — pairId: {Data.pairId}");
    }
    
    private void SetState(CardState newState)
    {
        State = newState;
        _button.interactable = (newState == CardState.FaceDown);
    }
}

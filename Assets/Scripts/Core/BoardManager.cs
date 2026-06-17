using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RectTransform _boardContainer;
    [SerializeField] private DynamicGridLayout _dynamicGrid;
    
    [SerializeField] private GridConfig _currentLayout = new GridConfig(2, 2);
    [SerializeField] private int _seed = 42;
    [SerializeField] private float _cardSpacing = 10f;

    private List<Card> _spawnedCards = new();

    [SerializeField] private List<Card> _pendingPair = new();
    private bool _isEvaluating = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnAfterLayout());
    }
    
    private IEnumerator SpawnAfterLayout()
    {
        // Wait 1 frame for Canvas to calculate RectTransform sizes
        yield return null;
        
        // Setup grid FIRST before spawning anything
        _dynamicGrid.SetupGrid(_currentLayout.columns, _currentLayout.rows, _cardSpacing);

        // Generate shuffled card data before card spawn
        var cardDataList = ShuffleController.GenerateShuffleCards(_currentLayout, _seed);

        // Spawn cards and set into GridLayoutGroup
        foreach (var cardData in cardDataList)
        {
            var cardGO = Instantiate(_cardPrefab, _boardContainer);
            var controller = cardGO.GetComponent<Card>();
            controller.Initialize(cardData, null);
            _spawnedCards.Add(controller);
        }
    }
    
    public void ResetBoard(GridConfig newLayout)
    {
        foreach (var card in _spawnedCards)
        {
            Destroy(card.gameObject);
        }

        _spawnedCards.Clear();
        _pendingPair.Clear();

        _currentLayout = newLayout;

        _seed = Random.Range(0, 99999);

        StartCoroutine(SpawnAfterLayout());
    }
    
    public void RequestFlip(Card card)
    {
        Debug.Log($"RequestFlip — cardId: {card.Data.cardId}, pairId: {card.Data.pairId}");
        
        if (_pendingPair.Contains(card)) return;

        _pendingPair.Add(card);

        if (card.Data.pairId == -1)
        {
            if (_pendingPair.Count == 2)
            {
                StartCoroutine(FlipAndEvaluate(card));
            }
            else
            {
                StartCoroutine(HandleWildcard(card));
            }
            return;
        }

        StartCoroutine(FlipAndEvaluate(card));
    }
    
    private IEnumerator HandleWildcard(Card card)
    {
        bool flipDone = false;
        yield return card.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);

        yield return new WaitForSeconds(0.4f);
        card.SetMatched();
        _pendingPair.Clear();
        CheckWinCondition();
    }
    
    private IEnumerator FlipAndEvaluate(Card card)
    {
        bool flipDone = false;
        yield return card.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);

        // Only evaluate when 2 cards in pair list
        if (_pendingPair.Count < 2) yield break;

        var cardA = _pendingPair[0];
        var cardB = _pendingPair[1];
        _pendingPair.Clear();

        yield return EvaluatePair(cardA, cardB);
    }
    
    private IEnumerator EvaluatePair(Card cardA, Card cardB)
    {
        _isEvaluating = true;

        bool isMatch = cardA.Data.pairId == cardB.Data.pairId;

        // Wildcard — pairId -1 always matches itself (When extra card add that time it become wildcard)
        if (cardA.Data.pairId == -1 && cardB.Data.pairId == -1)
            isMatch = true;

        if (isMatch)
        {
            cardA.SetMatched();
            cardB.SetMatched();
            Debug.Log("MATCH !!!!");
        }
        else
        {
            // Small pause so player sees both faces
            yield return new WaitForSeconds(0.6f);

            bool aDone = false, bDone = false;
            StartCoroutine(cardA.DoReverseFlip(() => aDone = true));
            StartCoroutine(cardB.DoReverseFlip(() => bDone = true));

            yield return new WaitUntil(() => aDone && bDone);
            Debug.Log("MISMATCH !!!!");
        }

        _isEvaluating = false;
        CheckWinCondition();
    }
    
    private void CheckWinCondition()
    {
        foreach (var card in _spawnedCards)
        {
            if (card.State != CardState.Matched) return;
        }

        Debug.Log("YOU WIN!");
    }
    
}
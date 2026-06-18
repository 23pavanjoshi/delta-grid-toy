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
    [SerializeField] private List<Sprite> _cardIconSprite = new();
    [SerializeField] private Sprite _wildCardSpriteIcon;

    [SerializeField] private List<Card> _pendingPair = new();
    
    private int _moveCount = 0;
    private float _elapsedTime = 0f;
    
    public bool IsGameOver = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    
    public void StartNewGame()
    {
        _seed = Random.Range(0, 99999);
        _moveCount = 0;
        _elapsedTime = 0f;
        IsGameOver = false;
        ScoreManager.Instance.ResetScore();
        StartCoroutine(SpawnAfterLayout());
    }
    
    private void Update()
    {
        if (_spawnedCards.Count > 0 && !IsGameOver)
            _elapsedTime += Time.deltaTime;
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
            var cardSprite = cardData.pairId == -1 ? _wildCardSpriteIcon : _cardIconSprite[cardData.spriteIndex];
            controller.Initialize(cardData, cardSprite);
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
        IsGameOver = false;

        StartCoroutine(SpawnAfterLayout());
    }
    
    public void RequestFlip(Card card)
    {
        Debug.Log($"RequestFlip — cardId: {card.Data.cardId}, pairId: {card.Data.pairId}");
    
        if (_pendingPair.Contains(card)) return;

        _moveCount++;
        ScoreManager.Instance.OnTurnTaken();
        
        _pendingPair.Add(card);

        if (card.Data.pairId == -1)
        {
            if (_pendingPair.Count == 2)
            {
                var waiting = _pendingPair[0];
                _pendingPair.Clear();
                StartCoroutine(HandleWildcardWithPending(card, waiting));
            }
            else
            {
                StartCoroutine(HandleWildcard(card));
            }
            return;
        }

        if (_pendingPair.Count == 2)
        {
            var cardA = _pendingPair[0];
            var cardB = _pendingPair[1];
            _pendingPair.Clear();
            StartCoroutine(FlipThenEvaluate(cardA, cardB));
            return;
        }

        StartCoroutine(FlipAndWait(card));
    }
    
    private IEnumerator FlipAndWait(Card card)
    {
        bool flipDone = false;
        yield return card.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);
    }

    private IEnumerator FlipThenEvaluate(Card cardA, Card cardB)
    {
        bool flipDone = false;
        yield return cardB.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);

        yield return EvaluatePair(cardA, cardB);
    }
    
    private IEnumerator HandleWildcardWithPending(Card wildcard, Card waitingCard)
    {
        bool reverseDone = false;
        StartCoroutine(waitingCard.DoReverseFlip(() => reverseDone = true));
        yield return new WaitUntil(() => reverseDone);

        bool flipDone = false;
        yield return wildcard.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);

        yield return new WaitForSeconds(0.4f);
        wildcard.SetMatched();
        ScoreManager.Instance.OnMatch();
        AudioManager.Instance.PlayMatch();
        CheckWinCondition();
    }
    
    private IEnumerator HandleWildcard(Card card)
    {
        bool flipDone = false;
        yield return card.DoFlip(() => flipDone = true);
        yield return new WaitUntil(() => flipDone);

        yield return new WaitForSeconds(0.4f);
        card.SetMatched();
        ScoreManager.Instance.OnMatch();
        AudioManager.Instance.PlayMatch();
        _pendingPair.Clear();
        CheckWinCondition();
    }
    
    private IEnumerator EvaluatePair(Card cardA, Card cardB)
    {
        bool isMatch = cardA.Data.pairId == cardB.Data.pairId;

        // Wildcard — pairId -1 always matches itself (When extra card add that time it become wildcard)
        if (cardA.Data.pairId == -1 && cardB.Data.pairId == -1)
            isMatch = true;

        if (isMatch)
        {
            cardA.SetMatched();
            cardB.SetMatched();
            ScoreManager.Instance.OnMatch();
            AudioManager.Instance.PlayMatch();
            Debug.Log("MATCH !!!!");
        }
        else
        {
            ScoreManager.Instance.OnMismatch();
            AudioManager.Instance.PlayMismatch();
            // Small pause so player sees both faces
            yield return new WaitForSeconds(0.6f);

            bool aDone = false, bDone = false;
            StartCoroutine(cardA.DoReverseFlip(() => aDone = true));
            StartCoroutine(cardB.DoReverseFlip(() => bDone = true));

            yield return new WaitUntil(() => aDone && bDone);
            Debug.Log("MISMATCH !!!!");
        }

        CheckWinCondition();
    }
    
    private void CheckWinCondition()
    {
        foreach (var card in _spawnedCards)
        {
            if (card.State != CardState.Matched) return;
        }

        IsGameOver = true;
        SaveManager.DeleteSave();
        AudioManager.Instance.PlayGameOver();
        GameManager.Instance.ResultDeclared();
        Debug.Log("YOU WIN!");
    }
    
    public IEnumerator RestoreState(SaveData data)
    {
        foreach (var card in _spawnedCards)
        {
            Destroy(card.gameObject);
        }

        _spawnedCards.Clear();
        _pendingPair.Clear();
        IsGameOver = false;

        _currentLayout = new GridConfig(data.columns, data.rows);
        _seed = data.seed;
        _moveCount = data.moveCount;
        _elapsedTime = data.elapsedTime;

        yield return null;

        _dynamicGrid.SetupGrid(_currentLayout.columns, _currentLayout.rows, _cardSpacing);

        var cardDataList = ShuffleController.GenerateShuffleCards(_currentLayout, _seed);

        for (int i = 0; i < cardDataList.Count; i++)
        {
            var cardGO = Instantiate(_cardPrefab, _boardContainer);
            var controller = cardGO.GetComponent<Card>();
            var cardSprite = cardDataList[i].pairId == -1 ? _wildCardSpriteIcon : _cardIconSprite[cardDataList[i].spriteIndex];
            controller.Initialize(cardDataList[i], cardSprite);

            if (i < data.cardStates.Count && data.cardStates[i].isMatched)
            {
                controller.SetMatchedInstant();
            }

            _spawnedCards.Add(controller);
        }

        ScoreManager.Instance.RestoreScore(data.score, data.turnScore, 0);
        
        Debug.Log($"[BoardManager] Restored → {data.columns}x{data.rows} seed:{data.seed} moves:{data.moveCount}");
    }
    
    public SaveData CaptureState()
    {
        var data = new SaveData();
        data.columns = _currentLayout.columns;
        data.rows = _currentLayout.rows;
        data.seed = _seed;
        data.moveCount = _moveCount;
        data.elapsedTime = _elapsedTime;
        data.score = ScoreManager.Instance.CurrentScore;
        data.turnScore = ScoreManager.Instance.TurnScore;
        data.comboCount = ScoreManager.Instance.ComboCount;

        foreach (var card in _spawnedCards)
        {
            data.cardStates.Add(new CardSaveState
            {
                cardId = card.Data.cardId,
                pairId = card.Data.pairId,
                spriteIndex = card.Data.spriteIndex,
                isMatched = card.State == CardState.Matched
            });
        }

        return data;
    }

    public void RestartGame()
    {
        ResetBoard(_currentLayout);
    }
    
}
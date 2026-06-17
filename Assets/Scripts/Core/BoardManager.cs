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
    
    [SerializeField] private int _columns = 2;
    [SerializeField] private int _rows = 2;
    [SerializeField] private int _seed = 42;
    [SerializeField] private float _cardSpacing = 10f;

    private List<Card> _spawnedCards = new();

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
        _dynamicGrid.SetupGrid(_columns, _rows, _cardSpacing);

        // Generate shuffled card data before card spawn
        var cardDataList = ShuffleController.GenerateShuffleCards(_columns, _rows, _seed);

        // Spawn cards and set into GridLayoutGroup
        foreach (var cardData in cardDataList)
        {
            var cardGO = Instantiate(_cardPrefab, _boardContainer);
            var controller = cardGO.GetComponent<Card>();
            controller.Initialize(cardData, null);
            _spawnedCards.Add(controller);
        }
    }
}
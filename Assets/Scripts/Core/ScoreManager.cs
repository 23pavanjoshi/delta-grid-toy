using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnTurnScoreChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;
    public event Action<int> OnComboChanged;

    private int _turnScore = 0;
    private int _currentScore = 0;
    private int _highScore = 0;
    private int _comboCount = 0;
    private float _lastMatchTime;

    private const float ComboWindow = 5f;
    private const float ComboIncrement = 0.5f;
    private const float MaxMultiplier = 4f;
    private const int BaseMatchScore = 10;

    public int CurrentScore => _currentScore;
    public int TurnScore => _turnScore;
    public int HighScore => _highScore;
    public int ComboCount => _comboCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadHighScore();
        ResetScore();
    }

    public void OnTurnTaken()
    {
        _turnScore += 1;
        OnTurnScoreChanged?.Invoke(_turnScore);
    }

    public void OnMatch()
    {
        bool withinWindow = _lastMatchTime > 0f && (Time.time - _lastMatchTime) <= ComboWindow;

        if (withinWindow)
            _comboCount++;
        else
            _comboCount = 0;

        _lastMatchTime = Time.time;

        float multiplier = Mathf.Min(1f + _comboCount * ComboIncrement, MaxMultiplier);
        int earned = Mathf.RoundToInt(BaseMatchScore * multiplier);
        _currentScore += earned;

        Debug.Log($"[ScoreManager] +{earned} | combo:{_comboCount} x{multiplier:F1} | total:{_currentScore}");

        CheckHighScore();

        OnScoreChanged?.Invoke(_currentScore);
        OnComboChanged?.Invoke(_comboCount);
    }

    public void OnMismatch()
    {
        if (_comboCount == 0) return;

        _comboCount = 0;
        _lastMatchTime = 0f;

        OnComboChanged?.Invoke(_comboCount);
        Debug.Log("[ScoreManager] Mismatch — combo reset");
    }

    public void ResetScore()
    {
        _turnScore = 0;
        _currentScore = 0;
        _comboCount = 0;
        _lastMatchTime = 0f;

        OnTurnScoreChanged?.Invoke(_turnScore);
        OnScoreChanged?.Invoke(_currentScore);
        OnComboChanged?.Invoke(_comboCount);
    }

    private void CheckHighScore()
    {
        if (_currentScore <= _highScore) return;

        _highScore = _currentScore;
        SaveHighScore();
        OnHighScoreChanged?.Invoke(_highScore);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", _highScore);
        PlayerPrefs.Save();
    }

    public void LoadHighScore()
    {
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        OnHighScoreChanged?.Invoke(_highScore);
    }

    public void RestoreScore(int score, int turn, int savedHighScore)
    {
        _currentScore = score;
        _turnScore = turn;
        _highScore = Mathf.Max(savedHighScore, _highScore);

        _comboCount = 0;
        _lastMatchTime = 0f;

        OnScoreChanged?.Invoke(_currentScore);
        OnTurnScoreChanged?.Invoke(_turnScore);
        OnHighScoreChanged?.Invoke(_highScore);
        OnComboChanged?.Invoke(_comboCount);
    }
}
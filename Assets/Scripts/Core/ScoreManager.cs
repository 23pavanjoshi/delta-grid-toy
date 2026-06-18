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

    private const float ComboWindow = 5f;
    private const float ComboIncrement = 0.5f;
    private const float MaxMultiplier = 4f;
    private const int BaseMatchScore = 10;

    public int CurrentScore => _currentScore;
    public int TurnScore => _turnScore;
    public int HighScore => _highScore;
    public int ComboCount => _comboCount;
    
    private ScoreCalculator _calculator = new ScoreCalculator();

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
        int earned = _calculator.RegisterMatch(Time.time);
        _currentScore = _calculator.CurrentScore;
        
        OnScoreChanged?.Invoke(_currentScore);
        OnComboChanged?.Invoke(_calculator.ComboCount);
    }

    public void OnMismatch()
    {
        _calculator.RegisterMismatch();
        _comboCount = 0;
        OnComboChanged?.Invoke(0);
    }

    public void ResetScore()
    {
        _calculator.Reset();
        _currentScore = 0;
        _comboCount = 0;

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

        OnScoreChanged?.Invoke(_currentScore);
        OnTurnScoreChanged?.Invoke(_turnScore);
        OnHighScoreChanged?.Invoke(_highScore);
        OnComboChanged?.Invoke(_comboCount);
    }
}
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("Game Panel")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _turnText;
    [SerializeField] private TMP_Text _highScoreText;
    
    [Header("Result Panel")]
    [SerializeField] private TMP_Text _resultScoreText;
    [SerializeField] private TMP_Text _resultTurnText;
    [SerializeField] private TMP_Text _resulthighScoreText;

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
        ScoreManager.Instance.OnTurnScoreChanged += UpdateTurn;
        ScoreManager.Instance.OnHighScoreChanged += UpdateHighScore;
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        ScoreManager.Instance.OnTurnScoreChanged -= UpdateTurn;
        ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScore;
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
        _resultScoreText.text = $"Score: {score}";
    }

    private void UpdateTurn(int turn)
    {
        _turnText.text = $"Turns: {turn}";
        _resultTurnText.text = $"Turns: {turn}";
    }

    private void UpdateHighScore(int high)
    {
        _highScoreText.text = $"Best: {high}";
        _resulthighScoreText.text = $"Best: {high}";
    }
}
using UnityEngine;

public class ScoreCalculator
{
    private const float ComboWindow    = 5f;
    private const float ComboIncrement = 0.5f;
    private const float MaxMultiplier  = 4f;

    public int  CurrentScore { get; private set; }
    public int  ComboCount   { get; private set; }

    private float _lastMatchTime;

    public int RegisterMatch(float currentTime)
    {
        float elapsed = currentTime - _lastMatchTime;

        if (_lastMatchTime > 0f && elapsed <= ComboWindow)
            ComboCount++;
        else
            ComboCount = 0;

        _lastMatchTime = currentTime;

        float multiplier = Mathf.Min(1f + ComboCount * ComboIncrement, MaxMultiplier);
        int earned = Mathf.RoundToInt(100 * multiplier);
        CurrentScore += earned;
        return earned;
    }

    public void RegisterMismatch()
    {
        ComboCount = 0;
        _lastMatchTime = 0f;
    }

    public void Reset()
    {
        CurrentScore   = 0;
        ComboCount     = 0;
        _lastMatchTime = 0f;
    }
}
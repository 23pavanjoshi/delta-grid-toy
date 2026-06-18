using NUnit.Framework;

public class ScoreCalculatorTests   // must match filename exactly
{
    private ScoreCalculator _calc;

    [SetUp]
    public void SetUp()
    {
        _calc = new ScoreCalculator();
    }
    
    [Test]
    public void FirstMatch_ScoreIs100_NoCombo()
    {
        _calc.RegisterMatch(1f);

        Assert.AreEqual(100, _calc.CurrentScore);
        Assert.AreEqual(0, _calc.ComboCount);
    }

    [Test]
    public void TwoQuickMatches_ComboIncrements()
    {
        _calc.RegisterMatch(1f);
        _calc.RegisterMatch(3f); // within 5s window

        Assert.AreEqual(1, _calc.ComboCount);
        Assert.IsTrue(_calc.CurrentScore > 200); // multiplier applied
    }

    [Test]
    public void MismatchBetweenMatches_ComboResets()
    {
        _calc.RegisterMatch(1f);
        _calc.RegisterMatch(3f); // combo = 1

        _calc.RegisterMismatch();

        Assert.AreEqual(0, _calc.ComboCount);
    }

    [Test]
    public void SlowMatches_OutsideWindow_ComboDoesNotBuild()
    {
        _calc.RegisterMatch(1f);
        _calc.RegisterMatch(10f); // 9s gap — outside 5s window

        Assert.AreEqual(0, _calc.ComboCount);
        Assert.AreEqual(200, _calc.CurrentScore); // two base matches, no multiplier
    }

    [Test]
    public void Reset_ClearsScoreAndCombo()
    {
        _calc.RegisterMatch(1f);
        _calc.RegisterMatch(2f);
        _calc.Reset();

        Assert.AreEqual(0, _calc.CurrentScore);
        Assert.AreEqual(0, _calc.ComboCount);
    }

    [Test]
    public void ComboMultiplier_CapsAtMaximum()
    {
        // Build a long combo — hit match every 1s
        for (int i = 0; i < 20; i++)
            _calc.RegisterMatch(i * 1f);

        // Score per match should stop increasing at max multiplier
        int scoreBefore = _calc.CurrentScore;
        _calc.RegisterMatch(20f);
        int earned = _calc.CurrentScore - scoreBefore;

        Assert.AreEqual(400, earned); // 100 * 4x max
    }
}
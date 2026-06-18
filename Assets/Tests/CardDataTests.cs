using NUnit.Framework;

public class CardDataTests   // must match filename exactly
{
    [Test]
    public void CardsWithSamePairId_AreAMatch()
    {
        var a = new CardData(pairId: 3, cardId: 0, spriteIndex: 0);
        var b = new CardData(pairId: 3, cardId: 1, spriteIndex: 0);

        Assert.AreEqual(a.pairId, b.pairId);
    }

    [Test]
    public void CardsWithDifferentPairId_AreNotAMatch()
    {
        var a = new CardData(pairId: 3, cardId: 0, spriteIndex: 0);
        var b = new CardData(pairId: 2, cardId: 1, spriteIndex: 0);

        Assert.AreNotEqual(a.pairId, b.pairId);
    }

    [Test]
    public void WildcardCard_HasPairIdMinusOne()
    {
        var wildcard = new CardData(pairId: -1, cardId: 0, spriteIndex: -1);

        Assert.AreEqual(-1, wildcard.pairId);
    }
}
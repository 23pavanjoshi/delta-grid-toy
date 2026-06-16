[System.Serializable]
public class CardData
{
    public int cardId;
    public int pairId;
    public int spriteIndex;

    public CardData(int cardId, int pairId, int spriteIndex)
    {
        this.cardId = cardId;
        this.pairId = pairId;
        this.spriteIndex = spriteIndex;
    }
}
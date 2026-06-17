using System.Collections.Generic;

namespace Data
{
    [System.Serializable]
    public class CardSaveState
    {
        public int cardId;
        public int pairId;
        public int spriteIndex;
        public bool isMatched;
    }

    [System.Serializable]
    public class SaveData
    {
        public int columns;
        public int rows;
        public int seed;
        public int score;
        public int comboCount;
        public int moveCount;
        public float elapsedTime;
        public List<CardSaveState> cardStates = new();
    }
}
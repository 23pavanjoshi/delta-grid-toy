using System.Collections.Generic;

namespace Data
{
    public static class ShuffleController
    {
        public static List<CardData> GenerateShuffleCards(GridConfig layout, int seed)
        {
            var rng = new System.Random(seed);
            var cards = new List<CardData>();

            int total = layout.TotalCards;
            int pairs = total / 2;

            for (int i = 0; i < pairs; i++)
            {
                cards.Add(new CardData(cards.Count, i, i));
                cards.Add(new CardData(cards.Count, i, i));
            }

            // odd grid gets a lone wildcard that matches itself
            if (total % 2 != 0)
                cards.Add(new CardData(cards.Count, -1, -1));

            // shuffle in place
            for (var i = 1; i < cards.Count; i++)
            {
                var j = rng.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }

            return cards;
        }
    }
}

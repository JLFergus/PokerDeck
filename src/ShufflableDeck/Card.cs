namespace ShufflableDeck
{
    public enum CardSuits { Clubs, Diamonds, Hearts, Spades }
    public enum CardValues { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public class Card
    {
        private int _rank;
        public CardSuits Suit { get; private set; }
        public CardValues Value { get; private set; }

        public static bool operator ==(Card c1, Card c2)
        {
            return c1.Rank == c2.Rank;
        }
        public static bool operator !=(Card c1, Card c2)
        {
            return c1.Rank != c2.Rank;
        }
        public override bool Equals(object obj) { return this == (Card)obj; }

        public Card(CardValues value, CardSuits suit)
        {
            Suit = suit;
            Value = value;
            CalculateRank();
        }

        public string Name { get { return Value.ToString() + " of " + Suit.ToString(); } }

        // This will allow easy comparison between two cards to determine the high card. Not used in sorting
        private void CalculateRank()
        {
            _rank = System.Convert.ToInt32(Suit) + (System.Convert.ToInt32(Value) * 4);
        }
        public int Rank { get { return _rank; } }
    }
}

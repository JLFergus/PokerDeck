using System;
using System.Collections.Generic;
using System.Linq;

namespace ShufflableDeck
{
    public class Deck
    {
        // I ended up having to make rand static because my tests were running so fast that instantiating two decks one after the other ended up with the same random seed,
        // so the "TwoRandomDecksShouldNotBeEqual" test kept failing.
        private static Random rand = new Random();

        public Queue<Card> Cards { get; private set; }
        public static bool operator ==(Deck d1, Deck d2)
        {
            if (d1.Cards.Count != d2.Cards.Count)
                return false;

            var l1 = d1.Cards.ToList();
            var l2 = d2.Cards.ToList();
            for (var i = 0; i < l1.Count; i++)
            {
                if (l1[i] != l2[i])
                    return false;
            }

            return true;
        }
        public static bool operator !=(Deck d1, Deck d2) { return !(d1 == d2); }
        public override bool Equals(object obj) { return this == (Deck)obj; }

        private Queue<Card> OrderedCards()
        {
            var cards = new Queue<Card>();

            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
            {
                foreach (CardValues value in Enum.GetValues(typeof(CardValues)))
                {
                    cards.Enqueue(new Card(value, suit));
                }
            }

            // Testing Purposes only. Uncomment to see the generated deck.
            //var deckList = deck.ToList();
            //foreach(var card in deckList)
            //{
            //    Console.WriteLine(card.Name);
            //}
            return cards;
        }
        private Queue<Card> RandomCards()
        {
            var deck = new Queue<Card>();
            // I thought about setting cardsLeft to OrderedCards().ToList() instead of repeating the nested for loop,
            // but I suspect this will be more performant
            var cardsLeft = new List<Card>();
            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
            {
                foreach (CardValues value in Enum.GetValues(typeof(CardValues)))
                {
                    cardsLeft.Add(new Card(value, suit));
                }
            }

            while (cardsLeft.Count > 0)
            {
                var index = (int)rand.Next(cardsLeft.Count);
                deck.Enqueue(cardsLeft[index]);
                cardsLeft.RemoveAt(index);
            }

            // Testing Purposes only. Uncomment to see the deck as it's generated.
            //var deckList = deck.ToList();
            //foreach (var card in deckList)
            //{
            //    Console.WriteLine(card.Name);
            //}
            //Console.WriteLine();

            return deck;
        }

        public static Deck NewDeck = new Deck();

        public Deck(Queue<Card> cards) { Cards = cards; }
        public Deck(bool random = false) { Cards = random ? RandomCards() : OrderedCards(); }
        // Sort Order is as follows: by Suit, then in ascending order by value (Aces high)
        // Suit Priority is as follows: Clubs < Diamonds < Hearts < Spades
        public void Sort()
        {
            var suitSize = Enum.GetValues(typeof(CardValues)).Length;
            Card[] clubs = new Card[suitSize];
            Card[] diamonds = new Card[suitSize];
            Card[] hearts = new Card[suitSize];
            Card[] spades = new Card[suitSize];

            // sort cards into bins by suit. Because I already know there are 12 cards in each suit with no duplicates,
            // I can cheat here and place them directly in their sorted position.
            while (Cards.Count > 0)
            {
                var card = Cards.Dequeue();
                var index = Array.IndexOf(Enum.GetValues(typeof(CardValues)), card.Value);
                switch (card.Suit)
                {
                    case CardSuits.Clubs:
                        clubs[index] = card;
                        break;
                    case CardSuits.Diamonds:
                        diamonds[index] = card;
                        break;
                    case CardSuits.Hearts:
                        hearts[index] = card;
                        break;
                    default:
                        spades[index] = card;
                        break;
                }
            }

            // Now I just have to re-build the Deck, essentially stacking all the bins on top of each other
            foreach (var card in clubs)
                Cards.Enqueue(card);
            foreach (var card in diamonds)
                Cards.Enqueue(card);
            foreach (var card in hearts)
                Cards.Enqueue(card);
            foreach (var card in spades)
                Cards.Enqueue(card);
        }

        public void RiffleShuffle(int precision = 8)
        {
            // 0 or negative precision produces Out of Range errors in Random. 
            // Any higher precision than (count / 2 - 1) and we risk "splitting" the deck into one stack with all the cards, 
            // and one with zero cards, effectively not shuffling
            if (precision < 1 || precision > (Cards.Count / 2) - 1)
                throw new ArgumentOutOfRangeException();

            var shuffledDeck = new Queue<Card>();
            var leftDeck = Cards;
            var rightDeck = new Queue<Card>();
            
            // split the deck. Precision here determines the range of random deviation from the exact center of the deck
            var plusOrMinus = Math.Pow(-1, rand.Next());
            var distFromCenter = rand.Next(precision);
            var splitIndex = (Cards.Count / 2) + (distFromCenter * plusOrMinus);

            for(var i = 0; i < splitIndex; i++)
                rightDeck.Enqueue(leftDeck.Dequeue());

            // Testing Purposes only. Uncomment to see the deck as it's generated.
            //var leftList = leftDeck.ToList();
            //var rightList = rightDeck.ToList();
            //Console.WriteLine("splitIndex: " + splitIndex);
            //Console.WriteLine("leftDeck: " + leftDeck.Count + " cards:");
            //foreach (var card in leftList)
            //    Console.WriteLine(card.Name);
            //Console.WriteLine("rightDeck: " + rightDeck.Count + " cards");
            //foreach (var card in rightList)
            //    Console.WriteLine("\t" + card.Name);
            //Console.WriteLine();

            // shuffle the two new decks together. Precision here determines the maximum number of cards that could be shuffled in from each stack at once.
            var left = rand.Next() % 2 == 0; // determines starting deck
            while(leftDeck.Count > 0 && rightDeck.Count > 0)
            {
                // the -1 +1 business here is to keep the stackSize from coming up zero, thereby accidentally allowing up to 2*precision cards from one stack
                var currentDeck = left ? leftDeck : rightDeck;
                var stackSize = rand.Next(precision - 1) + 1;
                if (stackSize > currentDeck.Count)
                    stackSize = currentDeck.Count;

                // Testing purposes only. Uncomment to see deck as it's being built
                //var tab = left ? "" : "\t"; 

                for (int i = 0; i < stackSize; i++)
                {
                    // Testing purposes only. Uncomment to see deck as it's being built
                    //Console.WriteLine(tab + currentDeck.Peek().Name); 
                    shuffledDeck.Enqueue(currentDeck.Dequeue());
                }
                left = !left;
            }
            // shuffling in remaining deck. At this point, one stack will already be at count=0, so only the other one will actually add anything
            while (leftDeck.Count > 0)
            {
                // Testing purposes only. Uncomment to see deck as it's being built
                //Console.WriteLine("\t" + leftDeck.Peek().Name); 
                shuffledDeck.Enqueue(leftDeck.Dequeue());
            }
            while (rightDeck.Count > 0)
            {
                // Testing purposes only. Uncomment to see deck as it's being built
                //Console.WriteLine(rightDeck.Peek().Name); 
                shuffledDeck.Enqueue(rightDeck.Dequeue());
            }

            // Testing purposes only. Uncomment to see deck as it's being built
            //Console.WriteLine(); 
            
            Cards = shuffledDeck;
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShufflableDeck;

namespace ShufflableDeckUnitTests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_NewCardShouldSaveSuitAndValue()
        {
            var testCard = new Card(CardValues.Eight, CardSuits.Diamonds);
            Assert.AreEqual(testCard.Suit, CardSuits.Diamonds);
            Assert.AreEqual(testCard.Value, CardValues.Eight);
        }

        [TestMethod]
        public void Card_NewCardShouldReturnName()
        {
            var testCard = new Card(CardValues.Eight, CardSuits.Diamonds);
            Assert.AreEqual(testCard.Name, "Eight of Diamonds");
        }

        [TestMethod]
        public void Card_NewCardShouldCalculateRank()
        {
            var testCard = new Card(CardValues.Queen, CardSuits.Clubs);
            Assert.AreEqual(testCard.Rank, 40);

            testCard = new Card(CardValues.Eight, CardSuits.Diamonds);
            Assert.AreEqual(testCard.Rank, 25);

            testCard = new Card(CardValues.Two, CardSuits.Hearts);
            Assert.AreEqual(testCard.Rank, 2);

            testCard = new Card(CardValues.Ace, CardSuits.Spades);
            Assert.AreEqual(testCard.Rank, 51);
        }

        [TestMethod]
        public void Card_ComparingSameCardsShouldReturnTrue()
        {
            var c1 = new Card(CardValues.Eight, CardSuits.Spades);
            var c2 = new Card(CardValues.Eight, CardSuits.Spades);
            Assert.IsTrue(c1 == c2);
            Assert.IsFalse(c1 != c2);
        }

        [TestMethod]
        public void Card_ComparingDifferentCardsShouldReturnFalse()
        {
            // should reject if suit and value differ
            var c1 = new Card(CardValues.Eight, CardSuits.Spades);
            var c2 = new Card(CardValues.Nine, CardSuits.Diamonds);
            Assert.IsFalse(c1 == c2);
            Assert.IsTrue(c1 != c2);

            // should reject even if same suit
            c1 = new Card(CardValues.Eight, CardSuits.Spades);
            c2 = new Card(CardValues.Nine, CardSuits.Spades);
            Assert.IsFalse(c1 == c2);
            Assert.IsTrue(c1 != c2);

            // should reject even if same value
            c1 = new Card(CardValues.Eight, CardSuits.Spades);
            c2 = new Card(CardValues.Eight, CardSuits.Clubs);
            Assert.IsFalse(c1 == c2);
            Assert.IsTrue(c1 != c2);
        }
    }
}

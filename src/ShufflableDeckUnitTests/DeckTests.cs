using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShufflableDeck;
using System;
using System.Collections.Generic;

namespace ShufflableDeckUnitTests
{
    [TestClass]
    public class DeckTests
    {
        [TestMethod]
        public void Deck_OverloadedOperatorsShouldWork()
        {
            var d1 = new Deck();
            var d2 = new Deck();
            Assert.IsTrue(d1 == d2);
            Assert.IsFalse(d1 != d2);
        }
        [TestMethod]
        public void Deck_BasicInstantiationCreatesNewDeck()
        {
            var deck = new Deck();
            Assert.AreEqual(deck, Deck.NewDeck);
        }

        [TestMethod]
        public void Deck_RandomDeckShouldHaveNoRepeatCards()
        {
            var seenCards = new List<int>();
            var deck = new Deck(random: true);

            while (deck.Cards.Count != 0)
            {
                var card = deck.Cards.Dequeue();
                Assert.IsFalse(seenCards.Contains(card.Rank));
                seenCards.Add(card.Rank);
            }
        }
        [TestMethod]
        public void Deck_TwoRandomDecksShouldNotBeEqual()
        {
            // I realize this could theoretically produce two random decks that are exactly the same, 
            // but the odds of that happening are astronomical, so I'm willing to let it go
            var r1 = new Deck(random: true);
            var newDeck = new Deck();
            Assert.IsFalse(r1 == newDeck);

            var r2 = new Deck(random: true);
            Assert.AreNotEqual(r2,newDeck);

            Assert.AreNotEqual(r1,r2);
        }

        [TestMethod]
        public void Deck_SortedDeckShouldEqualNewDeck()
        {
            var d1 = new Deck();
            var d2 = new Deck(random: true);
            d2.Sort();

            Assert.AreEqual(d1, d2);
        }

        [TestMethod]
        public void Deck_ShuffledDeckShouldNotEqualOriginal()
        {
            // test shuffling new deck. Shuffling twice to ensure it doesn't just unshuffle it
            var d1 = new Deck();
            d1.RiffleShuffle();
            Assert.AreNotEqual(d1, Deck.NewDeck);
            d1.RiffleShuffle();
            Assert.AreNotEqual(d1, Deck.NewDeck);

            // test shuffling random deck. Shuffling twice to ensure it doesn't just unshuffle it
            var d2 = new Deck(random: true);
            var d3 = new Deck(d2.Cards);
            d2.RiffleShuffle();
            Assert.AreNotEqual(d2, d3);
            d2.RiffleShuffle();
            Assert.AreNotEqual(d2, d3);

            // verify that entering a precision still works
            var d4 = new Deck();
            d4.RiffleShuffle(15);
            Assert.AreNotEqual(d1, Deck.NewDeck);

            // verify that precision = 1 still works
            var d5 = new Deck();
            d5.RiffleShuffle(1);
            Assert.AreNotEqual(d5, Deck.NewDeck);

            // RiffleShuffle Allows as high a precision value as (n/2) - 1. 
            // Any higher than that and you risk "splitting" the deck into one stack of 52 cards and 1 of 0 cards
            var d6 = new Deck();
            d6.RiffleShuffle(25);
            Assert.AreNotEqual(d6, Deck.NewDeck);
        }

        [TestMethod]
        public void Deck_ShouldThrowExceptionsIfPrecisionIsOutOfRange()
        {
            var deck = new Deck();
            var errors = 0;
            try
            {
                deck.RiffleShuffle(0);
            } catch(ArgumentOutOfRangeException e) {
                errors++;
            }

            try
            {
                deck.RiffleShuffle(-5);
            }
            catch (ArgumentOutOfRangeException e)
            {
                errors++;
            }

            try
            {
                deck.RiffleShuffle(26);
            }
            catch (ArgumentOutOfRangeException e)
            {
                errors++;
            }

            try
            {
                deck.RiffleShuffle(1000);
            }
            catch (ArgumentOutOfRangeException e)
            {
                errors++;
            }

            Assert.AreEqual(errors, 4);
            Assert.AreEqual(deck, Deck.NewDeck);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Model = Satori.Model;

namespace Satori
{
    class FlashcardQuiz
    {
        public List<Model.Card> Cards { get; private set; }
        public List<Model.Card> CurrentDeck { get; private set; }
        public List<Model.Card> CorrectPile { get; private set; }
        public List<Model.Card> IncorrectPile { get; private set; }
        private int currentDeckIndex { get; set; }
        public int TotalCards { get; private set; }
        public int CorrectCards { get; private set; }

        public FlashcardQuiz(List<Model.Deck> decks)
        {
            Cards = new List<Model.Card>();
            IncorrectPile = new List<Model.Card>();
            CorrectPile = new List<Model.Card>();

            foreach(var d in decks)
            {
                var cards = Model.Card.SelectAllCardsByDeckID(d.DeckID);
                foreach(var c in cards)
                {
                    Cards.Add(c);
                }
            }

            currentDeckIndex = 0;
            TotalCards = Cards.Count;
            CorrectCards = 0;

            CopyCardsToCurrentDeck();
        }

        private void ShuffleDeck(List<Model.Card> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void CopyCardsToCurrentDeck()
        {
            CurrentDeck = new List<Model.Card>();

            foreach (var c in Cards)
            {
                CurrentDeck.Add(c);
            }

            ShuffleDeck(CurrentDeck);
        }

        public Model.Card DrawCard()
        {
            if (CurrentDeck.Count == 0)
            {
                return null;
            }

            return CurrentDeck[currentDeckIndex];
        }

        public string CheckResponse(string response)
        {
            Side side = Side.Front;
            string answer = getExpectedAnswer(side);

            string replyToAnswer;

            if (answer == response)
            {
                CorrectPile.Add(CurrentDeck[currentDeckIndex]);
                replyToAnswer = "Correct! The answer is\n" + CurrentDeck[currentDeckIndex].BackText;
                ++CorrectCards;
            }
            else
            {
                IncorrectPile.Add(CurrentDeck[currentDeckIndex]);
                replyToAnswer =  "Sorry! The answer is\n" + CurrentDeck[currentDeckIndex].BackText;
            }

            setupForNextCard();

            return replyToAnswer;
        }

        private void setupForNextCard()
        {
            ++currentDeckIndex;

            if (currentDeckIndex >= CurrentDeck.Count)
            {
                currentDeckIndex = 0;
                CurrentDeck = IncorrectPile;
                ShuffleDeck(CurrentDeck);
                IncorrectPile = new List<Model.Card>();
            }
        }

        private string getExpectedAnswer(Side side)
        {
            if (Side.Front == side)
            {
                return CurrentDeck[currentDeckIndex].BackText;
            }
            else
            {
                return CurrentDeck[currentDeckIndex].FrontText;
            }
        }

        public void Reset()
        {
            if (Cards.Count == 0)
            {
                return;
            }

            CopyCardsToCurrentDeck();
            CorrectPile.Clear();
            IncorrectPile.Clear();

            CorrectCards = 0;
            currentDeckIndex = 0;
        }

    }
}

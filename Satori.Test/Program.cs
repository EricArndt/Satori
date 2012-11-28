using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Satori.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Satori.Model.Deck.LoadAllVisibleDecks().ToList().ForEach(deck => Console.WriteLine("{0}: {1}", deck.DeckID, deck.Name));
        }
    }
}

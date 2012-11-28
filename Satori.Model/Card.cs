using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Satori.Model
{
    public class Card
    {
        public int CardID { get; set; }
        public int DeckID { get; set; }
        public string FrontText { get; set; }
        public int? FrontLanguage { get; set; }
        public string BackText { get; set; }
        public int? BackLanguage { get; set; }
        public byte[] Picture { get; set; }

        public Card()
        {
            CardID = -1;
            FrontText = "";
            FrontLanguage = null;
            BackText = "";
            BackLanguage = null;
            Picture = null;
        }

        private static Card LoadCardFromDataRow(DataRow row)
        {
            return new Card
            {
                CardID = (int)row["CardID"],
                DeckID = (int)row["DeckID"],
                FrontText = (string)row["frontText"],
                FrontLanguage = row["frontLanguage"] is DBNull ? null :(int?)row["frontLanguage"] ,
                BackText = (string)row["backText"],
                BackLanguage = row["backLanguage"] is DBNull ? null : (int?)row["backLanguage"],
                Picture = row["picture"] is DBNull ? null : (byte[])row["picture"],
            };
        }

        public static void AddCard(int deckID, int cardID, string frontText, int? frontLanguage, string backText, int? backLanguage, byte[] picture)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("addCard", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DeckID", deckID);
                command.Parameters.AddWithValue("@frontText", frontText);
                command.Parameters.AddWithValue("@CardID", cardID);

                if (frontLanguage != null)
                {
                    command.Parameters.AddWithValue("@frontLanguage", frontLanguage);
                }
                else 
                {
                    command.Parameters.AddWithValue("@frontLanguage", DBNull.Value);
                }
                
                command.Parameters.AddWithValue("@backText", backText);

                if (backLanguage != null)
                { 
                    command.Parameters.AddWithValue("@backLanguage", backLanguage);
                }
                else
                {
                    command.Parameters.AddWithValue("@backLanguage", DBNull.Value);
                }

                if (picture != null)
                {
                    command.Parameters.AddWithValue("@picture", picture);
                }
                else
                {
                    command.Parameters.Add("@picture", SqlDbType.VarBinary, -1);
                    command.Parameters["@picture"].Value = DBNull.Value;
                }
                

                command.ExecuteNonQuery();
            }
        }

        public static void AddCard(Card card)
        {
            AddCard(card.DeckID, card.CardID, card.FrontText, card.FrontLanguage, card.BackText, card.BackLanguage, card.Picture);
        }

        public static void DeleteCard(Card card)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();

                var command = new SqlCommand("deleteCard", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CardID", card.CardID);

                command.ExecuteNonQuery();
            }
        }

        public static List<Card> SelectAllCardsByDeckID(int deckID)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();

                var command = new SqlCommand("selectCardsByDeckID", connection);

                command.Parameters.AddWithValue("@deckID", deckID);
                command.CommandType = CommandType.StoredProcedure;

                var adapter = new SqlDataAdapter(command);
                var set = new DataSet();

                adapter.Fill(set);

                var loadedCards = new List<Card>();

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    loadedCards.Add(LoadCardFromDataRow(row));
                }

                return loadedCards;
            }
        }

    }
}

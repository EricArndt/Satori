using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Satori.Model
{
    public class Deck
    {
        public int DeckID { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        private static Deck LoadDeckFromDataRow(DataRow row)
        {
            return new Deck
            {
                DeckID = (int)row["DeckID"],
                Name = (string)row["Name"],
                Created = (DateTime)row["Created"],
                Modified = (DateTime)row["Modified"],
            };
        }

        public static Deck LoadByDeckID(int deckID)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var deck = new Deck();

                var command = new SqlCommand("selectDeckByID", connection);
                command.Parameters.AddWithValue("@DeckID", deckID);
                command.CommandType = CommandType.StoredProcedure;

                var reader = command.ExecuteReader();

                while (reader.Read())
                { 
                    deck.DeckID = int.Parse(reader["DeckID"].ToString().Trim());
                    deck.Name = reader["Name"].ToString();
                    deck.Created = (System.DateTime)reader["Created"];
                    deck.Modified = (System.DateTime)reader["Modified"];
                }

                return deck;
            }
            
        }

        public static IEnumerable<Deck> LoadAllVisibleDecks()
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            { 
                connection.Open();

                var command = new SqlCommand("loadAllDecks", connection);
                var adapter = new SqlDataAdapter(command);
                var set = new DataSet();

                adapter.Fill(set);

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    yield return LoadDeckFromDataRow(row);
                }
            }
        }
        
        public static IEnumerable<Deck> LoadAllVisibleDecks(Language frontLanguage, Language backLanguage)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("exec searchDeckForLanguage @frontLanguage, @backLanguage", connection);

                command.Parameters.AddWithValue("@frontLanguage", frontLanguage == null ? null : frontLanguage.LanguageID as int?);
                command.Parameters.AddWithValue("@backLanguage", backLanguage == null ? null : backLanguage.LanguageID as int?);

                foreach (SqlParameter param in command.Parameters)
                {
                    if (param.Value == null)
                    {
                        param.Value = DBNull.Value;
                    }
                }

                var adapter = new SqlDataAdapter(command);
                var set = new DataSet();

                adapter.Fill(set);

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    yield return LoadDeckFromDataRow(row);
                }
            }
        }
        
        public static int CreateNewDeck(string name)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var command = new SqlCommand("addDeck", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Created", currentTime);
                command.Parameters.AddWithValue("@Modified", currentTime);

                return int.Parse(command.ExecuteScalar().ToString());
            }
        }

        public static void DeleteDeck(Deck d)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("deleteDeck", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DeckID", d.DeckID);
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateDeckTimestamp(Deck d)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("updateDeckTimestamp", connection);
                var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DeckID", d.DeckID);
                command.Parameters.AddWithValue("@Modified", currentTime);
                command.ExecuteNonQuery();
            }
        }
    }
}

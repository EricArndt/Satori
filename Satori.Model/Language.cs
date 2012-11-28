using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Satori.Model
{
    public class Language
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }

        private static Language LoadLanguageFromDataRow(DataRow row)
        {
            return new Language
            {
                LanguageID = (int)row["LanguageID"],
                Name = (string)row["Type"],
            };
        }

        public static IEnumerable<Language> LoadAllLanguages()
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();

                var command = new SqlCommand("loadAllLanguages", connection);
                var adapter = new SqlDataAdapter(command);
                var set = new DataSet();
                adapter.Fill(set);

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    yield return LoadLanguageFromDataRow(row);
                }
            }
        }

        public static void AddLanguage(string type)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();

                var command = new SqlCommand("addLanguage", connection);

                command.Parameters.AddWithValue("@Type", type);
                command.CommandType = CommandType.StoredProcedure;

                command.ExecuteNonQuery();
            }
        }

        public static Model.Language SelectLanguageByID(int languageID)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("selectLanguageByID", connection);
                command.Parameters.AddWithValue("@LanguageID", languageID);
                command.CommandType = CommandType.StoredProcedure;

                var Language = new Language();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Language.LanguageID = int.Parse(reader["LanguageID"].ToString().Trim());
                    Language.Name = reader["Type"].ToString();
                }

                return Language;
            }
        }


        public static bool DeleteLanguageByID(Language language)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=FlashcardDeckDB;Integrated Security=SSPI;"))
            {
                connection.Open();
                var command = new SqlCommand("deleteLanguageByID", connection);
                command.Parameters.AddWithValue("@LanguageID", language.LanguageID);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    return false;
                }
  
            }
        }
    }
}

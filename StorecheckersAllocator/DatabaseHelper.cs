using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace StorecheckersAllocator
{
    // Class to provide generic database commands and functionality
    public class DatabaseHelper
    {
        string database;
        SqlConnection dbConnection;

        public string Database { get { return database; } }

        // Constructor
        public DatabaseHelper(string database)
        {
            this.database = database;

            OpenDatabase();
        }

        // Method opens database and creates table if necessary.
        private void OpenDatabase()
        {
            // Set connection string
            string connectionString = "AttachDbFilename=|DataDirectory|\\" + database + ".mdf;" +
            "Data Source=.\\SQLEXPRESS;Integrated Security=True;User Instance=True";


            // Create DB connection
            dbConnection = new SqlConnection(connectionString);

            // Open DB
            try
            {
                dbConnection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Close DB
        public void CloseDatabase()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Method to execute a non query command
        public void ExecuteNonQuery(string commandString)
        {
            SqlCommand command = new SqlCommand(commandString, dbConnection);

            command.ExecuteNonQuery();
        }

        // Execute non query command which uses parameters as defined in commandParameters parameter
        public void ExecuteNonQueryWithParams(string commandString, Dictionary<string, object> commandParameters)
        {
            SqlCommand command = new SqlCommand(commandString, dbConnection);

            foreach (KeyValuePair<string, object> kvp in commandParameters)
            {
                command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            }

            command.ExecuteNonQuery();
        }

        public void PrintQuery(string commandString)
        {
            SqlCommand command = new SqlCommand(commandString, dbConnection);

            SqlDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                Console.Write("| ");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader[i] + " | ");
                }
                Console.WriteLine();
            }
        }

    }
}

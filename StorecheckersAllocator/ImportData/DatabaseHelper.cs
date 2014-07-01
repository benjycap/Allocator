using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace StorecheckersAllocator.ImportData
{
    // Class to provide generic database commands and functionality
    public class DatabaseHelper
    {
        // Connection string
        readonly string connectionString;

        string database;

        public string Database { get { return database; } }

        // Constructor
        public DatabaseHelper(string database)
        {
            this.database = database;
            connectionString = "AttachDbFilename=|DataDirectory|\\" + database + ".mdf;" +
                "Data Source=.\\SQLEXPRESS;Integrated Security=True;User Instance=True";
        }


        // Method to execute a non query command
        public void ExecuteNonQuery(string commandString)
        {
            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandString, dbConnection))
                {
                    try
                    {
                        dbConnection.Open();

                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        // Execute non query command which uses parameters as defined in commandParameters parameter
        public int ExecuteNonQueryWithParams(string commandString, SqlCommand commandWithParameters)
        {
            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                try
                {
                    dbConnection.Open();

                    commandWithParameters.CommandText = commandString;
                    commandWithParameters.Connection = dbConnection;

                    int queryVal = commandWithParameters.ExecuteNonQuery();

                    return (queryVal >= 1) ? queryVal : 0;
                }
                catch
                {   
                    throw;
                }
            }
        }

        public void PrintQuery(string commandString)
        {
            using (SqlConnection dbConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandString, dbConnection))
                {
                    try
                    {
                        dbConnection.Open();

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
                    catch
                    {
                        throw;
                    }
                }
            }
        }

    }
}

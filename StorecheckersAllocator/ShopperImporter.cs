using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace StorecheckersAllocator
{
    public class ShopperImporter
    {
        private static readonly string ShopperDB = "ShopperDatabase";
        // Designate column relations between downloaded SQL database and source data
        private static readonly Dictionary<string, int> columnMap = new Dictionary<string, int>()
     {
         {"ShopperID", 0},
         {"FirstName", 1},
         {"LastName", 3},
         {"AddressLine1", 4},
         {"AddressLine2", 5},
         {"AddressCity", 6},
         {"AddressCounty", 7},
         {"AddressPostcode", 9},
         {"PhoneNumber1", 10},
         {"PhoneNumber2", 11},
         {"MobileNumber", 24},
         {"Email", 12},
         {"Sex", 13},
         {"Birthdate", 16},
         {"AverageRating", 19},
         {"Comments", 21}
     };
        

        /*private static readonly Dictionary<string, string> columnMap = new Dictionary<string, string>()
        {
            {"ShopperID", "ShopperID"},
            {"First_Name", "FirstName"},
            {"Last_Name", "LastName"},
            {"Address_1", "AddressLine1"},
            {"Address_2", "AddressLine2"},
            {"City", "AddressCity"},
            {"State", "AddressCounty"},
            {"Postal_Code", "AddressPostcode"},
            {"Phone_1", "PhoneNumber1"},
            {"Phone_2", "PhoneNumber2"},
            {"Mobile_Phone", "MobileNumber"},
            {"Email", "Email"},
            {"Sex", "Sex"},
            {"Birthdate", "Birthdate"},
            {"Avg_Rating", "AverageRating"},
            {"Comments", "Comments"}
        };*/
        
        private DatabaseHelper dbHelper;

        public ShopperImporter()
        {
            dbHelper = new DatabaseHelper(ShopperDB);
        }

        public void Run()
        {
            CreateTableIfNotExists();

           // InsertTestValues();

            //dbHelper.PrintQuery("SELECT * FROM Shoppers");

            ImportData();
        }

        private void CreateTableIfNotExists()
        {
            // Parsing - phone: strip non numbers and spaces, add 0 at beginning if needed
            string createTable = @"CREATE TABLE Shoppers 
                (ShopperID INT PRIMARY KEY NOT NULL, 
                LastName NVARCHAR(255) NOT NULL, 
                FirstName NVARCHAR(255) NOT NULL, 
                AddressLine1 NVARCHAR(255) NOT NULL, 
                AddressLine2 NVARCHAR(255) NOT NULL, 
                AddressCity NVARCHAR(255) NOT NULL, 
                AddressCounty NVARCHAR(255) NOT NULL, 
                AddressPostcode NVARCHAR(255) NOT NULL, 
                PhoneNumber1 NVARCHAR(20) NOT NULL, 
                PhoneNumber2 NVARCHAR(20),
                MobileNumber NVARCHAR(20), 
                Email NVARCHAR(255) NOT NULL, 
                Sex NVARCHAR(1) NOT NULL CHECK (Sex IN('M', 'F')), 
                Birthdate DATE NOT NULL, 
                AverageRating TINYINT NOT NULL CHECK (AverageRating <= 10), 
                Comments NVARCHAR(MAX)
                )";

            string commandString = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Shoppers') " + createTable;

            dbHelper.ExecuteNonQuery(commandString);
        }

        private void InsertTestValues()
        {
            string insert = @"INSERT INTO Shoppers (ShopperID, LastName, FirstName, AddressLine1, 
                AddressLine2, AddressCity, AddressCounty, AddressPostcode, PhoneNumber1, PhoneNumber2,
                MobileNumber, Email, Sex, Birthdate, AverageRating, Comments) ";

            string values = @"VALUES (@ShopperID, @LastName, @FirstName, @AddressLine1, 
                @AddressLine2, @AddressCity, @AddressCounty, @AddressPostcode, @PhoneNumber1, @PhoneNumber2,
                @MobileNumber, @Email, @Sex, @Birthdate, @AverageRating, @Comments)";

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Parameters.Add("ShopperID", SqlDbType.NVarChar).Value = 1;
                sqlCommand.Parameters.Add("LastName", SqlDbType.NVarChar).Value = "personson";
                sqlCommand.Parameters.Add("FirstName", SqlDbType.NVarChar).Value = "person";
                sqlCommand.Parameters.Add("AddressLine1", SqlDbType.NVarChar).Value = "123 Somewhere Street";
                sqlCommand.Parameters.Add("AddressLine2", SqlDbType.NVarChar).Value = "Heretown";
                sqlCommand.Parameters.Add("AddressCity", SqlDbType.NVarChar).Value = "Therecity";
                sqlCommand.Parameters.Add("AddressCounty", SqlDbType.NVarChar).Value = "The Shire";
                sqlCommand.Parameters.Add("AddressPostcode", SqlDbType.NVarChar).Value = "SW1 123";
                sqlCommand.Parameters.Add("PhoneNumber1", SqlDbType.NVarChar).Value = "07980123456";
                sqlCommand.Parameters.Add("PhoneNumber2", SqlDbType.NVarChar).Value = DBNull.Value;
                sqlCommand.Parameters.Add("MobileNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                sqlCommand.Parameters.Add("Email", SqlDbType.NVarChar).Value = "a@b.com";
                sqlCommand.Parameters.Add("Sex", SqlDbType.NVarChar).Value = "M";
                sqlCommand.Parameters.Add("Birthdate", SqlDbType.Date).Value = new DateTime(1989, 05, 20);
                sqlCommand.Parameters.Add("AverageRating", SqlDbType.TinyInt).Value = 6;
                sqlCommand.Parameters.Add("Comments", SqlDbType.NVarChar).Value = "hes just some guy you know";
                
                dbHelper.ExecuteNonQueryWithParams(insert + values, sqlCommand);
            }
        }

        private void ImportData()
        {
            using (StreamReader reader = new StreamReader(@"C:\a\shoppers.sass"))
            {
                string commandString = @"INSERT INTO Shoppers
                    VALUES (@ShopperID, @LastName, @FirstName, @AddressLine1, @AddressLine2,
                    @AddressCity, @AddressCounty, @AddressPostcode, @PhoneNumber1, @PhoneNumber2,
                    @MobileNumber, @Email, @Sex, @Birthdate, @AverageRating, @Comments)";

                // Skip first line (column headers)
                reader.ReadLine();
                
                while(!reader.EndOfStream)
                {
                    List<string> data = reader.ReadLine().Split('\t').ToList();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        foreach (ColumnInfo.Column column in ColumnInfo.Columns)
                        {
                            object value = data[column.SourceColumn];

                            switch (column.Header)
                            {
                                case "ShopperID":
                                    value = Int32.Parse((string)value);
                                    break;
                                case "AverageRating":
                                    value = Int32.Parse((string)value);
                                    break;
                                case "Birthdate":
                                    value = DateTime.ParseExact((string)value, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                                    break;
                                case "PhoneNumber2":
                                    value = ((string)value == "") ? DBNull.Value : value;
                                    break;
                                case "MobileNumber":
                                    value = ((string)value == "") ? DBNull.Value : value;
                                    break;
                                default:
                                    value = data[column.SourceColumn];
                                    break;
                            }

                            sqlCommand.Parameters.Add(column.Header, column.DataType).Value = value;
                        }

                        dbHelper.ExecuteNonQueryWithParams(commandString, sqlCommand);

                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StorecheckersAllocator.ImportData
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
        
        private DatabaseHelper dbHelper;


        public ShopperImporter()
        {
            dbHelper = new DatabaseHelper(ShopperDB);
        }


        public void Run()
        {
            CreateTableIfNotExists();

            Console.WriteLine(ImportData());

            Console.WriteLine(UpdateData());
        }

        private void CreateTableIfNotExists()
        {
            // Table definition is defined in two places : here and ColumnInfo static constructor. should refactor.
            string createTable = @"CREATE TABLE Shoppers 
                (ShopperID INT PRIMARY KEY NOT NULL, 
                LastName NVARCHAR(255) NOT NULL, 
                FirstName NVARCHAR(255) NOT NULL, 
                AddressLine1 NVARCHAR(255) NOT NULL, 
                AddressLine2 NVARCHAR(255) NOT NULL, 
                AddressCity NVARCHAR(255) NOT NULL, 
                AddressCounty NVARCHAR(255) NOT NULL, 
                AddressPostcode NVARCHAR(255) NOT NULL, 
                PhoneNumber1 NVARCHAR(255) NOT NULL, 
                PhoneNumber2 NVARCHAR(255),
                MobileNumber NVARCHAR(255), 
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

        private int ImportData()
        {
            /*
             * TODO create exception handling for field input errors
             */

            string insert = @"INSERT INTO Shoppers
                    VALUES (@ShopperID, @LastName, @FirstName, @AddressLine1, @AddressLine2,
                    @AddressCity, @AddressCounty, @AddressPostcode, @PhoneNumber1, @PhoneNumber2,
                    @MobileNumber, @Email, @Sex, @Birthdate, @AverageRating, @Comments)";

            string commandString = "IF NOT EXISTS (SELECT * FROM Shoppers WHERE ShopperID = @ShopperID) " + insert;

            int affectedRows = 0;

            using (StreamReader reader = new StreamReader(@"C:\a\shoppers.sass"))
            {
                // Skip first line (column headers)
                reader.ReadLine();
                
                while(!reader.EndOfStream)
                {
                    // Parse tab-delimited data into a list
                    List<string> data = reader.ReadLine().Split('\t').ToList();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        foreach (ColumnInfo.Column column in ColumnInfo.Columns)
                        {
                            // Cleanse data value to be in correct format for SqlCommand.SqlParameterCollection
                            object value = CleanseValue(data[column.SourceColumn], column.Header);

                            // Add value to collection
                            sqlCommand.Parameters.Add(column.Header, column.DataType).Value = value;
                        }

                        // Execute insert/update command with attached parameters
                        affectedRows += dbHelper.ExecuteNonQueryWithParams(commandString, sqlCommand);

                    }
                }
            }
            return affectedRows;
        }

        private int UpdateData()
        {
            string commandString = @"UPDATE Shoppers SET
                    LastName=@LastName, FirstName=@FirstName, AddressLine1=@AddressLine1, 
                    AddressLine2=@AddressLine2, AddressCity=@AddressCity, AddressCounty=@AddressCounty, 
                    AddressPostcode=@AddressPostcode, PhoneNumber1=@PhoneNumber1, PhoneNumber2=@PhoneNumber2,
                    MobileNumber=@MobileNumber, Email=@Email, Sex=@Sex, Birthdate=@Birthdate, 
                    AverageRating=@AverageRating, Comments=@Comments
                    WHERE ShopperID LIKE @ShopperID";

            int affectedRows = 0;

            using (StreamReader reader = new StreamReader(@"C:\a\shoppers.sass"))
            {
                // Skip first line (column headers)
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    // Parse tab-delimited data into a list
                    List<string> data = reader.ReadLine().Split('\t').ToList();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        foreach (ColumnInfo.Column column in ColumnInfo.Columns)
                        {
                            // Cleanse data value to be in correct format for SqlCommand.SqlParameterCollection
                            object value = CleanseValue(data[column.SourceColumn], column.Header);

                            // Add value to collection
                            sqlCommand.Parameters.Add(column.Header, column.DataType).Value = value;
                        }

                        // Execute insert/update command with attached parameters
                        affectedRows += dbHelper.ExecuteNonQueryWithParams(commandString, sqlCommand);

                    }
                }
            }
            return affectedRows;
        }

        #region Cleanse Methods

        private object CleanseValue(object value, string header)
        {
            switch (header)
            {
                case "LastName":
                    return CleanseNameField((string)value);
                case "FirstName":
                    return CleanseNameField((string)value);
                case "AddressLine1":
                    return CleanseAddressField((string)value);
                case "AddressLine2":
                    return CleanseAddressField((string)value);
                case "AddressCity":
                    return CleanseAddressField((string)value);
                case "AddressCounty":
                    return CleanseAddressField((string)value);
                case "AddressPostcode":
                    return ((string)value).ToUpper();
                case "PhoneNumber1":
                    return CleansePhoneField((string)value, true);
                case "PhoneNumber2":
                    return CleansePhoneField((string)value, false);
                case "MobileNumber":
                    return CleansePhoneField((string)value, false);
                case "Birthdate":
                    return DateTime.ParseExact((string)value, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                default:
                    return value;
            }
        }

        private string CleanseNameField(string s)
        {
            s = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());

            return s;
        }

        private string CleanseAddressField(string s)
        {
            s = s.Replace("\"", "");
            s = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());

            return s;
        }

        private object CleansePhoneField(string s, bool required)
        {
            // Remove all non numeric values
            s = Regex.Replace(s, @"\D", "");
            // Add 0 at beginning if necessary (must be non empty string)
            if (s != "" && !s.StartsWith("0")) 
                s = s.Insert(0, "0");

            if (required)
                return s;
            else
                return (s == "") ? (object)DBNull.Value : s;
        }

        #endregion

    }
}

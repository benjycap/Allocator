using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorecheckersAllocator
{
    public class ShopperImporter
    {
        private static readonly string ShopperDB = "ShopperDatabase";
        
        private DatabaseHelper dbHelper;

        public void Run()
        {
            dbHelper = new DatabaseHelper(ShopperDB);

            CreateTableIfNotExists();

            //InsertTestValuesManual();
            InsertTestValues();

            dbHelper.PrintQuery("SELECT * FROM Shoppers");

            dbHelper.CloseDatabase();
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
                PhoneNumber1 NVARCHAR(11) NOT NULL, 
                PhoneNumber2 NVARCHAR(11),
                MobileNumber NVARCHAR(11), 
                Email NVARCHAR(255) NOT NULL, 
                Sex NVARCHAR(1) NOT NULL CHECK (Sex IN('M', 'F')), 
                Birthdate DATE NOT NULL, 
                AverageRating TINYINT NOT NULL CHECK (AverageRating <= 10), 
                Comments NVARCHAR(MAX)
                )";

            string command = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Shoppers') " + createTable;

            dbHelper.ExecuteNonQuery(command);
        }

        private void InsertTestValues()
        {
            Dictionary<string, object> parameters;

            parameters.Add("ShopperID, 

            //LastName, FirstName, AddressLine1, AddressLine2, AddressCity,
            //    AddressCounty, AddressPostcode, PhoneNumber1, PhoneNumber2,
            //   MobileNumber, Email, Sex, Birthdate, AverageRating, Comments
        }

        private void InsertTestValuesManual()
        {
            string insert = @"INSERT INTO Shoppers (ShopperID, LastName, FirstName, AddressLine1, 
                AddressLine2, AddressCity, AddressCounty, AddressPostcode, PhoneNumber1, PhoneNumber2,
                MobileNumber, Email, Sex, Birthdate, AverageRating, Comments) ";
                
            string values = @"VALUES (1, 'personson', 'person', '123 Somewhere Street', 'Heretown', 
                'Therecity', 'The Shire', 'SW1 123', 07980123456, NULL, NULL, 'a@b.com', 'M', 
                Convert(DATE, '20/05/1989', 103), 6, 'hes just some guy you know')";

            dbHelper.ExecuteNonQuery(insert + values);
        }
    }
}

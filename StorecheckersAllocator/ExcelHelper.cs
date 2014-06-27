using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;

namespace StorecheckersAllocator
{
    public class ExcelHelper
    {
        readonly string excelConnString;

        public ExcelHelper(string filePath)
        {
            excelConnString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0\"", filePath);
        }

        public OleDbDataReader getExcelData()
        {
            string commandString = "SELECT * FROM";

            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                try
                {
                    excelConnection.Open();

                    System.Console.WriteLine(excelConnection.GetSchema().ToString());
                }
                catch (OleDbException)
                {
                    throw;
                }
            }
            return null;
        }
    }
}

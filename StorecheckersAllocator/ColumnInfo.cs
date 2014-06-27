using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace StorecheckersAllocator
{
    public static class ColumnInfo
    {
        public static List<Column> Columns { get; private set; }

        public struct Column
        {
            public string Header { get; private set; }
            public SqlDbType DataType { get; private set; }
            public int SourceColumn { get; private set; }

            public Column(string header, SqlDbType dataType, int sourceColumn)
                : this()
            {
                Header = header;
                DataType = dataType;
                SourceColumn = sourceColumn;
            }
        }

        static ColumnInfo()
        {
            Columns = new List<Column>();

            Columns.Add(new Column("ShopperID", SqlDbType.NVarChar, 0));
            Columns.Add(new Column("FirstName", SqlDbType.NVarChar, 1));
            Columns.Add(new Column("LastName", SqlDbType.NVarChar, 3));
            Columns.Add(new Column("AddressLine1", SqlDbType.NVarChar, 4));
            Columns.Add(new Column("AddressLine2", SqlDbType.NVarChar, 5));
            Columns.Add(new Column("AddressCity", SqlDbType.NVarChar, 6));
            Columns.Add(new Column("AddressCounty", SqlDbType.NVarChar, 7));
            Columns.Add(new Column("AddressPostcode", SqlDbType.NVarChar, 9));
            Columns.Add(new Column("PhoneNumber1", SqlDbType.NVarChar, 10));
            Columns.Add(new Column("PhoneNumber2", SqlDbType.NVarChar, 11));
            Columns.Add(new Column("MobileNumber", SqlDbType.NVarChar, 24));
            Columns.Add(new Column("Email", SqlDbType.NVarChar, 12));
            Columns.Add(new Column("Sex", SqlDbType.NVarChar, 13));
            Columns.Add(new Column("Birthdate", SqlDbType.Date, 16));
            Columns.Add(new Column("AverageRating", SqlDbType.TinyInt, 19));
            Columns.Add(new Column("Comments", SqlDbType.NVarChar, 21));
        }
    }
}
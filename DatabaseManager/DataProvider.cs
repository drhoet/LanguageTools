using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager {
    class DataProvider {
        private string tableName;
        private DbCommand command;

        public DataProvider(DbConnection conn, string tableName) {
            command = conn.CreateCommand();
            this.tableName = tableName;
        }

        private int rowCountValue = -1;

        public int RowCount {
            get {
                // Return the existing value if it has already been determined. 
                if(rowCountValue != -1) {
                    return rowCountValue;
                }

                // Retrieve the row count from the database.
                command.CommandText = "SELECT COUNT(*) FROM " + tableName;
                rowCountValue = Convert.ToInt32(command.ExecuteScalar());
                return rowCountValue;
            }
        }

        private DataColumnCollection columnsValue;

        public DataColumnCollection Columns {
            get {
                // Return the existing value if it has already been determined. 
                if(columnsValue != null) {
                    return columnsValue;
                }

                // Retrieve the column information from the database.
                command.CommandText = "SELECT * FROM " + tableName;
                DbDataAdapter adapter = new SQLiteDataAdapter();
                adapter.SelectCommand = command;
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                adapter.FillSchema(table, SchemaType.Source);
                columnsValue = table.Columns;
                return columnsValue;
            }
        }

        private string commaSeparatedListOfColumnNamesValue = null;

        private string CommaSeparatedListOfColumnNames {
            get {
                // Return the existing value if it has already been determined. 
                if(commaSeparatedListOfColumnNamesValue != null) {
                    return commaSeparatedListOfColumnNamesValue;
                }

                // Store a list of column names for use in the 
                // SupplyPageOfData method.
                System.Text.StringBuilder commaSeparatedColumnNames =
                    new System.Text.StringBuilder();
                bool firstColumn = true;
                foreach(DataColumn column in Columns) {
                    if(!firstColumn) {
                        commaSeparatedColumnNames.Append(", ");
                    }
                    commaSeparatedColumnNames.Append(column.ColumnName);
                    firstColumn = false;
                }

                commaSeparatedListOfColumnNamesValue =
                    commaSeparatedColumnNames.ToString();
                return commaSeparatedListOfColumnNamesValue;
            }
        }

        // Declare variables to be reused by the SupplyPageOfData method. 
        private DbDataAdapter adapter = new SQLiteDataAdapter();

        public DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage) {
            // Retrieve the specified number of rows from the database, starting 
            // with the row specified by the lowerPageBoundary parameter.
            command.CommandText = "select " + CommaSeparatedListOfColumnNames + " from " + tableName
                + " limit " + rowsPerPage + ", " + lowerPageBoundary;
            adapter.SelectCommand = command;

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            adapter.Fill(table);
            return table;
        }
    }
}

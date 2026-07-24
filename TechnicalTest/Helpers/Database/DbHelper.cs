using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TechnicalTest.Helpers.Database
{
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["TechnicalTestDB"].ConnectionString; }
        }

        /// <summary>Untuk SELECT yang mengembalikan banyak baris (list, search, dsb).</summary>
        public static DataTable ExecuteDataTable(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                var dataTable = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
                return dataTable;
            }
        }

        /// <summary>Untuk SELECT yang mengembalikan satu baris saja (GetById, Login).</summary>
        public static DataRow ExecuteSingleRow(string storedProcedureName, params SqlParameter[] parameters)
        {
            var table = ExecuteDataTable(storedProcedureName, parameters);
            return table.Rows.Count > 0 ? table.Rows[0] : null;
        }

        /// <summary>Untuk Insert/Update/Delete yang tidak butuh output value.</summary>
        public static int ExecuteNonQuery(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>Untuk SP yang punya parameter OUTPUT (mis. sp_Supplier_Insert -> @NewSupplierID).</summary>
        public static void ExecuteNonQueryWithOutputParams(string storedProcedureName, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>Untuk SP yang mengembalikan satu nilai skalar (mis. IsDuplicate).</summary>
        public static object ExecuteScalar(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }
    }
}

using Npgsql;
using System.Data;

namespace HarnwareGUI.Services
{
    internal class OdooDatabaseService
    {
        private readonly string _connectionString = Globals.ConnectionString;
        private NpgsqlConnection _connection;

        public OdooDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to open a database connection
        public void OpenConnection()
        {
            try
            {
                if (_connection == null)
                {
                    _connection = new NpgsqlConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error opening database connection: " + ex.Message, ex);
            }
        }

        // Method to close a database connection
        public void CloseConnection()
        {
            try
            {
                if (_connection != null && _connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error closing database connection: " + ex.Message, ex);
            }
        }

        // Method to execute a non-query command (e.g., INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error executing non-query: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public Dictionary<string, Dictionary<int, int>> ExecuteQuery(string query)
        {
            Dictionary<string, Dictionary<int, int>> data = new Dictionary<string, Dictionary<int, int>>();

            try
            {
                OpenConnection();
                using (var command = new NpgsqlCommand(query, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string connector = reader.GetString(0);
                        int awg = int.Parse(reader.GetString(1)); // Convert AWG from TEXT to int
                        int count = reader.GetInt32(2);

                        if (!data.ContainsKey(connector))
                        {
                            data[connector] = new Dictionary<int, int>();
                        }

                        data[connector][awg] = count;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error executing query: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }

            return data;
        }

        // Method to execute a query and return a DataTable
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error executing query: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        // Method to execute a scalar query (e.g., SELECT COUNT(*))
        public object ExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error executing scalar query: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SqlDBHelper.SqlDataHelperSource
{
     public class SqlDataBaseHelper
    {
        public SqlDataBaseHelper(string BdLink)
        {
            _bdLink = BdLink;
            Init();
        }

        private SqlConnection _sqlConnection;

        private string _bdLink;

        private void Init()
        {
            try
            {
                _sqlConnection = new SqlConnection(_bdLink);
            }
            catch (Exception)
            {

                throw new Exception("Incorrect database link, check if the database is local ");
            }

            _sqlConnection.Open();
        }

        public void AddData(string NameOfDataTable, string[] ColumnNames ,object[] ValueOfVariables)
        {
            if (ColumnNames.Length != ValueOfVariables.Length || ColumnNames.Length != ValueOfVariables.Length)
                throw new ArgumentOutOfRangeException();

            SqlCommand sqlCommand;

            string columnNames = string.Join(", ", ColumnNames);
            string parameterNames = string.Join(", ", ColumnNames.Select((n, i) => $"@{n}{i}"));

            sqlCommand = new SqlCommand($"INSERT INTO [{NameOfDataTable}] ({columnNames}) VALUES ({parameterNames})", _sqlConnection);

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                sqlCommand.Parameters.AddWithValue($"@{ColumnNames[i]}{i}", ValueOfVariables[i]);
            }

            sqlCommand.ExecuteNonQuery();
        }

        public async void AddDataAsycn(string NameOfDataTable, string[] ColumnNames, object[] ValueOfVariables)
        {
                if (ColumnNames.Length != ValueOfVariables.Length || ColumnNames.Length != ValueOfVariables.Length)
                    throw new ArgumentOutOfRangeException();

                SqlCommand sqlCommand;

            await Task.Run(() =>
            {

                string columnNames = string.Join(", ", ColumnNames);
                string parameterNames = string.Join(", ", ColumnNames.Select((n, i) => $"@{n}{i}"));

                sqlCommand = new SqlCommand($"INSERT INTO [{NameOfDataTable}] ({columnNames}) VALUES ({parameterNames})", _sqlConnection);

                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue($"@{ColumnNames[i]}{i}", ValueOfVariables[i]);
                }

                sqlCommand.ExecuteNonQuery();
            });
        }

        public DataTable GetDataAsDataTable(string NameOfDataTable)
        {
            if (NameOfDataTable == null)
                throw new NullReferenceException();
           
            DataTable result = new DataTable();

            string query = $"SELECT * FROM [{NameOfDataTable}]";

            SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            sqlDataAdapter.Fill(result);                     
            
            return result;
        }
        
        public async Task<DataTable> GetAsyncDataAsDataTable(string NameOfDataTable)
        {
            if (NameOfDataTable == null)
                throw new NullReferenceException();

             DataTable result = new DataTable();

            await Task.Run(() => {

                string query = $"SELECT * FROM [{NameOfDataTable}]";

                SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                sqlDataAdapter.Fill(result);
            });

            return result;
        }

        public List<string[]> GetDataAsList(string NameOfDataTable , params string[] columns)
        {
            if (NameOfDataTable == null)
                throw new NullReferenceException();

            List<string[]> result = new List<string[]>();
            string[] row = null;

            string query = $"SELECT * FROM [{NameOfDataTable}]";
            SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while(reader.Read())
            {
                row = new string[columns.Length];

                for (int i = 0; i < columns.Length; i++)
                {
                    row[i] = reader[columns[i]].ToString();
                }

                result.Add(row);
            }

            result.Add(row);
            return result;
        }

        public async Task<List<string[]>> GetDataAsListAsync(string NameOfDataTable, params string[] columns)
        {
            if (NameOfDataTable == null)
                throw new NullReferenceException();

            List<string[]> result = new List<string[]>();
            string[] row = null;

            await Task.Run(() => 
            {
                string query = $"SELECT * FROM [{NameOfDataTable}]";
                SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    row = new string[columns.Length];

                    for (int i = 0; i < columns.Length; i++)
                    {
                        row[i] = reader[columns[i]].ToString();
                    }

                    result.Add(row);
                }
            });

            return result;
        }

        public List<string[]> FindMatches (string NameOfDataTable, string SearchWord , params string[] columns)
        {
            List<string[]> DataBase = GetDataAsList(NameOfDataTable, columns);
            List<string[]> result = DataBase.Where((x) =>
            {
                return x[0].ToLower().Contains(SearchWord.ToLower());
            }).ToList();

            return result;
        }

        public async Task<List<string[]>> FindMatchesAsync(string NameOfDataTable, string SearchWord, params string[] columns)
        {
            List<string[]> result = null;

            await Task.Run(() => 
            {
                List<string[]> DataBase = GetDataAsListAsync(NameOfDataTable, columns).Result;
                result = DataBase.Where((x) =>
                {
                    return x[0].ToLower().Contains(SearchWord.ToLower());
                }).ToList();

            });

            return result;
        }
    }
}




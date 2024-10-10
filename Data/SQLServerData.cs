using Microsoft.Data.SqlClient;
using System.Data;

namespace C_Sharp_Worker_Service.Data
{
	internal abstract class SQLServerData
	{
		public async Task<SqlDataReader> ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			SqlDataReader result = null;

			SqlConnection sqlConnection = new SqlConnection();
			
			sqlConnection.ConnectionString = connectionString;

			await sqlConnection.OpenAsync();

			SqlCommand sqlCommand = new SqlCommand();
			
			sqlCommand.Connection = sqlConnection;
			sqlCommand.CommandType = commandType;
			sqlCommand.CommandText = commandText;

			result = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

			return result;
		}

		public async Task ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			using (SqlConnection sqlConnection = new SqlConnection())
			{
				sqlConnection.ConnectionString = connectionString;

				await sqlConnection.OpenAsync();

				using (SqlCommand sqlCommand = new SqlCommand())
				{
					sqlCommand.Connection = sqlConnection;
					sqlCommand.CommandType = commandType;
					sqlCommand.CommandText = commandText;

					await sqlCommand.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
using C_Sharp_Worker_Service.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace C_Sharp_Worker_Service.Data
{
	internal class IndexData : SQLServerData
	{

		public async Task<List<IndexModel>> GetIndices(string connectionString)
		{
			List<IndexModel> result = new List<IndexModel>();

			string commandText = @"
SELECT	DISTINCT
        SCHEMA_NAME(o.schema_id) AS SchemaName               
        ,OBJECT_NAME(o.object_id) AS TableName
        ,i.[name] AS IndexName
        ,CASE WHEN ISNULL(ps.function_id,1) = 1 THEN 'NO' ELSE 'YES' END AS IsPartitioned
        ,p.partition_number AS PartitionNumber
        ,dmv.Avg_Fragmentation_In_Percent AS AverageFragmentationInPercent
FROM sys.partitions AS p WITH (NOLOCK)
INNER JOIN sys.indexes AS i WITH (NOLOCK)
        ON i.object_id = p.object_id
        AND i.index_id = p.index_id
INNER JOIN sys.objects AS o WITH (NOLOCK)
        ON o.object_id = i.object_id
INNER JOIN sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL , NULL, N'LIMITED') dmv
        ON dmv.OBJECT_ID = i.object_id
        AND dmv.index_id = i.index_id
        AND dmv.partition_number  = p.partition_number
LEFT JOIN sys.data_spaces AS ds WITH (NOLOCK)
      ON ds.data_space_id = i.data_space_id
LEFT JOIN sys.partition_schemes AS ps WITH (NOLOCK)
      ON ps.data_space_id = ds.data_space_id
WHERE
      OBJECTPROPERTY(p.object_id, 'ISMSShipped') = 0;";

			using (SqlDataReader sqlDataReader = await base.ExecuteReader(connectionString, CommandType.Text, commandText))
			{
				while (sqlDataReader.Read())
				{
					IndexModel index = new IndexModel()
					{
						SchemaName = Convert.ToString(sqlDataReader["SchemaName"]),
						TableName = Convert.ToString(sqlDataReader["TableName"]),
						IndexName = Convert.ToString(sqlDataReader["IndexName"]),
						IsPartitioned = string.Compare(Convert.ToString(sqlDataReader["IsPartitioned"]), "YES") == 0,
						PartitionNumber = Convert.ToInt32(sqlDataReader["PartitionNumber"]),
						AverageFragmentationInPercent = Convert.ToDouble(sqlDataReader["AverageFragmentationInPercent"])
					};

					result.Add(index);
				}
			}

			return result;
		}

		public async Task ProcessIndex(string connectionString, IndexModel model, bool rebuild)
		{
			string commandText = string.Empty;

			if (model.IsPartitioned)
			{
				commandText = @$"
ALTER INDEX [{model.IndexName}] ON [{model.SchemaName}].[{model.TableName}]
{(rebuild ? "REBUILD" : "REORGANIZE")} PARTITION = {model.PartitionNumber} WITH ( ONLINE = ON )";
			}
			else
			{
				commandText = @$"
ALTER INDEX [{model.IndexName}] ON [{model.SchemaName}].[{model.TableName}]
{(rebuild ? "REBUILD" : "REORGANIZE")} WITH ( ONLINE = ON )";
			}

			try
			{
				await base.ExecuteNonQuery(connectionString, CommandType.Text, commandText);
			}
			catch
			{
				if (model.IsPartitioned)
				{
					commandText = @$"
ALTER INDEX [{model.IndexName}] ON [{model.SchemaName}].[{model.TableName}]
{(rebuild ? "REBUILD" : "REORGANIZE")} PARTITION = {model.PartitionNumber}";
				}
				else
				{
					commandText = @$"
ALTER INDEX [{model.IndexName}] ON [{model.SchemaName}].[{model.TableName}]
{(rebuild ? "REBUILD" : "REORGANIZE")}";
				}

				await base.ExecuteNonQuery(connectionString, CommandType.Text, commandText);
			}
		}
	}
}
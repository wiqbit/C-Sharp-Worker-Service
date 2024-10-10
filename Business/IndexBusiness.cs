using C_Sharp_Worker_Service.Data;
using C_Sharp_Worker_Service.Model;

namespace C_Sharp_Worker_Service.Business
{
	internal class IndexBusiness
	{
		public async Task DoWork(IConfiguration configuration)
		{
			IndexData indexData = new IndexData();

			string connectionStringKeys = configuration["IndexWorkerConnectionStringKeys"];

			foreach (string connectionStringKey in connectionStringKeys.Split(","))
			{
				string connectionString = configuration.GetConnectionString(connectionStringKey);

				List<IndexModel> indices = await indexData.GetIndices(connectionString);

				IndexModel.Sort sort = new IndexModel.Sort();

				indices.Sort(sort.Compare);

				foreach (IndexModel index in indices)
				{
					if (index.AverageFragmentationInPercent >= 30D)
					{
						await indexData.ProcessIndex(connectionString, index, true);
					}
					else if (index.AverageFragmentationInPercent >= 10D)
					{
						await indexData.ProcessIndex(connectionString, index, false);
					}
				}
			}
		}
	}
}
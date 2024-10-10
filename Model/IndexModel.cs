namespace C_Sharp_Worker_Service.Model
{
	internal class IndexModel
	{
		internal class Sort : IComparer<IndexModel>
		{
			public int Compare(IndexModel x, IndexModel y)
			{
				int result = 0;

				if (x.AverageFragmentationInPercent > y.AverageFragmentationInPercent)
				{
					result = -1;
				}
				else if (x.AverageFragmentationInPercent < y.AverageFragmentationInPercent)
				{
					result = 1;
				}

				return result;
			}
		}

		public string SchemaName { get; set; }
		public string TableName { get; set; }
		public string IndexName { get; set; }
		public bool IsPartitioned { get; set; }
		public int PartitionNumber { get; set; }
		public double AverageFragmentationInPercent { get; set; }
	}
}
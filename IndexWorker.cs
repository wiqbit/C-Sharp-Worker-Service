using C_Sharp_Worker_Service.Business;

namespace C_Sharp_Worker_Service
{
	public class IndexWorker : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<IndexWorker> _logger;

		public IndexWorker(IConfiguration configuration, ILogger<IndexWorker> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				IndexBusiness indexBusiness = new IndexBusiness();

				await indexBusiness.DoWork(_configuration);

				if (_logger.IsEnabled(LogLevel.Information))
				{
					_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				}

				await Task.Delay(60 * 60 * 1000, stoppingToken);
			}
		}
	}
}
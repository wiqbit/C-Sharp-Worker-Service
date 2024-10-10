using C_Sharp_Worker_Service;

HostApplicationBuilder hostApplicationBuilder = Host.CreateApplicationBuilder(args);

hostApplicationBuilder.Services.AddHostedService<IndexWorker>();

IHost host = hostApplicationBuilder.Build();

host.Run();
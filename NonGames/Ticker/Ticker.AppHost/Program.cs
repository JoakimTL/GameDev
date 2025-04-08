var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Ticker_ApiService>("apiservice");

builder.AddProject<Projects.Ticker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

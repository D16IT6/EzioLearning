var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder
    .AddProject<Projects.EzioLearning_Api>("eziolearning-api", "https-no-browser");

builder
    .AddProject<Projects.EzioLearning_Wasm>("eziolearning-wasm", "https-no-browser")
    .WithReference(apiService);

builder.Build().Run();

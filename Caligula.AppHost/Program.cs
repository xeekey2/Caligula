var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Caligula_ApiService>("apiservice");

builder.AddProject<Projects.Caligula_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

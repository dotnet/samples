var builder = DistributedApplication.CreateBuilder(args);

// https://learn.microsoft.com/en-us/dotnet/aspire/frameworks/orleans?tabs=dotnet-cli
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();
var clusteringTable = storage.AddTables("clustering");
var orleans = builder.AddOrleans("default")
    .WithClustering(clusteringTable);

var service = builder.AddProject<Projects.GPSTracker_Service>("gpstracker-service")
    .WithReference(orleans)
    .WithReplicas(3);

var deviceGateway = builder.AddProject<Projects.GPSTracker_FakeDeviceGateway>("device-gateway")
    .WithReference(orleans.AsClient())
    .WithExternalHttpEndpoints()
    .WaitFor(service);

builder.Build().Run();

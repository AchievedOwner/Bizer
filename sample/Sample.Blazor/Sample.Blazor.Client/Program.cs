using Bizer;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Sample.Contracts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBizer(options => options.AddAssmebly(typeof(ITestService).Assembly))
    .AddDynamicHttpProxy("http://localhost:5216/");
await builder.Build().RunAsync();

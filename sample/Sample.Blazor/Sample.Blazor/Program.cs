using Microsoft.AspNetCore.Components.Authorization;

using Sample.Blazor.Client;
using Sample.Blazor.Client.Pages;
using Sample.Blazor.Components;
using Sample.Contracts;
using Sample.Contracts.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddTransient<ITestService, TestSerivce>();

builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddScoped<AuthenticationStateProvider, SampleAuthenticationStateProvider>();
builder.Services.AddScoped<SampleAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //  app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Sample.Blazor.Client._Imports).Assembly);

app.Run();

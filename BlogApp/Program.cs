using BlogApp.Components;
using BlogApp.Data;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// API endpoint for command execution
app.MapPost("/api/command", async (CommandRequest request) =>
{
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        var psi = new ProcessStartInfo
        {
            FileName = request.Command,
            Arguments = request.Arguments ?? string.Empty,
            WorkingDirectory = request.WorkingDirectory ?? Directory.GetCurrentDirectory(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(request.TimeoutMs))
        {
            process.Kill();
            stopwatch.Stop();
            return Results.BadRequest(new CommandResult
            {
                Command = request.Command,
                ExitCode = -1,
                Output = string.Empty,
                ErrorOutput = "Process execution timeout.",
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            });
        }

        await Task.WhenAll(outputTask, errorTask);
        stopwatch.Stop();

        return Results.Ok(new CommandResult
        {
            Command = request.Command,
            ExitCode = process.ExitCode,
            Output = outputTask.Result,
            ErrorOutput = errorTask.Result,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds
        });
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        return Results.BadRequest(new CommandResult
        {
            Command = request.Command,
            ExitCode = -1,
            Output = string.Empty,
            ErrorOutput = ex.Message,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds
        });
    }
});

app.Run();

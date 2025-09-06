using BetGame.Interfaces;
using BetGame.Abstractions;
using BetGame.Models;
using BetGame.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BetGame;

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConsoleWrapper, ConsoleWrapper>()
            .AddScoped<IBetEngine, BetEngineService>()
            .AddScoped<WalletService>()
            .AddScoped<GameRunnerService>()
            .AddScoped<IRandomGenerator, RandomGenerator>()
            .AddScoped<IWallet, Wallet>(_ => new Wallet(0));

        services.Configure<BetOptions>(configuration.GetSection(BetOptions.SectionName));

        var provider = services.BuildServiceProvider();

        var runner = provider.GetRequiredService<GameRunnerService>();
        await runner.RunAsync();
    }
}

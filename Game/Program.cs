using BetGame.Interfaces;
using BetGame.Abstractions;
using BetGame.Models;
using BetGame.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BetGame;

internal class Program
{
    static Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConsoleWrapper, ConsoleWrapper>()
            .AddScoped<IBetEngine, BetEngineService>()
            .AddScoped<WalletService>()
            .AddScoped<GameRunnerService>()
            .AddScoped<IRandomGenerator, RandomGenerator>()
            .AddScoped<IWallet, Wallet>();

        services.Configure<BetOptions>(configuration.GetSection(BetOptions.SectionName));

        var provider = services.BuildServiceProvider();

        var runner = provider.GetRequiredService<GameRunnerService>();
        runner.Run();
        return Task.CompletedTask;
    }
}

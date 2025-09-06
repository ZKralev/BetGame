using BetGame.Abstractions;
using BetGame.Interfaces;
using BetGame.Models;

namespace BetGame.Services;

public class GameRunnerService(WalletService walletService, IConsoleWrapper console, IBetEngine betEngine) : IGameRunner
{
    private readonly WalletService _walletService = walletService;
    private readonly IConsoleWrapper _console = console;
    private readonly IBetEngine _betEngine = betEngine;

    public async Task RunAsync()
    {
        string action;
        MenuOption menuOption;

        do
        {
            _console.SetTitle("Bet Game");
            _console.WriteLine("");
            _console.WriteLine("=== Bet Game ===");
            _console.WriteLine("1) Deposit");
            _console.WriteLine("2) Withdraw");
            _console.WriteLine("3) Place Bet");
            _console.WriteLine("0) Exit");
            _console.WriteLine("Please submit your action: ");
            action = (_console.ReadLine() ?? "0").Trim();

            if (!Enum.TryParse<MenuOption>(action, out menuOption))
            {
                _console.SetForegroundColor(ConsoleColor.Red);
                _console.WriteLine("Invalid choice");
                _console.ResetColor();
                continue;
            }

            try
            {
                switch (menuOption)
                {
                    case MenuOption.Deposit:
                        await Task.Run(() => _walletService.Deposit());
                        break;
                    case MenuOption.Withdraw:
                        await Task.Run(() => _walletService.Withdraw());
                        break;
                    case MenuOption.PlaceBet:
                        _console.WriteLine($"Minimum bet: 1");
                        _console.WriteLine($"Maximum bet: 10");
                        _console.WriteLine("Enter your bet amount: ");

                        double stake = _console.ReadLine() is string input && double.TryParse(input, out double parsedStake) ? parsedStake : 0;

                        BetResult result = await Task.Run(() => _betEngine.PlayRound(stake));

                        _walletService.UpdateBalance(stake, result.WinAmount ?? 0);

                        _console.SetForegroundColor(result.Color);
                        _console.WriteLine($"{result.Message}{_walletService.Balance:F2}");
                        _console.ResetColor();
                        break;
                    case MenuOption.Exit:
                        _console.WriteLine("Thank you for playing!");
                        break;
                }
            }
            catch (Exception ex)
            {
                _console.SetForegroundColor(ConsoleColor.Red);
                _console.WriteLine($"Error: {ex.Message}");
                _console.ResetColor();
            }

        } while (menuOption != MenuOption.Exit);
    }
}
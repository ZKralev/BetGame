using BetGame.Abstractions;
using BetGame.Interfaces;
using BetGame.Models;

namespace BetGame.Services;

public class GameRunnerService(WalletService walletService, IConsoleWrapper console, IBetEngine betEngine) : IGameRunner
{
    public void Run()
    {
        string action;
        MenuOption menuOption;

        do
        {
            console.SetTitle("Bet Game");
            console.WriteLine("");
            console.WriteLine("=== Bet Game ===");
            console.WriteLine("1) Deposit");
            console.WriteLine("2) Withdraw");
            console.WriteLine("3) Place Bet");
            console.WriteLine("0) Exit");
            console.WriteLine("Please submit your action: ");
            action = (console.ReadLine() ?? "0").Trim();

            if (!Enum.TryParse<MenuOption>(action, out menuOption))
            {
                console.SetForegroundColor(console.GetColor("Red"));
                console.WriteLine("Invalid choice");
                console.ResetColor();
                continue;
            }

            try
            {
                switch (menuOption)
                {
                    case MenuOption.Deposit:
                        walletService.Deposit();
                        break;
                    case MenuOption.Withdraw:
                        walletService.Withdraw();
                        break;
                    case MenuOption.PlaceBet:
                        console.WriteLine($"Minimum bet: 1");
                        console.WriteLine($"Maximum bet: 10");
                        console.WriteLine("Enter your bet amount: ");

                        double stake = console.ReadLine() is string input && double.TryParse(input, out double parsedStake) ? parsedStake : 0;

                        BetResult result = betEngine.PlayRound(stake);

                        walletService.UpdateBalance(stake, result.WinAmount ?? 0);

                        console.SetForegroundColor(result.Color);
                        console.WriteLine($"{result.Message}{walletService.Balance:F2}");
                        console.ResetColor();
                        break;
                    case MenuOption.Exit:
                        console.WriteLine("Thank you for playing!");
                        break;
                }
            }
            catch (Exception ex)
            {
                console.SetForegroundColor(console.GetColor("Red"));
                console.WriteLine($"Error: {ex.Message}");
                console.ResetColor();
            }

        } while (menuOption != MenuOption.Exit);
    }
}
using BetGame.Interfaces;

namespace BetGame.Services;

public class WalletService(IWallet wallet, IConsoleWrapper console)
{
    private readonly IWallet _wallet = wallet;
    private readonly IConsoleWrapper _console = console;
    public double Balance => _wallet.Balance;

    public void Deposit()
    {
        _console.WriteLine("Enter amount to deposit: ");
        string? amount = _console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amount) && double.TryParse(amount, out double depositAmount))
        {
            _wallet.Deposit(depositAmount);
            _console.SetForegroundColor(ConsoleColor.Green);
            _console.WriteLine($"Deposited ${depositAmount}, your new balance: ${_wallet.Balance:F2}");
            _console.ResetColor();
        }
        else
        {
            _console.SetForegroundColor(ConsoleColor.Red);
            _console.WriteLine("Invalid amount.");
            _console.ResetColor();
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
        }
    }

    public void Withdraw()
    {
        _console.WriteLine("Enter amount to withdraw: ");
        string? amount = _console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amount) && double.TryParse(amount, out double withdrawAmount))
        {
            if (withdrawAmount > _wallet.Balance)
            {
                _console.SetForegroundColor(ConsoleColor.Red);
                _console.WriteLine("Insufficient funds.");
                _console.ResetColor();
                throw new InvalidOperationException("Insufficient funds for this withdrawal.");
            }

            _wallet.Withdraw(withdrawAmount);
            _console.SetForegroundColor(ConsoleColor.Yellow);
            _console.WriteLine($"Withdraw ${withdrawAmount}, your new balance: ${_wallet.Balance:F2}");
            _console.ResetColor();
        }
        else
        {
            _console.SetForegroundColor(ConsoleColor.Red);
            _console.WriteLine("Invalid amount.");
            _console.ResetColor();
            throw new ArgumentException("Withdraw amount must be positive.", nameof(amount));
        }
    }

    public double GetBalance() => _wallet.Balance;
    public double SetBalance(double newBalance)
    {
        _wallet.SetBalance(newBalance);
        return _wallet.Balance;
    }

    public void UpdateBalance(double stake, double amount)
    {
        var newBalance = _wallet.Balance - stake + amount;
        _wallet.SetBalance(newBalance);
    }
}
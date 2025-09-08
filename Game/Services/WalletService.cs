using BetGame.Interfaces;

namespace BetGame.Services;

public class WalletService(IWallet wallet, IConsoleWrapper console)
{
    public double Balance => wallet.Balance;

    public void Deposit()
    {
        console.WriteLine("Enter amount to deposit: ");
        string? amount = console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amount) && double.TryParse(amount, out double depositAmount))
        {
            wallet.Deposit(depositAmount);
            console.SetForegroundColor(console.GetColor("Green"));
            console.WriteLine($"Deposited ${depositAmount}, your new balance: ${wallet.Balance:F2}");
            console.ResetColor();
        }
        else
        {
            console.SetForegroundColor(console.GetColor("Red"));
            console.WriteLine("Invalid amount.");
            console.ResetColor();
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
        }
    }

    public void Withdraw()
    {
        console.WriteLine("Enter amount to withdraw: ");
        string? amount = console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amount) && double.TryParse(amount, out double withdrawAmount))
        {
            if (withdrawAmount > wallet.Balance)
            {
                console.SetForegroundColor(console.GetColor("Red"));
                console.WriteLine("Insufficient funds.");
                console.ResetColor();
                throw new InvalidOperationException("Insufficient funds for this withdrawal.");
            }

            wallet.Withdraw(withdrawAmount);
            console.SetForegroundColor(console.GetColor("Yellow"));
            console.WriteLine($"Withdraw ${withdrawAmount}, your new balance: ${wallet.Balance:F2}");
            console.ResetColor();
        }
        else
        {
            console.SetForegroundColor(console.GetColor("Red"));
            console.WriteLine("Invalid amount.");
            console.ResetColor();
            throw new ArgumentException("Withdraw amount must be positive.", nameof(amount));
        }
    }

    public double GetBalance() => wallet.Balance;
    public double SetBalance(double newBalance)
    {
        wallet.SetBalance(newBalance);
        return wallet.Balance;
    }

    public void UpdateBalance(double stake, double amount)
    {
        var newBalance = wallet.Balance - stake + amount;
        wallet.SetBalance(newBalance);
    }
}
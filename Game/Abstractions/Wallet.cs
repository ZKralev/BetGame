using BetGame.Interfaces;

namespace BetGame.Abstractions
{
    public class Wallet(double initialBalance) : IWallet
    {
        public double Balance { get; private set; } = initialBalance >= 0 ? initialBalance : 0;

        public void Deposit(double amount)
        {
            if (amount > 0)
            {
                Balance += amount;
            }
        }

        public void Withdraw(double amount)
        {
            if (amount > 0 && amount <= Balance)
            {
                Balance -= amount;
            }
        }

        public void SetBalance(double newBalance)
        {
            Balance = newBalance;
        }
    }
}
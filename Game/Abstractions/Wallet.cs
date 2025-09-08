using BetGame.Interfaces;

namespace BetGame.Abstractions
{
    public class Wallet : IWallet
    {
        public double Balance { get; private set; } = 0;

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
            Balance = newBalance < 0 ? 0 : newBalance;
        }
    }
}
namespace BetGame.Interfaces
{
    public interface IWallet
    {
        void Deposit(double amount);
        void Withdraw(double amount); 
        double Balance { get; }
        void SetBalance(double newBalance);
    }
}

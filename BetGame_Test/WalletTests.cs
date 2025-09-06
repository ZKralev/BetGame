using BetGame.Abstractions;

namespace BetGame.Tests
{
    [TestClass]
    public class WalletTests
    {
        [TestMethod]
        public void Wallet_InitialBalance_IsSetCorrectly()
        {
            var wallet = new Wallet(100.0);
            Assert.AreEqual(100.0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_InitialBalance_NegativeValue_DefaultsToZero()
        {
            var wallet = new Wallet(-50.0);
            Assert.AreEqual(0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_Deposit_PositiveAmount_IncreasesBalance()
        {
            var wallet = new Wallet(10.0);
            wallet.Deposit(20.0);
            Assert.AreEqual(30.0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_Deposit_NegativeAmount_DoesNotChangeBalance()
        {
            var wallet = new Wallet(10.0);
            wallet.Deposit(-5.0);
            Assert.AreEqual(10.0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_Withdraw_ValidAmount_DecreasesBalance()
        {
            var wallet = new Wallet(50.0);
            wallet.Withdraw(20.0);
            Assert.AreEqual(30.0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_Withdraw_AmountExceedsBalance_DoesNotChangeBalance()
        {
            var wallet = new Wallet(10.0);
            wallet.Withdraw(20.0);
            Assert.AreEqual(10.0, wallet.Balance);
        }

        [TestMethod]
        public void Wallet_SetBalance_UpdatesBalance()
        {
            var wallet = new Wallet(10.0);
            wallet.SetBalance(99.0);
            Assert.AreEqual(99.0, wallet.Balance);
        }
    }
}   
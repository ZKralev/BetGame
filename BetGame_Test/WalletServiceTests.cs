using BetGame.Interfaces;
using BetGame.Services;
using Moq;

namespace BetGame.Tests
{
    [TestClass]
    public class WalletServiceTests
    {
        private Mock<IWallet> _wallet = null!;
        private Mock<IConsoleWrapper> _console = null!;
        private WalletService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _wallet = new Mock<IWallet>();
            _console = new Mock<IConsoleWrapper>();
            _service = new WalletService(_wallet.Object, _console.Object);
        }

        [TestMethod]
        public void Deposit_ValidAmount_CallsWalletDeposit_AndPrintsGreen()
        {
            _console.SetupSequence(c => c.ReadLine()).Returns("100");
            _wallet.SetupGet(w => w.Balance).Returns(100);

            _service.Deposit();

            _wallet.Verify(w => w.Deposit(100), Times.Once);
            _console.Verify(c => c.SetForegroundColor(ConsoleColor.Green), Times.Once);
            _console.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("Deposited $100"))), Times.Once);
            _console.Verify(c => c.ResetColor(), Times.Once);
        }

        [TestMethod]
        public void Deposit_InvalidAmount_Throws_AndPrintsRed()
        {
            _console.SetupSequence(c => c.ReadLine()).Returns("abc");

            var ex = Assert.ThrowsException<ArgumentException>(() => _service.Deposit());
            StringAssert.Contains(ex.Message, "Deposit amount must be positive.");

            _console.Verify(c => c.SetForegroundColor(ConsoleColor.Red), Times.Once);
            _console.Verify(c => c.WriteLine("Invalid amount."), Times.Once);
            _console.Verify(c => c.ResetColor(), Times.Once);
        }

        [TestMethod]
        public void Withdraw_ValidAmount_CallsWalletWithdraw_AndPrintsYellow()
        {
            _console.SetupSequence(c => c.ReadLine()).Returns("50");
            _wallet.SetupGet(w => w.Balance).Returns(200);

            _service.Withdraw();

            _wallet.Verify(w => w.Withdraw(50), Times.Once);
            _console.Verify(c => c.SetForegroundColor(ConsoleColor.Yellow), Times.Once);
            _console.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("Withdraw $50"))), Times.Once);
            _console.Verify(c => c.ResetColor(), Times.Once);
        }

        [TestMethod]
        public void Withdraw_InsufficientFunds_Throws_AndPrintsRed()
        {
            _console.SetupSequence(c => c.ReadLine()).Returns("50");
            _wallet.SetupGet(w => w.Balance).Returns(40);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => _service.Withdraw());
            StringAssert.Contains(ex.Message, "Insufficient funds");

            _console.Verify(c => c.SetForegroundColor(ConsoleColor.Red), Times.Once);
            _console.Verify(c => c.WriteLine("Insufficient funds."), Times.Once);
            _console.Verify(c => c.ResetColor(), Times.Once);
            _wallet.Verify(w => w.Withdraw(It.IsAny<double>()), Times.Never);
        }

        [TestMethod]
        public void Withdraw_InvalidAmount_Throws_AndPrintsRed()
        {
            _console.SetupSequence(c => c.ReadLine()).Returns("oops");

            var ex = Assert.ThrowsException<ArgumentException>(() => _service.Withdraw());
            StringAssert.Contains(ex.Message, "Withdraw amount must be positive.");

            _console.Verify(c => c.SetForegroundColor(ConsoleColor.Red), Times.Once);
            _console.Verify(c => c.WriteLine("Invalid amount."), Times.Once);
            _console.Verify(c => c.ResetColor(), Times.Once);
        }

        [TestMethod]
        public void UpdateBalance_ComputesNewBalance_AndSets()
        {
            _wallet.SetupGet(w => w.Balance).Returns(100);

            _service.UpdateBalance(5, 10);

            _wallet.Verify(w => w.SetBalance(105), Times.Once);
        }

        [TestMethod]
        public void GetBalance_ReturnsUnderlyingBalance()
        {
            _wallet.SetupGet(w => w.Balance).Returns(123.45);
            var bal = _service.GetBalance();
            Assert.AreEqual(123.45, bal, 1e-9);
        }

        [TestMethod]
        public void SetBalance_SetsAndReturnsNewBalance_UsingCallback()
        {
            double current = 0;
            _wallet.SetupGet(w => w.Balance).Returns(() => current);
            _wallet.Setup(w => w.SetBalance(It.IsAny<double>()))
                   .Callback<double>(v => current = v);

            var returned = _service.SetBalance(77.7);

            Assert.AreEqual(77.7, current, 1e-9);
            Assert.AreEqual(77.7, returned, 1e-9);
            _wallet.Verify(w => w.SetBalance(77.7), Times.Once);
        }
    }
}
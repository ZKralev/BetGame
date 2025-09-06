using Moq;
using BetGame.Services;
using BetGame.Interfaces;
using BetGame.Abstractions;
using BetGame.Models;

namespace BetGame.Tests
{
    [TestClass]
    public class GameRunnerServiceTests
    {        
        private Mock<IWallet> _walletCoreMock = null!;
        private Mock<WalletService> _walletMock = null!;
        private Mock<IConsoleWrapper> _consoleMock = null!;
        private Mock<IBetEngine> _betEngineMock = null!;
        private GameRunnerService _gameRunner = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create mocks for the dependencies
            _walletCoreMock = new Mock<IWallet>();
            _consoleMock = new Mock<IConsoleWrapper>();
            _betEngineMock = new Mock<IBetEngine>();

            // Create the WalletService mock with the required dependencies
            _walletMock = new Mock<WalletService>(_walletCoreMock.Object, _consoleMock.Object);
            
            _gameRunner = new GameRunnerService(_walletMock.Object, _consoleMock.Object, _betEngineMock.Object);
        }

        [TestMethod]
        public async Task RunAsync_ExitImmediately_PrintsThankYou()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("0");
            await _gameRunner.RunAsync();
            _consoleMock.Verify(c => c.WriteLine("Thank you for playing!"), Times.Once);
        }

        [TestMethod]
        public async Task RunAsync_InvalidMenuOption_PrintsInvalidChoice()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("invalid")
                .Returns("0");
            await _gameRunner.RunAsync();
            _consoleMock.Verify(c => c.WriteLine("Invalid choice"), Times.Once);
        }

        [TestMethod]
        public async Task RunAsync_Deposit_CallsWalletDeposit()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("1")    
                .Returns("100")  
                .Returns("0");   
            
            await _gameRunner.RunAsync();
            
            _walletCoreMock.Verify(w => w.Deposit(100), Times.Once);
            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("Deposited $100"))), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task RunAsync_Withdraw_CallsWalletWithdraw()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("2")   
                .Returns("50")  
                .Returns("0");  
            _walletCoreMock.SetupGet(w => w.Balance).Returns(100);
            _walletCoreMock.Setup(w => w.Withdraw(It.IsAny<double>()));
            await _gameRunner.RunAsync();
            _walletCoreMock.Verify(w => w.Withdraw(50), Times.Once);
        }

        [TestMethod]
        public async Task RunAsync_PlaceBet_ValidBet_UpdatesBalanceAndPrintsMessage()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("3") 
                .Returns("5") 
                .Returns("0"); 


            var betResult = new BetResult(0, "You win!", 10, ConsoleColor.Green);
            _betEngineMock.Setup(b => b.PlayRound(5)).Returns(betResult);
            _walletCoreMock.SetupGet(w => w.Balance).Returns(100);

            await _gameRunner.RunAsync();

            _betEngineMock.Verify(b => b.PlayRound(5), Times.Once);
            _walletCoreMock.Verify(w => w.SetBalance(105), Times.Once);
            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("You win!"))), Times.Once);
        }

        [TestMethod]
        public async Task RunAsync_PlaceBet_InvalidBet_PrintsError()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("3") 
                .Returns("notanumber")
                .Returns("0"); 

            var betResult = new BetResult(0, "No win", 0, ConsoleColor.Red);
            _betEngineMock.Setup(b => b.PlayRound(0)).Returns(betResult);
            _walletCoreMock.SetupGet(w => w.Balance).Returns(50);

            await _gameRunner.RunAsync();

            _betEngineMock.Verify(b => b.PlayRound(0), Times.Once);
            _walletCoreMock.Verify(w => w.SetBalance(50), Times.Once);
            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("No win"))), Times.Once);
        }

        [TestMethod]
        public async Task RunAsync_ExceptionDuringAction_PrintsError()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("1")   
                .Returns("100") 
                .Returns("0");   
            _walletCoreMock.Setup(w => w.Deposit(100)).Throws(new Exception("Deposit failed"));

            await _gameRunner.RunAsync();

            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("Error: Deposit failed"))), Times.Once);
        }
    }
}
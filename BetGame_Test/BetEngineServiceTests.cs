using BetGame.Services;
using BetGame.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using BetGame.Models;

namespace BetGame.Tests
{
    [TestClass]
    public class BetEngineServiceTests
    {
        private BetOptions _betOptions = null!;
        private Mock<IRandomGenerator> _randomMock = null!;
        private Mock<IConsoleWrapper> _consoleMock = null!;
        private BetEngineService _betEngine = null!;
        private IOptions<BetOptions> _options = null!;

        [TestInitialize]
        public void Setup()
        {
            _betOptions = new BetOptions
            {
                SmallestBet = 1,
                BiggestBet = 100,
                MinimalRoll = 0.0,
                MaxRoll = 1.0,
                SmallWin = new SmallWinBetOutcome { WinRatioStart = 1.1, WinRatioEnd = 1.5 },
                BigWin = new BigWinBetOutcome { WinRatioStart = 2.0, WinRatioEnd = 3.0 }
            };
            _randomMock = new Mock<IRandomGenerator>();
            _consoleMock = new Mock<IConsoleWrapper>();
            _options = Options.Create(_betOptions);
            _betEngine = new BetEngineService(
                _options,
                _randomMock.Object,
                _consoleMock.Object
            );
        }

        [TestMethod]
        public void PlayRound_Loss_ReturnsZeroWin()
        {
            _randomMock.Setup(r => r.NextDouble()).Returns(0.4);
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("10"); 

            var result = _betEngine.PlayRound(10);

            Assert.AreEqual(0, result.WinAmount);
            Assert.AreEqual("No luck this time! Your current balance is: $", result.Message);
        }

        [TestMethod]
        public void PlayRound_SmallWin_ReturnsWinAmount()
        {
            _randomMock.SetupSequence(r => r.NextDouble())
                .Returns(0.7) 
                .Returns(0.5); 
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("10"); 

            var result = _betEngine.PlayRound(10);

            Assert.IsTrue(result.WinAmount > 0);
            StringAssert.Contains(result.Message, "Congratulations! You win");
        }

        [TestMethod]
        public void PlayRound_BigWin_ReturnsBigWinAmount()
        {
            _randomMock.SetupSequence(r => r.NextDouble())
                .Returns(0.95) 
                .Returns(0.5); 
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("10"); 

            var result = _betEngine.PlayRound(10);

            Assert.IsTrue(result.WinAmount > 0);
            StringAssert.Contains(result.Message, "JACKPOT! You win");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PlayRound_InvalidBet_ThrowsException()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("0"); 

            _betEngine.PlayRound(0);
        }

        [TestMethod]
        public void PlayRound_NoWin_WhenRollIsAtOrBelowHalf()
        {
            // roll => 0.4 (<= 0.5) => no win path
            _randomMock.SetupSequence(r => r.NextDouble())
                .Returns(0.4);

            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);

            var result = engine.PlayRound(5);

            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("You rolled:"))), Times.Once);
            Assert.AreEqual(0, result.WinAmount);
            StringAssert.Contains(result.Message, "No luck this time!");
            Assert.AreEqual(ConsoleColor.Red, result.Color);
        }

        [TestMethod]
        public void PlayRound_SmallWin_WhenRollBetweenHalfAndPointNine()
        {
            // roll => 0.7 => small win; jackpot RNG => 0.0 => multiplier = 1.1 + 0.4*0 = 1.1
            _randomMock.SetupSequence(r => r.NextDouble())
                .Returns(0.7) // roll
                .Returns(0.0); // jackpot multiplier

            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);

            var result = engine.PlayRound(5);

            Assert.AreEqual(5 * 1.1, result.WinAmount);
            StringAssert.Contains(result.Message, "Congratulations! You win $5.50!");
            Assert.AreEqual(ConsoleColor.Green, result.Color);
        }

        [TestMethod]
        public void PlayRound_BigWin_WhenRollAbovePointNine()
        {
            // roll => 0.95 => big win; jackpot RNG => 0.0 => multiplier = 2.0
            _randomMock.SetupSequence(r => r.NextDouble())
                .Returns(0.95) // roll
                .Returns(0.0); // jackpot multiplier

            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);

            var result = engine.PlayRound(5);

            Assert.AreEqual(10.0, result.WinAmount);
            StringAssert.Contains(result.Message, "JACKPOT! You win $10.00!");
            Assert.AreEqual(ConsoleColor.Green, result.Color);
        }

        [TestMethod]
        public void PlayRound_Throws_WhenStakeBelowMinimum()
        {
            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => engine.PlayRound(0.5));
        }

        [TestMethod]
        public void PlayRound_Throws_WhenStakeAboveMaximum()
        {
            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => engine.PlayRound(120));
        }

        [TestMethod]
        public void Ctor_Throws_WhenOptionsIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BetEngineService(null!, _randomMock.Object, _consoleMock.Object));
        }

        [TestMethod]
        public void GetBetOptions_ReturnsConfiguredOptions()
        {
            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);
            Assert.AreSame(_betOptions, engine.GetBetOptions());
        }

        [TestMethod]
        public void GetRandomGenerator_ReturnsInjectedInstance()
        {
            var engine = new BetEngineService(_options, _randomMock.Object, _consoleMock.Object);
            Assert.AreSame(_randomMock.Object, engine.GetRandomGenerator());
        }
    }
}
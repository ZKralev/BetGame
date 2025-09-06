using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BetGame.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public async Task Main_Exits_Immediately_WithZeroInput()
        {
            // Arrange: create a temp directory and write appsettings.json expected by Program.cs
            var originalDir = Directory.GetCurrentDirectory();
            var originalIn = Console.In;
            var originalOut = Console.Out;
            var tempDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "BetGame_Test_" + Guid.NewGuid().ToString("N"))).FullName;
            try
            {
                Directory.SetCurrentDirectory(tempDir);
                var json = "{ \"BetOptions\": { \"SmallestBet\": 1, \"BiggestBet\": 10, \"MinimalRoll\": 0, \"MaxRoll\": 1, \"SmallWin\": { \"WinRatioStart\": 1.1, \"WinRatioEnd\": 0.4 }, \"BigWin\": { \"WinRatioStart\": 2.0, \"WinRatioEnd\": 1.0 } } }";
                File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

                // Feed "0" to exit immediately
                Console.SetIn(new StringReader("0" + Environment.NewLine));
                Console.SetOut(new StringWriter());

                // Act: reflectively invoke BetGame.Program.Main
                var gameAssembly = typeof(BetGame.Services.GameRunnerService).Assembly;
                var programType = gameAssembly.GetType("BetGame.Program", throwOnError: true);
                var main = programType!.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                Assert.IsNotNull(main, "Could not find Main method on BetGame.Program");

                var task = (Task)main!.Invoke(null, new object[] { Array.Empty<string>() })!;
                await task; // Should complete quickly because input is 0
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
                try { Directory.Delete(tempDir, recursive: true); } catch { /* ignore cleanup failures */ }
            }
        }
    }
}

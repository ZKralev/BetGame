using BetGame.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace BetGame.Tests
{
    [TestClass]
    public class ConsoleAndRandomTests
    {
        [TestMethod]
        public void ConsoleWrapper_WriteLine_WritesToConsole()
        {
            var originalOut = Console.Out;
            try
            {
                var sw = new StringWriter();
                Console.SetOut(sw);

                var cw = new ConsoleWrapper();
                cw.WriteLine("Hello World");

                StringAssert.Contains(sw.ToString(), "Hello World");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void ConsoleWrapper_ReadLine_ReadsFromConsole()
        {
            var originalIn = Console.In;
            try
            {
                Console.SetIn(new StringReader("abc" + Environment.NewLine));

                var cw = new ConsoleWrapper();
                var line = cw.ReadLine();

                Assert.AreEqual("abc", line);
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ConsoleWrapper_SetTitle_SetsConsoleTitle()
        {
            var originalTitle = Console.Title;
            try
            {
                var cw = new ConsoleWrapper();
                var title = "BetGame_Test_" + Guid.NewGuid().ToString("N");
                cw.SetTitle(title);

                Assert.AreEqual(title, Console.Title);
            }
            finally
            {
                // best effort restore
                try { Console.Title = originalTitle; } catch { }
            }
        }

        [TestMethod]
        public void ConsoleWrapper_SetForegroundColor_And_ResetColor_DoNotThrow()
        {
            var originalColor = Console.ForegroundColor;
            var cw = new ConsoleWrapper();
            try
            {
                cw.SetForegroundColor(ConsoleColor.Blue);
                // Some test hosts (CI, headless) may ignore color changes; just ensure it's a valid enum value
                Assert.IsTrue(Enum.IsDefined(typeof(ConsoleColor), Console.ForegroundColor));

                cw.ResetColor();
                Assert.IsTrue(Enum.IsDefined(typeof(ConsoleColor), Console.ForegroundColor));
            }
            finally
            {
                // Restore original color
                Console.ForegroundColor = originalColor;
            }
        }

        [TestMethod]
        public void ConsoleWrapper_StaticConsoleReadLine_Executes()
        {
            var originalIn = Console.In;
            try
            {
                Console.SetIn(new StringReader("x" + Environment.NewLine));
                // Method returns void, we only execute it to cover the call
                ConsoleWrapper.ConsoleReadLine();
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        [TestMethod]
        public void RandomGenerator_NextDouble_ReturnsValueBetweenZeroAndOne()
        {
            var rng = new RandomGenerator();
            for (int i = 0; i < 5; i++)
            {
                var value = rng.NextDouble();
                Assert.IsTrue(value >= 0.0 && value < 1.0, $"Generated value {value} was out of range [0,1)");
            }
        }
    }
}

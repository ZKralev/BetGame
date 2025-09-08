# BetGame

A small console betting game built with C# and .NET. Manage a wallet, place bets, and try your luck with random multipliers.

## Prerequisites

- .NET SDK 9.0 or later
  - Check your version: `dotnet --version`

## Repository layout

```
BetGame/               # Solution root
├── BetGame.sln        # Solution
├── Game/              # Console application (this project)
│   ├── Abstractions/  # Concrete wrappers & simple domain types (ConsoleWrapper, Wallet, BetResult)
│   ├── Interfaces/    # Interfaces (IWallet, IConsoleWrapper, IBetEngine, etc.)
│   ├── Models/        # Options & enums (BetOptions, MenuOptions)
│   ├── Services/      # Core services (WalletService, BetEngineService, GameRunnerService)
│   ├── appsettings.json
│   └── BetGame.csproj
└── BetGame_Test/      # MSTest test project
    └── BetGame_Test.csproj
```

## Running the app

From the solution root:

```bash
# Run the console app
dotnet run --project .\Game
```

The game will display a menu:

- 1) Deposit
- 2) Withdraw
- 3) Place Bet
- 0) Exit

## Building

```bash
# Build all projects (Debug)
dotnet build .\BetGame.sln
```

## Configuration

The console app reads options from `Game/appsettings.json` under the `BetOptions` section. Example:

```json
{
  "BetOptions": {
    "SmallestBet": 1,
    "BiggestBet": 10,
    "MinimalRoll": 0,
    "MaxRoll": 1,
    "SmallWin": {
      "WinRatioStart": 1.1,
      "WinRatioEnd": 0.4
    },
    "BigWin": {
      "WinRatioStart": 2.0,
      "WinRatioEnd": 1.0
    }
  }
}
```

## Testing

I use MSTest + Moq. To run all tests:

```bash
# From the solution root
dotnet test .\BetGame.sln
```

## Code Coverage

This solution uses the `coverlet.collector` data collector. To collect coverage and generate an HTML report:

```bash
# 1) Collect coverage (Cobertura)
dotnet test .\BetGame.sln --collect:"XPlat Code Coverage" --results-directory .\TestResults\Coverage

# 2) Generate HTML report (requires reportgenerator)
# If not installed:
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator -reports:"TestResults\Coverage\**\coverage.cobertura.xml" \
  -targetdir:"TestResults\CoverageReport" \
  -reporttypes:"Html;HtmlSummary"

# Open the report
start .\TestResults\CoverageReport\index.html
```

## Architecture (quick overview)

- `Program.cs` wires up DI, loads `BetOptions` from configuration, and runs `GameRunnerService`.
- `GameRunnerService` handles the CLI menu and orchestrates:
  - `WalletService` for deposit/withdraw/balance updates.
  - `BetEngineService` for the betting round (random rolls, win logic).
- `ConsoleWrapper` abstracts console IO for easier testing.
- `Wallet` provides a basic in-memory implementation of `IWallet`.

## License

This project is provided as-is for learning and demonstration purposes.

# Bet Game (C# .NET Console)

A small console betting game built with C# and .NET. Manage a wallet, place bets, and try your luck with random multipliers.

## Requirements

- .NET SDK 9.0 or later
  - Check your version: `dotnet --version`

## Getting Started

1. Clone this repository
2. Open a terminal in the project root (`bet game/`)

### Run (development)

```bash
# From the project directory
dotnet run
```

### Build

```bash
# Build Debug
dotnet build

# Build Release
dotnet build -c Release
```

After building, binaries will be in:
- `bin/Debug/net9.0/` (Debug)
- `bin/Release/net9.0/` (Release)

## How to Play

On launch you will see a menu:

- 1) Deposit
- 2) Withdraw
- 3) Place Bet
- 0) Exit

Enter the number of your choice and follow the prompts.

Balances and winnings are displayed with two decimal places (e.g., `$12.50`).

## Game Rules

- Min/Max bet: `1` to `10` (see `BetEngine.MinBet`/`MaxBet`)
- Each bet rolls a number from 1 to 10 (inclusive):
  - 1–5: Lose the stake
  - 6–9: Win a random multiplier in the range [0.0, 2.0) × stake
  - 10: Win a random multiplier in the range [2.0, 10.0) × stake
- Your wallet balance is updated as: `balance = balance - stake + winAmount`

Note: The multiplier ranges are half-open intervals; the upper bound is not included.

## Example Session

```text
=== Bet Game ===
1) Deposit
2) Withdraw
3) Place Bet
0) Exit
Please submit your action: 1
Enter amount to deposit: 20
Current Balance: $20.00

Please submit your action: 3
Enter amount between 1 and 10 to bet: 5
Congratulations! You win 7.35! Your current balance is:  $22.35

Please submit your action: 2
Enter amount to withdraw: 10
Current Balance: $12.35

Please submit your action: 0
Goodbye! :)
```

## Project Structure

- `Program.cs`
  - Console UI loop and menu
  - Actions: `Deposit()`, `Withdraw()`, `PlaceBet()`
- `Wallet.cs`
  - Tracks `Balance`
  - Methods: `TryDeposit()`, `TryWithdraw()`, `TryPlaceBet()`
- `BetEngine.cs`
  - Game constants: `MinBet`, `MaxBet`
  - Core logic: `PlayRound(stake)` returns `BetResult`
  - Outcome probabilities and multipliers
- `MenuOptions.cs`
  - Enum for menu choices

## Configuration

- Change bet limits in `BetEngine.cs`:
  - `public const int MinBet = 1;`
  - `public const int MaxBet = 10;`
- Default starting balance is `0`. You can pass an initial value to `Wallet`'s constructor if you want to start with funds.

## Notes

- This sample uses `double` for monetary values. For production scenarios, prefer `decimal` to avoid floating‑point rounding issues.

## License

Add your preferred license here (e.g., MIT).

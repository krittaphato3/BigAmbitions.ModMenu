# ItzRealOzone Trainer

A MelonLoader mod for **Big Ambitions** (Hovgaard Games) that adds an in-game trainer accessible via the phone menu. Made by ItzRealOzone.

## Features

The trainer is organized into 7 tabbed panels, accessible from the in-game phone:

### Money
- Quick-add buttons ($1K, $5K, $10K, $50K, $100K, $500K, $1M)
- Custom amount input (add or set exact money)
- Economy sliders: tax percentage, market price multiplier, export multiplier

### Player
- Fill all needs (energy, happiness, hunger) instantly
- Energy level slider + quick-set buttons (25/50/75/100)
- Happiness +/- buttons
- Hunger +/- buttons
- Movement speed presets (Walk, Jog, Run, Scooter)
- Decay toggles: disable energy/happiness/hunger decay, disable aging
- Age adjustment (-5, -1, +1, +5 years)
- Complete all personal goals

### Vehicles
- Toggle disable vehicle damage/fuel
- Repair, refuel, clean current vehicle
- Clear parking tickets
- Tow to gas station or auto repair

### Business
- Max all customer satisfaction
- Unlock all courses, contacts, disable wholesale/import limits, all products from importers
- Business multiplier sliders: customer promotion, employee salary, wholesale/importer urgent fees, bank interest rate, rivals difficulty
- Business list with search/filter and teleport to owned buildings

### Gameplay
- Game speed slider + presets (Pause, 1x, 2x, 5x, 10x)
- Skip to next day
- Time presets (6AM, 12PM, 6PM, 10PM) + custom time input
- Toggle traffic, tutorial, invincibility
- Complete quest/objective, unlock all contacts
- Teleport to quest target or GPS destination
- Deliver all imports (paid or free)
- Bank interest multiplier slider
- Quick save

### Staff (Employees)
- Max ALL employee satisfaction
- Salary multiplier slider + presets (Free, 0.5x, 1x, 2x)
- Set all wages
- Generate recruitment candidates by skill type (Customer Service, Cleaning, Lawyer, Purchasing, Logistics, Delivery, Programmer, HR Manager)

### Rivals
- Refresh rivals data
- Defeat all rivals instantly
- Difficulty presets (Easy, Normal, Hard, Brutal)

## Installation

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader) v0.6+ on Big Ambitions
2. Download the latest `BigAmbitionsTrainer.dll` from [Releases](https://github.com/aluna/BigAmbitionsTrainer/releases)
3. Place the `.dll` in `Big Ambitions/Mods/`
4. Launch the game and open your phone to see the Trainer app

## Configuration

The mod registers MelonLoader preferences under the **ItzRealOzone Trainer** category. These can be configured via MelonLoader's built-in settings UI or `MelonPreferences.cfg`:

| Setting | Default | Description |
|---------|---------|-------------|
| DisableEnergy | false | Disable player energy decay |
| DisableHappiness | false | Disable player happiness decay |
| DisableAging | false | Disable player aging |
| DisableVehicleDamage | false | Disable vehicle damage accumulation |
| DisableVehicleFuel | false | Disable vehicle fuel consumption |
| AllCoursesUnlocked | false | Unlock all education courses |
| AllContactsUnlocked | false | Unlock all business contacts |
| DisableWholesaleImportLimits | false | Remove wholesale/import quantity limits |
| AllProductsFromImporters | false | Make all products available from importers |
| DisableTraffic | false | Disable NPC traffic |
| DisableTutorial | false | Disable tutorial pop-ups |
| Invincibility | false | Enable player invincibility |
| DisableHunger | false | Disable player hunger decay |
| GameSpeed | 1.0 | Game speed multiplier |
| PhoneIntegration | true | Enable in-game phone menu integration |

## Project Structure

```
BigAmbitionsTrainer/
├── BigAmbitionsTrainer.csproj          # .NET 6.0 project targeting MelonLoader
├── Properties/
│   └── AssemblyInfo.cs                # MelonLoader mod metadata
├── BigAmbitionsTrainer.Core/
│   └── TrainerMain.cs                 # Mod entry point (MelonMod)
├── BigAmbitionsTrainer.Config/
│   └── TrainerConfig.cs               # MelonLoader preferences handler
├── BigAmbitionsTrainer.Modules/
│   ├── BusinessModule.cs              # Business manipulation logic
│   ├── EmployeeModule.cs              # Employee management
│   ├── GameplayModule.cs              # Game speed, time, quests, traffic
│   ├── MoneyModule.cs                 # Money and economy manipulation
│   ├── PlayerStatsModule.cs           # Player stats, needs, speed
│   ├── RivalsModule.cs                # Rival NPC management
│   ├── UndoSystem.cs                  # Generic undo/redo system
│   ├── VehicleModule.cs               # Vehicle management
│   └── WorldModule.cs                 # World-level settings
├── BigAmbitionsTrainer.PhoneIntegration/
│   ├── PhoneButtonInjector.cs         # Runtime UI injection into phone menu
│   └── TrainerPanel.cs                # Full in-game trainer UI panel (2379 lines)
├── BigAmbitionsTrainer.UI.Components/
│   └── ToastNotification.cs           # IMGUI toast notification system
└── BigAmbitionsTrainer.Dashboard/
    ├── AnalyticsEngine.cs             # Financial analytics computation
    ├── AnalyticsResult.cs             # Analytics result model
    ├── CompanyBreakdown.cs            # Per-company breakdown model
    ├── DailyTotal.cs                  # Daily income/expense model
    ├── IncomeExpenseBar.cs            # Income vs expense bar model
    ├── Transaction.cs                 # Financial transaction model
    ├── TransactionParser.cs           # CSV transaction parser
    └── TypeBreakdown.cs              # Per-type breakdown model
```

## Building

```bash
# Prerequisites: MelonLoader, Big Ambitions game assemblies
dotnet build BigAmbitionsTrainer.csproj -c Release
```

Place the required MelonLoader and game assemblies (MelonLoader.dll, Il2CppBigAmbitions.dll, etc.) in your project's reference paths, or update the `.csproj` reference hints.

## Technical Overview

- **Framework**: .NET 6.0 (Il2Cpp / MelonLoader)
- **Language**: C# with latest language features (LangVersion 15.0, file-scoped namespaces)
- **UI**: The trainer panel is built entirely at runtime using Unity UI components (no asset bundles)
- **Phone Integration**: Dynamically clones an existing phone app button, re-parents it, and hooks click events
- **Toasts**: Custom IMGUI-based notification system rendered via OnGUI
- **Dashboard**: Financial analytics engine that parses the game's Transaction CSV for income/expense analysis (subsystem, not currently wired to the phone UI)

## License

Distributed under the MIT License. See `LICENSE` for more information.

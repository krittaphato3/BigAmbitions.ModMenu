# ItzRealOzone Trainer

A MelonLoader mod for **Big Ambitions** (Hovgaard Games) that adds an in-game trainer accessible via the in-game phone and a standalone F8 overlay. Made by ItzRealOzone.

## Features

The trainer is organized into 8 tabbed panels. Access it by pressing **F8** for the standalone overlay, or open your in-game phone and tap the Trainer app.

### Money
- Quick-add buttons ($1K, $5K, $10K, $50K, $100K, $500K, $1M)
- Economy cycle buttons: tax percentage, market price multiplier

### Player
- Fill all needs (energy, happiness, hunger) instantly
- Energy level quick-set buttons (25/50/75/100)
- Happiness +/- buttons
- Hunger +/- buttons
- Movement speed presets (Walk, Jog, Run, Scooter)
- Decay toggles: disable energy/happiness/hunger decay, disable aging
- Age adjustment (-5, +5 years)
- Complete all personal goals

### Vehicles
- Toggle disable vehicle damage/fuel
- Repair, refuel, clean current vehicle
- Clear parking tickets
- Tow to gas station or auto repair

### Business
- Max all customer satisfaction
- Unlock all courses, contacts, disable wholesale/import limits, all products from importers
- Business multiplier cycle buttons: customer promotion, employee salary, bank interest rate, rivals difficulty

### Gameplay
- Game speed presets (Pause, 1x, 2x, 5x, 10x)
- Skip to next day
- Time presets (6 AM, 12 PM, 6 PM, 10 PM)
- Toggle traffic, tutorial, invincibility
- Complete quest/objective
- Deliver all imports (paid or free)
- Quick save

### Staff (Employees)
- Max ALL employee satisfaction
- Salary multiplier presets (Free, 0.5x, 1x, 2x)
- Generate recruitment candidates by skill type (Customer Service, Cleaning, Lawyer, Purchasing, Logistics, Delivery, Programmer, HR Manager)

### Rivals
- Refresh rivals data
- Defeat all rivals instantly
- Difficulty presets (Easy, Normal, Hard, Brutal)

### Settings
- Save/Load all toggle settings to MelonPreferences
- Toggle phone integration on/off
- Close overlay button

## F8 Standalone Overlay

Pressing **F8** toggles an IMGUI overlay window that provides instant access to all trainer features without needing to open the phone:

- 880x640 pixel window centered on screen
- 8 tabbed categories matching the phone layout
- Fade animation (0.15s) on open/close
- Scrollable content areas with scroll indicator
- Hover-brightened buttons with visible background colors
- Full resource lifecycle — all textures destroyed on close and recreated fresh on each open
- Works independently of phone integration (no phone app needed)

## Installation

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader) v0.6+ on Big Ambitions
2. Download the latest `ItzRealOzoneBATrainer.dll` from [Releases](https://github.com/aluna/BigAmbitionsTrainer/releases)
3. Place the `.dll` in `Big Ambitions/Mods/`
4. Launch the game; press **F8** or open your phone to see the Trainer app

## Configuration

The mod registers MelonLoader preferences under the **ItzRealOzone Trainer** category. Configure via MelonLoader's built-in settings UI or `MelonPreferences.cfg`:

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
| ToastDuration | 3.0 | Toast notification display duration (seconds) |
| ToastFadeStart | 2.0 | Time at which toast starts fading (seconds) |

Toggle settings can be saved/loaded in-game via the Settings tab's **Save All Settings** / **Load All Settings** buttons.

## Project Structure

```
BigAmbitionsTrainer/
├── BigAmbitionsTrainer.csproj           # .NET 6.0 project targeting MelonLoader
├── Properties/
│   └── AssemblyInfo.cs                 # MelonLoader mod metadata
├── BigAmbitionsTrainer.Core/
│   └── TrainerMain.cs                  # Mod entry point (MelonMod), F8 handler
├── BigAmbitionsTrainer.Config/
│   └── TrainerConfig.cs                # MelonLoader preferences handler
├── BigAmbitionsTrainer.Modules/
│   ├── BusinessModule.cs               # Business manipulation logic
│   ├── EmployeeModule.cs               # Employee management
│   ├── GameplayModule.cs               # Game speed, time, quests, traffic
│   ├── MoneyModule.cs                  # Money and economy manipulation
│   ├── PlayerStatsModule.cs            # Player stats, needs, speed
│   ├── RivalsModule.cs                 # Rival NPC management
│   └── VehicleModule.cs                # Vehicle management
├── BigAmbitionsTrainer.PhoneIntegration/
│   ├── PhoneButtonInjector.cs          # Runtime UI injection into phone menu
│   └── TrainerPanel.cs                 # Full in-game phone trainer UI panel
├── BigAmbitionsTrainer.UI.Components/
│   ├── ConfirmationDialog.cs           # IMGUI confirmation modal
│   ├── ToastNotification.cs            # IMGUI toast notification system
│   ├── TooltipManager.cs               # IMGUI per-frame hover tooltip
│   └── TrainerOverlay.cs               # F8 standalone IMGUI overlay
└── BigAmbitionsTrainer.Dashboard/
    ├── AnalyticsEngine.cs              # Financial analytics computation
    ├── AnalyticsResult.cs              # Analytics result model
    ├── CompanyBreakdown.cs             # Per-company breakdown model
    ├── DailyTotal.cs                   # Daily income/expense model
    ├── IncomeExpenseBar.cs             # Income vs expense bar model
    ├── Transaction.cs                  # Financial transaction model
    ├── TransactionParser.cs            # CSV transaction parser
    └── TypeBreakdown.cs               # Per-type breakdown model
```

## Building

```bash
# Prerequisites: MelonLoader, Big Ambitions game assemblies
dotnet build -c Release
```

Output: `bin/Release/net6.0/ItzRealOzoneBATrainer.dll`

Place the required MelonLoader and game assemblies (MelonLoader.dll, Il2CppBigAmbitions.dll, etc.) in your project's reference paths, or update the `.csproj` reference hints.

## Technical Overview

- **Framework**: .NET 6.0 (Il2Cpp / MelonLoader)
- **Language**: C# 15.0, file-scoped namespaces
- **Phone UI**: Built entirely at runtime using Unity uGUI components (no asset bundles). Clones an existing phone app button and hooks click events. The game's native layout groups handle button positioning — no manual override of existing button sizes.
- **F8 Overlay**: Custom IMGUI UI rendered via `OnGUI`. Avoids stripped IL2CPP methods (`GUI.Button`, `GUI.BeginScrollView`, `GUI.skin`) by using manual hit-testing, `GUI.BeginGroup`, and `GUIStyleState`-only construction. Background and text are drawn in separate passes matching the ToastNotification pattern for reliable rendering.
- **Toasts**: Custom IMGUI-based notification system rendered via `OnGUI`
- **Dashboard**: Financial analytics engine that parses the game's Transaction CSV for income/expense analysis (subsystem, not currently wired to the phone UI)

## Quality of Life Features

- **Search bar**: Filter buttons across all tabs by label text
- **Collapsible cards**: Click the arrow (▼) on any card header to collapse/expand
- **Confirmation dialogs**: Destructive actions show an IMGUI confirmation before executing
- **Button tooltips**: Hovering over any action button shows a tooltip near the cursor
- **Candidate skill slider**: Employee tab slider (1–100) for generated candidate skill levels
- **Toast configuration**: Duration and fade timing configurable via MelonPreferences
- **Settings save/load**: Persist and restore all toggle states from the Settings tab

## Performance

- **Tick cooldown**: Module OnUpdate calls throttled to every 30 frames; lightweight tasks run every frame
- **Resource lifecycle**: Overlay textures created per-open and destroyed on close, preventing stale texture context issues
- **Asset caching**: Game sprite/font lookups performed once and cached for the panel's lifetime

## License

Distributed under the MIT License. See `LICENSE` for more information.

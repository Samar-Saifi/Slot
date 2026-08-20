# Slot Machine

A simple 3-reel slot machine game built in Unity, featuring weighted random symbol generation, a betting system, and physically-animated spinning reels.

## Game Overview

You start with a balance of **30 credits**. Each round you choose a bet (**5, 10, or 20**), pull the handle to spin three reels, and if all three reels land on the **same symbol type**, you win — your balance is multiplied by that symbol's payout multiplier.

**Symbols and behavior** are fully data-driven via `SCO_SlotItem` ScriptableObjects (`Cherry`, `Bell`, `Bar`, `Seven`), each configured with:
- An **icon** (sprite shown on the reel)
- A **payout multiplier** (applied to your balance on a win)
- A **weight** (controls how often that symbol appears — lower weight means rarer, so high-value symbols like `Seven` can be tuned to appear less often than `Cherry`)

**Controls:**
| Action | Key |
|---|---|
| Pull handle / Spin | `Space` |
| Increase bet | `↑` / `W` |
| Decrease bet | `↓` / `S` |

The handle needs to be pulled once to start the reels, and then **once per reel** (in sequence) to stop them — mimicking a real mechanical slot machine rather than stopping all reels at once.

## Instructions to Run WebGL Build

**Option A — Play a published WebGL build on itch.io :**
1. Open `https://mdsamar.itch.io/slot` and play the build.

**Option B — Play a pre-built WebGL build :**
1. Navigate to the build folder (contains `index.html`, `Build/`, and `TemplateData/`).
   ```bash
   cd build/webGL
   ```
   
3. Use python script to run the server
   - Run a local server from the build folder,
     ```bash
     python server.py
     ```
   - Open `http://127.0.0.1:8000` in a browser and play.
   

## Bonus Features

- **Data-driven symbols**: New slot symbols can be added purely by creating a new `SCO_SlotItem` asset (`Slot → Item` in the Create menu) — no code changes needed.
- **Weighted RNG**: A generic, reusable `RNG.Pick<T>()` selects items proportionally to a configurable `weight` field, so the odds of landing each symbol (and therefore the payout balance of the machine) can be tuned entirely from the Inspector.
- **Sequential, physically-styled reel stopping**: Each reel accelerates, spins continuously, and eases to a stop individually (with an ease-out cubic curve) as the handle is pulled repeatedly, rather than all reels stopping simultaneously — closer to how a real slot machine feels.
- **Reactive UI via events**: Balance and bet UI update through `System.Action` events (`OnBalanceChanged`, `OnBetChanged`) rather than polling every frame.
- **New Input System support**: Spin/bet-adjust controls work across keyboard, gamepad, and XR controller bindings out of the box.

## Thought Process / Approach

The project is organized around a small set of focused, single-responsibility components:

- **`BettingManager`** owns the player's balance and bet amount, and exposes `TryPlaceBet()` / `AddWinnings()` so nothing else needs to touch balance math directly.
- **`ReelController`** only knows how to spin, scroll, and stop *its own* symbols, and reports back whichever symbol landed at the center via `GetReelSlot()`. It has no idea whether that's a win — it's purely presentational/mechanical.
- **`MachineController`** is the orchestrator: it listens for the "pull handle" input, deducts the bet on the first pull of a round, tells each reel in turn to start/stop spinning, and once every reel has been stopped it hands the result off to be evaluated.
- **`GameManager`** is the sole place that decides whether a round is a win by comparing the `ItemType` of the symbol landed on each reel, then converts that into a payout using the winning symbol's `payoutMultiplier`.
- **`SCO_SlotItem`** (a `ScriptableObject`) turns each symbol into a designer-editable data asset (icon, type, payout multiplier, spawn weight), so balancing the machine's odds and payouts is an Inspector task, not a code task.
- **`RNG`** centralizes weighted-random selection in one generic, reusable method so both the initial reel fill and the "next symbol scrolled into view" logic share identical, testable odds logic.

This separation keeps input handling, visual/physics simulation, win evaluation, and economy/balance concerns independent of each other, so, for example, the payout rules or reel-stopping feel can each be changed without touching the other systems.

**Trade-offs / things I'd revisit with more time:** the win condition currently only checks for "all three reels match," so there's no support for partial-match payouts (e.g. two-of-three) or paylines across multiple rows; the UI is minimal (balance + bet display only, no win/lose feedback banner or animation); and `GameManager.CheckSlots` computes earnings from the *current* balance rather than the amount wagered, which is worth revisiting if bet size is meant to directly scale winnings.

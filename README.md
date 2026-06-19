# delta-grid-toy

A card-match (memory) game built in Unity 6.3 LTS. Flip cards, find pairs, win when the board clears.

Built as a take-home technical assessment. The repo name is intentionally obscure — don't want the solution floating around indexed under obvious search terms.

---

## Running the project

Unity 6.3 LTS. Open the project, load `Assets/Scenes/GameScene`, hit Play.

For Android: switch platform in Build Settings, connect a device or start an emulator, Build & Run. The back button is handled, and the game autosaves on pause.

No third-party packages. No asset store. No tween libraries. All animation is coroutine-based using `Mathf` and `AnimationCurve`.

---

## Supported grid layouts

2×2, 2×3, 3×3, 4×3, 4×4, 5×3, 5×4, 5×5

Odd-count grids (3×3, 5×3, 5×5) get a wildcard card — `pairId = -1` — that auto-matches when flipped solo. Cards scale to fit the display area regardless of layout.

---

## Project structure

```
Assets/
├── Scripts/
│   ├── Core/         — GameManager, BoardManager, ScoreManager
│   ├── Cards/        — Card, CardData
│   ├── Data/         — SaveManager, ShuffleController, GridConfig
│   ├── Audio/        — AudioManager
│   └── UI/           — HUD, WinScreen
├── Tests/
│   └── EditMode/     — ScoreCalculatorTests, CardDataTests
├── Prefabs/
├── Sprites/
└── Scenes/
```

---

## Architecture notes

`Card` runs a four-state machine: FaceDown → Flipping → FaceUp → Matched. Each card decides for itself whether it can accept input based on its own state — the board never locks globally. Doing it this way avoids a pile of special-case guards in `BoardManager` and keeps the continuous-input logic localised.

`BoardManager` maintains a `List<Card>` as a pending pair buffer rather than a simple two-variable approach. The buffer snapshot and clear happen synchronously inside `RequestFlip`, not inside coroutines after yield points. Without that, two coroutines sharing the same buffer could clear it at different times under rapid taps — stranding cards face-up with no way to recover.

Scoring logic is extracted from `ScoreManager` into a plain C# `ScoreCalculator` class so it can be unit tested in EditMode without needing a scene.

Shuffle is seeded Fisher-Yates via `ShuffleController`. Same seed always produces the same board.

Save/load persists matched state only. Cards mid-flip at save time reset to FaceDown on load. Combo windows aren't restored — that was a deliberate call, not an oversight.

Full write-up in `ARCHITECTURE.md`. Decision log in `DEVLOG.md`.

---

## Tests

EditMode tests in `Assets/Tests/EditMode/`. 9 tests total across `ScoreCalculatorTests` and `CardDataTests`. Run them from the Unity Test Runner window.

---

## AI disclosure

See `AI_DISCLOSURE.md`.

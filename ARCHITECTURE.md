# ARCHITECTURE

## Overview

The project follows a straightforward manager-based architecture. Each manager is responsible for a specific part of the game, and communication is handled through direct method calls and C# events where it makes sense.

Since this is a relatively small project, I avoided adding an event bus or any complex architecture. Unity's built-in lifecycle and simple event-driven communication were enough to keep the code clean and easy to follow.

---

# Core Systems

## GameManager

Acts as the starting point of the game.

When the game launches, it checks if a saved game exists. If one is found, it restores the previous state; otherwise, it starts a new game.

It also handles:

* Pause menu
* Escape / Android back button
* Auto-save when the application is paused or closed

Gameplay itself is handled by `BoardManager`, so `GameManager` doesn't contain any board or scoring logic.

---

## BoardManager

This is the main gameplay controller.

Its responsibilities include:

* Creating the card grid
* Tracking the currently selected cards
* Checking whether two cards match
* Detecting when the game is completed
* Saving and restoring the board state

During gameplay it also communicates with `ScoreManager` and `AudioManager` whenever score updates or sound effects are needed.

---

## Card

Each card manages its own visuals and flip animation.

The card state is controlled using the `CardState` enum (`FaceDown`, `Flipping`, `FaceUp`, `Matched`).

While a flip animation is playing, the button is temporarily disabled. Once a card is matched, it remains non-interactable, which naturally prevents double-clicks or unwanted input without adding extra checks.

The card never decides whether it matches another card. It simply reports the player's click to `BoardManager`.

---

## ScoreManager

Responsible for tracking:

* Current score
* Number of turns
* Combo state
* High score

The actual score calculation is handled by `ScoreCalculator`, allowing the gameplay logic to stay separate from the scoring rules.

Whenever the score changes, events are raised so the UI updates immediately.

The high score is stored using `PlayerPrefs`.

---

## ScoreCalculator

A plain C# class without any Unity dependencies.

It contains all score calculation logic, including combo timing and multiplier progression.

Separating this logic from `ScoreManager` makes it much easier to test or adjust scoring rules later without touching gameplay code.

---

## SaveManager

A small static helper responsible for saving and loading game data.

Game data is serialized using Unity's `JsonUtility` and stored inside `Application.persistentDataPath`.

If the save file doesn't exist or becomes invalid, it safely returns `null`, allowing the game to start with a fresh session instead of crashing.

---

## AudioManager

A lightweight singleton responsible for game sound effects.

Instead of using imported audio assets, simple sounds are generated through `AudioClip`.

It exposes methods like:

* PlayFlip()
* PlayMatch()
* PlayMismatch()
* PlayGameOver()

---

# Gameplay Flow

```
Player taps a card

↓
Card receives the click

↓
BoardManager validates the selection

↓
Second card selected?

    No
      → Wait for next selection

    Yes
      ↓
   Compare both cards

      Match
        → Mark cards as matched
        → Update score
        → Play match sound
        → Check win condition

      Mismatch
        → Update turn count
        → Play mismatch sound
        → Flip both cards back
```

---

# Save System

Whenever the game is saved, the complete gameplay state is stored, including:

* Board layout
* Random seed
* Matched cards
* Current score
* Elapsed time

Loading rebuilds the same board using the stored seed and immediately restores already matched cards, allowing the player to continue exactly where they left off.

---

# Design Decision

One area I'd improve with more time is `BoardManager`.

At the moment it handles several responsibilities, including board creation, card selection, match validation, win detection and save-state generation.

For this assignment, keeping those systems together made development faster and debugging much simpler.

If this project continued to grow, I'd split those responsibilities into smaller classes—for example, a dedicated `BoardSpawner` for creating the board and a `MatchEvaluator` for handling matching logic.

The current implementation works well for the project size, but separating those responsibilities would make the codebase easier to maintain as more gameplay features are added.

---
languages:
- csharp
products:
- windows-forms
page_type: sample
name: "Matching Game Sample for WinForms (.NET Framework C#)"
urlFragment: "matching-game-winforms-netframework-cs"
description: "A simple matching game using Windows Forms for .NET Framework"
---
# Matching game

This sample demonstrates a match-2 memory game where you try to match tiles in the fewest moves possible. Use this sample as the basis for an upgrade from .NET Framework to .NET [article](https://learn.microsoft.com/dotnet/desktop/winforms/migration/how-to-upgrade-winforms).

## Code structure

The solution is split into two projects:

- **`MatchingGame.Logic`** — Contains the game logic. Manages the board state, tracks which cards are flipped, counts turns, and saves the player's best score to the Windows Registry.

- **`MatchingGame`** — The WinForms UI. Renders the game board as a grid of tiles, handles player input, and shows a results message when all pairs are matched.

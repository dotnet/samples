---
languages:
- vb
products:
- windows-forms
page_type: sample
name: "Matching Game Sample for WPF (.NET Framework Visual Basic)"
urlFragment: "matching-game-wpf-netframework-vb"
description: "A simple matching game using WPF for .NET Framework"
---
# Matching game

This sample demonstrates a match-2 memory game where you try to match tiles in the fewest moves possible. Use this sample as the basis for an upgrade from .NET Framework to .NET [article](https://learn.microsoft.com/dotnet/desktop/wpf/migration/how-to-upgrade-wpf).

## Code structure

The solution is split into two projects:

- **`MatchingGame.Logic`** — Contains the game logic. Manages the board state, tracks which cards are flipped, counts turns, and saves the player's best score to the Windows Registry.

- **`MatchingGame`** — The WPF UI. Renders the game board as a grid of buttons in a XAML layout, handles player input, and shows a results message when all pairs are matched.

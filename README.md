## About The Game
Cardcaster is a roguelike where you collect powerful cards that synergize in unique ways. Fight off enemies as you traverse the dungeon.

Indie game built in Unity/C# (800 hours)

See [`Assets/_Scripts/`](Assets/_Scripts) for code. See [Cardcaster on steam](https://store.steampowered.com/app/3276950/Cardcaster/)

## Technical Highlights
There are other systems I didn't include here.

### Procedural Generation
- Implemented a room-graph generation system.  
- Preauthored room templates are used to build each level.
- A rule-based connector (via ScriptableObjects) determines valid room connections.
- This approach is inspired by [Enter the Gungeon’s dungeon generation](https://www.boristhebrave.com/2019/07/28/dungeon-generation-in-enter-the-gungeon/).

### Deck System
- Implemented a full deck-hand-discard cycle.
- Supports operations: draw, play, discard, stack, replace, reshuffle.
- Maintains hand size dynamically in sync with player stats.

### Card Framework
- The card system uses `ScriptableObject` inheritance.
- Supports multiple card types: ability cards, modifiers, and persistent effects.
- Modular attributes (damage, duration, cooldown, area, projectile speed).
- Modifier system applies stat changes and effect prefabs at runtime.
- Plug-and-play card creation: new cards require no engine changes.
- Example cards:
  - **Dagger Shoot:** spawns projectiles with cooldown + applied effects.
  - **Open Palms:** modifies player hand size dynamically.


## What I’d improve

Some functions in `handcard.cs` are messy. There are various if statements checking the type of card. If I were still working on the game, I would I move this logic to the derived card scriptable objects.

The `CardUIManager`, `DeckManager`, and `Handcard` are more closely intertwined than I would like. Better encapsulation between these scripts would make the code clearer.

The card logic contains a lot of inheritance, which complicates the logic. If I were to start over, I would probably use composition instead.
This actually didn't make working with the card logic too difficult unlike the enemy logic, which I refactored from inheritance-based to composition-based.

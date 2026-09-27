# FrizzQOL Game Clock

Shows the in-game time on the HUD, using the game's font. The default spot is under the small minimap.

## Config

`BepInEx/config/com.frizzqol.gameclock.cfg`

| Setting | Default | What it does |
| --- | --- | --- |
| ClockFormat | 12h | `12h` or `24h`. Example: 6:42 PM or 18:42. |
| ShowAmPm | true | Adds AM or PM in 12-hour mode. Unused in 24-hour mode. |
| ShowDay | true | Shows the day number. |
| ShowWeather | true | Shows the current weather name. |
| ShowWind | true | Shows wind direction and strength. |
| ShowCold | true | Shows Cold while the world is cold. |
| ShowWet | true | Shows Wet while the world is wet. |
| ShowFreezing | true | Shows Freezing while the world is freezing. |
| Corner | TopRight | `TopRight` sits under the minimap. `TopLeft` and `TopCenter` use that corner of the HUD. |
| OffsetX | 0 | Pixels. Positive moves the label right. |
| OffsetY | 0 | Pixels. Positive moves the label up. |

## Multiplayer

The clock is local. Each player installs it and can use their own settings. A dedicated server does not need it.

## Source

https://github.com/frizzlebeard/FrizzQOL.GameClock

## Install

Install with r2modman or the Thunderstore Mod Manager.

To install by hand, copy `FrizzQOL.GameClock.dll` into `BepInEx/plugins`.

## Requirements

- Valheim
- [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)

## License

[MIT License](https://opensource.org/licenses/MIT). You can use, copy, change, and share this mod. The LICENSE file shipped with the package has the full text.

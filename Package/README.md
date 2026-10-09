# FrizzQOL Game Clock

https://github.com/frizzlebeard/FrizzQOL.GameClock

> The in-game time on your HUD, in the game's own font. 🕐

---

## What it does

A small clock sits under the minimap. It can also show the day, the weather name, the wind, and whether the world is cold, wet, or freezing. It hides while the big map is open. In the top-right spot it also hides when the small minimap is hidden. Long lines stay inside the minimap width.

The clock is yours alone. It does not change the world for anyone else.

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

Each player installs it and can use their own settings. A dedicated server does not need it.

## Install

Install with r2modman or the Thunderstore Mod Manager.

To install by hand, copy `FrizzQOL.GameClock.dll` into `BepInEx/plugins`.

## Requirements

- Valheim
- [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)

## Release notes

### 0.1.1

The clock hides while the big map is open. In the top-right spot it also hides when the small minimap is hidden. The label stays under the minimap instead of climbing onto it, and long lines stay inside the minimap width.

### 0.1.0

First public release.

The clock sits under the minimap in the game's own font. Time can be 12-hour or 24-hour. The day, the weather name, the wind, and cold, wet, or freezing can each be turned off. TopRight follows the minimap. TopLeft and TopCenter use that corner of the HUD. OffsetX and OffsetY nudge the label by pixels.

Each player keeps their own settings. A dedicated server does not need the mod.

## Support

☕ If you enjoy my work, please buy me a coffee.

Cash App: `$FrizzleFry4`

## License

[MIT License](https://opensource.org/licenses/MIT). You can use, copy, change, and share this mod. The LICENSE file shipped with the package has the full text.

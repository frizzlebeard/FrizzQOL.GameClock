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

## Support

☕ If you enjoy my work, please buy me a coffee.

Cash App: `$FrizzleFry4`

## License

[MIT](LICENSE). You can use, copy, change, and share this mod. Keep the copyright notice with any copy.

## Building

1. Copy `Environment.props.example` to `Environment.props`.
2. Set your Valheim and BepInEx folders in that file.
3. From this folder, run:

```
dotnet build GameClock.sln -c Release
```

The plugin file is `FrizzQOL.GameClock.dll`, under the project `bin\Release\net48` folder.

`Environment.props` stays on your machine. It is listed in `.gitignore`.

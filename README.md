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

## Install

Install with r2modman or the Thunderstore Mod Manager.

To install by hand, copy `FrizzQOL.GameClock.dll` into `BepInEx/plugins`.

## Requirements

- Valheim
- [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)

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

## Publishing

This folder is ready to push as its own public repository. Create an empty GitHub repo named `FrizzQOL.GameClock`. Do not add a README, license, or gitignore on GitHub. Those files are already here. Then run:

```
git remote add origin https://github.com/<you>/FrizzQOL.GameClock.git
git push -u origin main
```

Set `website_url` in `Package/manifest.json` to that repository before the Thunderstore upload.

## Thunderstore package

A valid upload is a zip whose root contains `icon.png`, `README.md`, and `manifest.json`. `CHANGELOG.md` is optional and is included here. Copy `FrizzQOL.GameClock.dll` from the Release build into `Package`, then zip the files themselves. Do not zip the `Package` folder. If the files sit inside a folder in the zip, Thunderstore rejects the package.

- `manifest.json`
- `README.md`
- `CHANGELOG.md`
- `icon.png`
- `LICENSE`
- `FrizzQOL.GameClock.dll`

The dll belongs at the zip root. The mod manager installs those files under `BepInEx/plugins/<Team>-<PackageName>/`.

Before you upload, check the package readme in the [markdown preview](https://thunderstore.io/tools/markdown-preview/) and the manifest in the [manifest validator](https://thunderstore.io/tools/manifest-v1-validator/). The package rules are in [Creating a Package](https://wiki.thunderstore.io/mods/creating-a-package).

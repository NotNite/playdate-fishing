# Playdate Fishing

FFXIV fishing controlled by a Playdate.

## What?

[The Playdate](https://play.date/) is a handheld game console with a crank. [A friend](https://github.com/KazWolfe) gave me the idea to connect the crank to FFXIV fishing. Here it is.

## How?

The Playdate exposes a [serial connection over USB](https://github.com/jaames/playdate-reverse-engineering/blob/main/usb/usb.md). There are two features on the serial connection we can (ab)use:

- Anything you `print` in a game gets logged to the serial port
- You can evaluate Lua bytecode with the `eval` command

Note the *bytecode* in that sentence. We can evaluate custom Lua on the fly, but compiling a new program for each command sounds miserable. However, I have [too much experience with Lua bytecode](https://notnite.com/blog/ffxiv-modloader-ace/), and I found [this excellent GitHub gist](https://gist.github.com/ericlewis/43d07016275308de11a5519466deea85) that details splicing data into Lua bytecode to pass JSON to the game. I reimplemented that gist in C#, allowing me to have two way communication with everyone's favorite cheese slice.

## Installation

This guide assumes you are a developer that is probably in the Playdate developer ecosystem but is probably not in the FFXIV developer ecosystem. You will have to build this from source - I'm not providing binaries for a project this dumb.

Requirements:

- A [Playdate](https://play.date/) and [XIVLauncher](https://goatcorp.github.io/)
- The [Playdate SDK](https://play.date/dev/) and [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download)

First, build and install the Playdate game (making sure you have the Playdate SDK in your path, and your Playdate is connected via USB and is unlocked):

```shell
pdc ./playdate ./com.notnite.playdatefishing.pdx
pdutil install ./com.notnite.playdatefishing.pdx
```

Then, build the Dalamud plugin:

```shell
dotnet build ./dalamud/PlaydateFishing
```

Now, open FFXIV. Go into the Experimental tab of the Dalamud settings (`/xlsettings`), and add the path to the built DLL to the "Dev Plugin Locations" section. Then, do the following:

- Plug in your Playdate to your computer
- Ensure your Playdate is unlocked on the home screen
- Enable the Dalamud plugin, then start the Playdate game, in that exact order

Congrats - you can now rethink your life choices. It is suggested to disable the plugin before closing the game.

## Controls

- Undock the crank to use Cast
- Dock the crank to use Quit
- Crank to use Hook
  - If you use Patience, the speed you crank at determines whether to use Precision Hookset or Powerful Hookset

## Contributing

First, don't.

Second, I work on the Playdate part in Visual Studio Code and the FFXIV part in JetBrains Rider. If you're contributing to the Lua part, consider making a `.vscode/settings.json` like the following, to silence some annoying errors:

```json
{
  "Lua.workspace.library": [
    "<path to your playdate SDK>/CoreLibs",
    "playdate/annotations.lua"
  ],
  "Lua.diagnostics.globals": [
    "playdate",
    "fishing",
    "input",
    "log",
    "json"
  ],
  "Lua.runtime.special": {
    "import": "require"
  }
}
```

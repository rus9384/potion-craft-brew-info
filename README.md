# Brewing Information

## Description

Brewing Information is a mod for Potion Craft that will show you some important information about brewins process:
- Ingredient grind status.
- Potion health.
- Potion coordinates on the map.
- Potion rotation angle.
- Path angle (where the potion will travel if you will stir).

## Installation
To install Brewing Information, you will need to follow the following steps (if you already installed other mods, skip to step 3):
1. Install [BepInEx 5.x x64 bit](https://github.com/BepInEx/BepInEx/releases/latest).
2. Launch the game and close it.
3. Copy `com.rus9384.brewinginformation.dll` file to `BepInEx/plugins` folder.

## Customization
After installing Brewing Information and launching the game once, `BepInEx/config` folder will have `com.rus9384.brewinginformation.cfg` file. You can edit this file to customize mod behavior:
- `General` section:
- - `EnableGrindStatus` - this setting controls whether the information about ingredient grind is shown. Possible values: `true`, `false`.
- - `EnableHealth` - this setting controls whether the information about potion health is shown. Possible values: `true`, `false`.
- - `EnablePotionCoordinates` - this setting controls whether the information about potion coordinates is shown. Possible values: `true`, `false`.
- - `EnablePotionRotation` - this setting controls whether the information about potion rotation angle is shown. Possible values: `true`, `false`.
- - `EnablePathAngle` - this setting controls whether the information about path angle is shown. Possible values: `true`, `false`.
- `Graphics` section:
- - `Scale` - this setting affects the size of information window. `100` is default scale. Recommended range of values: `40` to `150`.
- - `EnableScratches` - this setting controls whether the scratches visual effect is rendered. Possible values: `true`, `false`.

# BremuGb
![](https://github.com/Briensturm/BremuGb/workflows/.NET%20Core/badge.svg)

BremuGb is a cycle-accurate Gameboy emulator which is developed in C# and runs on .net core.

The main goals of the project are high accuracy and more importantly, a human readable codebase, which many existing Gameboy emulators are lacking.

Supported features:
* Full speed Gameboy emulation
* 4 channel 41KHz audio
* MBC 1-5 with battery saves
* Somewhat accurate rendering pipeline (Prehistorik Man)

Currently unsupported:
* CGB (Gameboy Color) emulation
* Emulation of more obscure MBCs/Hardware

## How to build and run
BremuGb consists of a reusable emulation library and a frontend which is based on OpenTK and OpenAL libraries.

Building BremuGb requires the .net core 3.1 SDK.

The releases which are provided in this repository are published in framework-dependent mode and require a .net core 3.1 runtime environment to be installed on the target machine.

To run BremuGb you can either specify the Gameboy rom file on the command line or drag and drop the rom onto the BremuGb executable.

## Controls
| Key        | Function             |
|------------|----------------------|
| Arrow Keys | D-Pad                |
| A          | A Button             |
| S          | B Button             |
| Enter      | Start Button         |
| Left Shift | Select Button        |
| Tab        | Fast Forward         |
| P          | Increase Screen Size |
| M          | Decrease Screen Size |
| Esc        | Exit                 |

## Screenshots

![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_zelda_1.png "Link's Awakening")
![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_zelda_2.png "Link's Awakening")

![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_pokemon_1.png "Pokemon")
![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_pokemon_2.png "Pokemon")

![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_mario_1.png "Mario")
![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_mario_2.png "Mario")

![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_prehistorik_1.png "Prehistorik Man")
![alt text](https://github.com/Briensturm/BremuGb/raw/master/Docs/images/screen_prehistorik_2.png "Prehistorik Man")


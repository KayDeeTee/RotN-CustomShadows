# RotN-CustomShadows
Lets you modify rift of the necrodancers shadows

## Installation
Install Bepinex and put Shadow Plugin.dll into `RiftOfTheNecroDancerOSTVolume1/Bepinex/Plugins`

## Usage
Create a folder called `CustomShadows`  at `Appdata/LocalLow/Brace Yourself Games/Rift of the NecroDancer` so it is next to the `CustomSongs` folder.

Put your custom shadow images in that folder, they should be a number that is the subdivision you want when divided by 192.

![image](https://github.com/user-attachments/assets/fe5cd8b8-2449-418b-bbd5-e277b1572a84)

In this example 48/192 = 1/4 so things on .25 of a beat will have that shadow
96/192 = 1/2 so things on .5 of a beat will have that shadow
64/192 would be 1/3, 128/192 is 2/3 etc.
-1 is unmatched

Images should be 512x512. You can trim them if you have unused space, but thats the scale you need to design them for.

## Compiling

You should just need to put the `Assembly-Csharp.dll` and `BugSplat.Unity.Runtime.dll` from the games files into a folder called `lib` next to plugin then run dotnet build.
But you will probably have to go through the bepinex plugin creation setup to get a working folder setup.

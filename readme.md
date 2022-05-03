# Analog Wheel Controls for Space Engineers

This is a plugin for Space Engineers that allows analog control of the brakes and throttle of wheeled vehicles. It works in singleplayer mode but it must be installed on the server for it to work in multiplayer. In the latter case, it's not necessary to have it on the client. In other words: If you want to use this in MP, you'll have to pester your local server admins (please don't) to make them install it.

The plugin was meant to be a proof of concept for KeenSWH to be able to test and implement a fix quickly and as such it is quite unrefined. One way it shows is that the braking curve (how hard it brakes depending on your input) is hard-coded. I basically brute-forced a bunch of numbers until I found something that works with my device. It can be changed in MyBrakeExtensions.UpdateBrake() to suit your needs.

Hopefully we won't need this much longer but until then, enjoy.


## Installation

* Download the plugin from the [Releases](https://github.com/Garbius/SEAnalogWheels/releases) section or build it from source
* Drop the dll files into your `common\SpaceEngineers\Bin64` folder
  * `SEAnalogWheels.dll`
  * `0Harmony.dll`
* Add `-plugin SEAnalogWheels.dll` to your Space Engineers launch options, which can be found by opening Space Engineers properties in your Steam Library and hitting the _"SET LAUNCH OPTIONS..."_ button


## Support

I can be found on the Wooting and KeenSWH Discord servers. Please provide the log file located in `%APPDATA%\SpaceEngineers\SpaceEngineers_xxxxxxxx_xxxxxxxxx.log` as it often contains information crucial to solving your problem. If it looks more like a bug, please open an issue on the GitHub page or create a pull request if you can fix it on your own.


## Build it yourself

To build this project, you need to reference the following assemblies from your `common\SpaceEngineers\Bin64` folder:
* `HavokWrapper`
* `Sandbox.Game`
* `VRage`
* `VRage.Game`
* `VRage.Library`
* `VRage.Math`

You will also need [WootingAnalogSDK.NET](https://www.nuget.org/packages/WootingAnalogSDK.NET/) in your project because that's what this is all about, and [Harmony](https://www.nuget.org/packages/lib.harmony) to build the more recent versions. 

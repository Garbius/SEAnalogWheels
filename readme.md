# Analog Wheel Controls for Space Engineers

This is a plugin for Space Engineers that allows analog control of the brakes and throttle of wheeled vehicles. It works in singleplayer mode but it must be installed on the server for it to work in multiplayer. In the latter case, it's not necessary to have it on the client. In other words: If you want to use this in MP, you'll have to pester your local server admins (please don't) to make them install it.

The plugin was meant to be a proof of concept for KeenSWH to be able to test and implement a fix quickly and as such it is quite unrefined. One way it shows is that the braking curve (how hard it brakes depending on your input) is hard-coded. I basically brute-forced a bunch of numbers until I found something that works with my device. It can be changed in MyBrakeExtensions.UpdateBrake() to suit your needs.

Hopefully we won't need this much longer but until then, enjoy.


## Installation

Install the custom game launcher from the [Github repository](https://github.com/sepluginloader/SpaceEngineersLauncher). Instructions are at the bottom of the page. It will automatically install [Plugin Loader](https://github.com/sepluginloader/PluginLoader). Start the game using the launcher and open the Plugins window from the main menu. Click the little plus sign in the bottom right corner of the Plugins column and find this plugin in the list and enable it. The game must be restarted before it can be used.

## Support

I can be found in the KeenSWH and Plugin Loader Discord servers. Please provide the log file located in `%APPDATA%\SpaceEngineers\SpaceEngineers_xxxxxxxx_xxxxxxxxx.log` as it often contains information crucial to solving your problem. If it looks more like a bug, please open an issue on the GitHub page or create a pull request if you can fix it on your own.


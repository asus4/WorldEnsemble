# World Instrument

Our team's submission for [Google’s Immersive Geospatial Challenge](https://googlesimmersive.devpost.com/) hackathon.

## Tested Environment

- Unity 2022.3.12f1. Should be compatible Unity 2022 LTS.
- macOS. Windows is not supported for now.

## Simulate on Unity Editor

- Pull this repository with **[Git-LFS](https://git-lfs.com/)**.
- Change the platform to iOS.  
  Editor Simulation on Android is not supported for now.
- Play and Enjoy the scene `Scenes/WorldInstrument`.

## How to build for iOS / Android

- Go Google's developer console and get the API key for Google Geospatial API.
  - See [AR Core Geospatial website](https://developers.google.com/ar/develop/ios/geospatial/enable) for more detail.
- Open `Project Settings/XR Plug-in Management/ARCore Extensions`
- Enter your Google Geospatial API key to `ProjectSettings/ARCoreExtensionsProjectSettings.json`
- Build for each platform.
  - iOS: We tested with Xcode 15.0. If you use earlier version of Xcode, Disable the post-build-process at [CustomPostprocessBuild.cs](https://github.com/asus4/WorldInstrument/blob/main/Assets/Scripts/Editor/CustomPostprocessBuild.cs).

## Our Team

- [@asus4](https://github.com/asus4): Creative coder
- [TWOTH](https://twoth.bandcamp.com/): Musician

## Open Source Libraries & Assets

### Assets

<p xmlns:cc="http://creativecommons.org/ns#" xmlns:dct="http://purl.org/dc/terms/"><a property="dct:title" rel="cc:attributionURL" href="https://github.com/asus4/WorldInstrument">World Instrument</a> by <a rel="cc:attributionURL dct:creator" property="cc:attributionName" href="https://twoth.bandcamp.com/">Towth</a> is licensed under <a href="http://creativecommons.org/licenses/by/4.0/?ref=chooser-v1" target="_blank" rel="license noopener noreferrer" style="display:inline-block;">CC BY 4.0<img style="height:22px!important;margin-left:3px;vertical-align:text-bottom;" src="https://mirrors.creativecommons.org/presskit/icons/cc.svg?ref=chooser-v1"><img style="height:22px!important;margin-left:3px;vertical-align:text-bottom;" src="https://mirrors.creativecommons.org/presskit/icons/by.svg?ref=chooser-v1"></a></p>

- [Sora Font](https://fonts.google.com/specimen/Sora/about): OFL 1.1
- [Berlin Riverside HDRI](https://hdri-haven.com/hdri/berlin-riverside): CC0

### Libraries

- [ARCore Extensions](https://github.com/google-ar/arcore-unity-extensions): Apache 2.0 and [ARCore Additional Terms of Service](https://developers.google.com/ar/develop/terms)
- [AR Foundation Replay](https://github.com/asus4/ARFoundationReplay): Unlicense
- [MemoryPack](https://github.com/Cysharp/MemoryPack): MIT

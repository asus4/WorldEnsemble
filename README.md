# World Instrument

Our team's submission for [Google’s Immersive Geospatial Challenge](https://googlesimmersive.devpost.com/) hackathon.

## Tested Environment

- Unity 2022.3.12f1. It will be compatible with Unity 2022 LTS.
- macOS. Windows is not supported for now.

## Simulate on Unity Editor

- Pull this repository with **[Git-LFS](https://git-lfs.com/)**.
- Simulation on the Unity Editor is supported only on iOS. Open Build Settings and switch the platform to iOS.
  ![fig-switch-platform](https://github.com/asus4/WorldInstrument/assets/357497/2bbcb90a-5f6f-4d2a-87a1-65db73f74a36)
- Play and Enjoy the scene `Scenes/WorldInstrument`.

## How to build for iOS / Android

- Go to Google's developer console and get the API key for Google Geospatial API.
  - See [AR Core Geospatial website](https://developers.google.com/ar/develop/ios/geospatial/enable) for more detail.
- Open `Project Settings/XR Plug-in Management/ARCore Extensions`
- Enter your Google Geospatial API key to `ProjectSettings/ARCoreExtensionsProjectSettings.json`
  ![fig-apikey](https://github.com/asus4/WorldInstrument/assets/357497/6c6beadc-3c74-4cd4-92fa-95f82571bf7f)
- Open `Assets/External Dependency Manager/` and resolve dependency.
  - On iOS, Run `pod update` in the build folder.
- Build for each platform.
  - iOS: We tested with Xcode 15.0. If you use the earlier version of Xcode, Disable the post-build-process at [CustomPostprocessBuild.cs](https://github.com/asus4/WorldInstrument/blob/main/Assets/Scripts/Editor/CustomPostprocessBuild.cs).

## Our Team

- [@asus4](https://github.com/asus4): Creative Technologist
- [TWOTH](https://twoth.bandcamp.com/): Musician

## License

This work is licensed under a [Creative Commons Attribution 4.0 International License][cc-by].

[![CC BY 4.0][cc-by-image]][cc-by]

[cc-by]: http://creativecommons.org/licenses/by/4.0/
[cc-by-image]: https://i.creativecommons.org/l/by/4.0/88x31.png
[cc-by-shield]: https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg

See [LICENSE](https://github.com/asus4/WorldInstrument/blob/main/LICENSE) for more.

## Assets and Open Source Libraries

### Assets

- [Sora Font](https://fonts.google.com/specimen/Sora/about): OFL 1.1
- [Berlin Riverside HDRI](https://hdri-haven.com/hdri/berlin-riverside): CC0

### Libraries

- [ARCore Extensions](https://github.com/google-ar/arcore-unity-extensions): Apache 2.0 and [ARCore Additional Terms of Service](https://developers.google.com/ar/develop/terms)
- [AR Foundation Replay](https://github.com/asus4/ARFoundationReplay): Unlicense
- [MemoryPack](https://github.com/Cysharp/MemoryPack): MIT

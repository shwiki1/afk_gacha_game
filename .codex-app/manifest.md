# App Manifest

## Identity
- Name: AFK Gacha / `afk_gacha_game`
- Runtime: Unity Android game client
- Repository root: `/storage/emulated/0/ZeroTermux/开发/开源项目/AFK-Gacha/afk_gacha_game`
- Public upstream: `Lambda-Forge/afk_gacha_game`
- Build fork: `shwiki1/afk_gacha_game`

## Source Of Truth
- Verify behavior against `client/Assets`, `client/Packages`, `client/ProjectSettings`, and `gateway.proto`.
- Keep `.codex-app/` current after meaningful changes, while treating source and workflow files as authoritative.

## Repo Map
- App runtime: `client/`
- Unity scripts: `client/Assets/Scripts/`
- Editor build entry point: `client/Assets/Editor/AfkAndroidBuild.cs`
- Backend connection: `client/Assets/Scripts/BackendConnection/`
- Generated and authored art: `client/Assets/AIGenerated/` and the full Unity asset tree
- Protocol: `gateway.proto` and `client/Assets/Scripts/Protobuf/`
- Navigation: `client/Assets/Scripts/Shared/SceneNavigator.cs` and scene build settings
- State/data layer: `client/Assets/Scripts/Shared/GlobalUserData.cs`, `PlayerPrefs`, protobuf callbacks
- UI system: Unity UI + TextMeshPro + project prefabs; visual asset provenance is recorded in `BUILD_NOTES.zh.md`
- Test/check commands: GitHub Actions workflow; no local Unity Editor on this ARM64 device
- Version source: Unity `ProjectSettings.asset` user version and workflow run number for Android versionCode
- GitHub cloud build: `.github/workflows/android-build.yml`; formatting: `.github/workflows/dotnet-format.yml`
- Backlog: `.codex-app/backlog.md`

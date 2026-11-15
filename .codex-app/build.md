# Build And Verification

## Local Checks Only
- Formatting: repository `.editorconfig` requires C# CRLF; inspect with `git diff` and keep source line endings consistent.
- Static source check: `git diff --check` reports CRLF as trailing whitespace unless Git is configured with `cr-at-eol`; use `git diff --ignore-space-at-eol` and inspect changed lines.
- Unity compile/build: unavailable on this ARM64 Termux device because the Unity Editor version is not installed locally.
- Unit tests: no project test suite was found in the public checkout.

## GitHub Cloud Build
- Repository: `shwiki1/afk_gacha_game`
- Workflow: `.github/workflows/android-build.yml`
- Runner: `ubuntu-22.04` x86_64
- Unity builder: `game-ci/unity-builder@v6`, editor `2022.3.62f3`, Android IL2CPP arm64
- Build method: `AfkAndroidBuild.Build`
- Artifact: `AFKGacha-Android`, APK under `build/**/*.apk`
- Build number: GitHub Actions `github.run_number` becomes Android `versionCode`
- Prior successful run: `43758881628`, versionCode `11`

## Version Management
- User-facing version source: `client/ProjectSettings/ProjectSettings.asset` when the full checkout is present; current value is `1.0`.
- Build number source: workflow `androidVersionCode: ${{ github.run_number }}`.
- Version bump policy: increment the cloud build number for every APK; change user-facing version only for a release change.

## Dependencies And Release Safety
- Unity dependencies: `client/Packages/manifest.json` and `packages-lock.json`.
- Source protocol: `gateway.proto` plus generated protobuf C# files.
- Native networking: project-owned `client/Assets/WebSocket/WebSocket.cs`.
- Release artifact naming: `AFKGacha.apk` inside the `AFKGacha-Android` artifact.
- Prior SHA-256: `a6764cd14366c76eedf28f3ed0099a5ca626d21ac1d6220bcfaa71c776bad575`.
- Do not commit `.env`, signing material, Unity credentials, or asset repository tokens.

## Test Matrix
- Source checks: changed scripts reviewed and endpoint/state transitions inspected.
- Network smoke: WebSocket `101/open` handshake to `ws://156.254.7.63:4001/2` passed.
- CI build: passed, run `43769884864` in `17m24s`.
- APK checks after CI: ABI, versionCode, `unzip -t`, `zipalign`, `apksigner`, and SHA-256.
- Device smoke: pending a connected Android device; install and logcat are required for final launch-loop confirmation.

## Required Secrets
- `UNITY_EMAIL`, `UNITY_PASSWORD`
- Optional private assets: `AFK_ASSETS_REPO`, `AFK_ASSETS_TOKEN`
- Optional release signing: `ANDROID_KEYSTORE_NAME`, `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASS`, `ANDROID_KEYALIAS_NAME`, `ANDROID_KEYALIAS_PASS`

## Notes
- The public working tree is sparse; GitHub Actions checks out the full repository and restores private assets only when configured.
- Use GitHub Actions for native packaging and keep secrets out of commits.
- Current artifact: `构建产物/43769884864/Android/AFKGacha.apk`, versionCode `12`, SHA-256 `120cefb6a02adeba6bd8d30ddb131b43f7954a0dd41ab7d6e60914263949c39f`.

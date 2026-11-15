# Current App State

Last updated: 2026-11-15 22:34 UTC

## Current Truth
- App: afk_gacha_game
- Stack: Unity C# client, NativeWebSocket, Google.Protobuf, Android IL2CPP
- Project version: Unity 2021.3.26f1; CI editor override: Unity 2022.3.62f3
- Package manager: Unity Package Manager (`client/Packages/manifest.json`)
- Local branch: master, based on the upstream checkout
- Build branch: `main` on `shwiki1/afk_gacha_game`
- Cloud build: `.github/workflows/android-build.yml`; formatting workflow: `.github/workflows/dotnet-format.yml`
- App version: `1.0`
- Last completed build number: `11`
- Last completed artifact: `构建产物/43758881628/Android/AFKGacha.apk`

## Active Task
- Fix Android launch-screen flashing by making the server endpoint deterministic and isolating WebSocket startup failures.
- Push the fix to the fork and run a new arm64 Android build.

## Recently Changed
- `client/Assets/Scripts/BackendConnection/ServerSelect.cs`: Android default endpoint is `ws://156.254.7.63:4001`; static defaults and input validation added.
- `client/Assets/Scripts/BackendConnection/SocketConnection.cs`: one-frame startup delay, connection state tracking, URL validation, duplicate protection, and exception-safe close handling added.
- `.codex-app/`: project memory was bootstrapped and is now being filled with verified facts.

## Checks
- WebSocket handshake to `ws://156.254.7.63:4001/2`: passed from the development environment.
- Gateway root `http://156.254.7.63:4001/`: 404, expected because the gateway exposes WebSocket routes rather than an HTML root.
- Local Unity Editor build: unavailable; this Termux device is ARM64 and does not have the project editor installed.
- Real Android launch/logcat: not run; no ADB device is connected.
- GitHub Actions build after this fix: pending.

## Memory Validation
- Source paths, workflow, project version, prior artifact metadata, and remote server endpoint were checked against the repository and live endpoints.

## Risks And Blockers
- The public sparse checkout does not contain every large scene/asset file; the GitHub workflow uses the full checkout and optional private asset restoration.
- A real Android device is still required to confirm that the Unity splash no longer loops and that the cleartext `ws://` transport is accepted by the target Android version.
- Signing remains Android Debug unless the repository's keystore secrets are configured.
- `.env` and local configuration files contain machine-only values and must stay untracked.

## Next Actions
- Commit the source and memory updates on top of the fork's current `main` tree.
- Trigger `.github/workflows/android-build.yml` and wait for the build result.
- Download the new `AFKGacha-Android` artifact and verify versionCode, ABI, zip alignment, signatures, and SHA-256.
- Install on a connected Android device when available and collect logcat if the launch loop persists.

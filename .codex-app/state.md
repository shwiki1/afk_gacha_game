# Current App State

Last updated: 2026-11-16 05:22 UTC

## Current Truth
- App: afk_gacha_game
- Stack: Unity C# client, NativeWebSocket, Google.Protobuf, Android IL2CPP
- Project version: Unity 2021.3.26f1; CI editor override: Unity 2022.3.62f3
- Package manager: Unity Package Manager (`client/Packages/manifest.json`)
- Local branch: master, based on the upstream checkout
- Build branch: `main` on `shwiki1/afk_gacha_game`
- Cloud build: `.github/workflows/android-build.yml`; formatting workflow: `.github/workflows/dotnet-format.yml`
- App version: `1.0`
- Build number: `16`
- Last completed build number: `16`
- Last completed artifact: `构建产物/43806724103/Android/AFKGacha.apk`
- Last artifact SHA-256: `eafb8acae88d854859e06a81057752e563a795eb257bc0c42ae0ce69a2a3a8c7`

## Active Task
- Android startup and black-screen rendering fixes are implemented, built, and verified on the target phone. WebSocket session stability remains a separate follow-up.

## Recently Changed
- `client/Assets/Scripts/BackendConnection/ServerSelect.cs`: Android default endpoint is `ws://156.254.7.63:4001`; static defaults and input validation added.
- `client/Assets/Scripts/BackendConnection/SocketConnection.cs`: one-frame startup delay, connection state tracking, URL validation, duplicate protection, and exception-safe close handling added.
- `client/Assets/Scripts/Shared/RuntimeRenderRepair.cs`: restores zero-scale Canvas transforms and missing Screen Space Camera references after scene load.
- Android path now selects Unity's built-in render pipeline before the first scene because URP threw a device-side NRE; CI uses Unity 2022.3.62f3, URP 14.0.12, and GLES3.
- `.codex-app/`: project memory was bootstrapped and is now being filled with verified facts.

## Checks
- WebSocket handshake to `ws://156.254.7.63:4001/2`: passed from the development environment.
- Gateway root `http://156.254.7.63:4001/`: 404, expected because the gateway exposes WebSocket routes rather than an HTML root.
- Local Unity Editor build: unavailable; this Termux device is ARM64 and does not have the project editor installed.
- Real Android launch/logcat: passed on Xiaomi 2311DRK48C / Android SDK 36 through wireless ADB `192.168.31.227:38399`.
- GitHub Actions build after this fix: passed, run `43806724103` in 9m36s.
- APK structure/signature checks: passed; v1/v2 signatures and `arm64-v8a` verified.
- APK metadata contains `RuntimeRenderRepair` and its diagnostic log string.
- Device screenshot: map/UI visible after launch; `SetupPerFrameShaderConstants` and `NullReferenceException` count was 0.

## Memory Validation
- Source paths, workflow, project version, prior artifact metadata, and remote server endpoint were checked against the repository and live endpoints.

## Risks And Blockers
- The public sparse checkout does not contain every large scene/asset file; the GitHub workflow uses the full checkout and optional private asset restoration.
- The current test log shows the WebSocket session closing after initial startup; the scene remains visible, but server-side login/user creation needs a separate network investigation.
- Signing remains Android Debug unless the repository's keystore secrets are configured.
- `.env` and local configuration files contain machine-only values and must stay untracked.

## Next Actions
- Investigate why the WebSocket session closes after the initial user request on the target device.
- Keep the built-in Android render fallback until a separate URP validation build proves the device-side NRE is gone.

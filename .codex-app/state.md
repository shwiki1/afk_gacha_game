# Current App State

Last updated: 2026-11-15 22:52 UTC

## Current Truth
- App: afk_gacha_game
- Stack: Unity C# client, NativeWebSocket, Google.Protobuf, Android IL2CPP
- Project version: Unity 2021.3.26f1; CI editor override: Unity 2022.3.62f3
- Package manager: Unity Package Manager (`client/Packages/manifest.json`)
- Local branch: master, based on the upstream checkout
- Build branch: `main` on `shwiki1/afk_gacha_game`
- Cloud build: `.github/workflows/android-build.yml`; formatting workflow: `.github/workflows/dotnet-format.yml`
- App version: `1.0`
- Last completed build number: `12`
- Last completed artifact: `构建产物/43769884864/Android/AFKGacha.apk`
- Last artifact SHA-256: `120cefb6a02adeba6bd8d30ddb131b43f7954a0dd41ab7d6e60914263949c39f`

## Active Task
- Android startup fix is implemented and built; only real-device launch/logcat verification remains.

## Recently Changed
- `client/Assets/Scripts/BackendConnection/ServerSelect.cs`: Android default endpoint is `ws://156.254.7.63:4001`; static defaults and input validation added.
- `client/Assets/Scripts/BackendConnection/SocketConnection.cs`: one-frame startup delay, connection state tracking, URL validation, duplicate protection, and exception-safe close handling added.
- `.codex-app/`: project memory was bootstrapped and is now being filled with verified facts.

## Checks
- WebSocket handshake to `ws://156.254.7.63:4001/2`: passed from the development environment.
- Gateway root `http://156.254.7.63:4001/`: 404, expected because the gateway exposes WebSocket routes rather than an HTML root.
- Local Unity Editor build: unavailable; this Termux device is ARM64 and does not have the project editor installed.
- Real Android launch/logcat: not run; no ADB device is connected.
- GitHub Actions build after this fix: passed, run `43769884864`.
- APK structure/signature checks: passed; v1/v2 signatures and `arm64-v8a` verified.

## Memory Validation
- Source paths, workflow, project version, prior artifact metadata, and remote server endpoint were checked against the repository and live endpoints.

## Risks And Blockers
- The public sparse checkout does not contain every large scene/asset file; the GitHub workflow uses the full checkout and optional private asset restoration.
- A real Android device is still required to confirm that the Unity splash no longer loops and that the cleartext `ws://` transport is accepted by the target Android version.
- Signing remains Android Debug unless the repository's keystore secrets are configured.
- `.env` and local configuration files contain machine-only values and must stay untracked.

## Next Actions
- Install the APK on a connected Android device and collect logcat if the launch loop persists.
- If the device rejects cleartext WebSocket traffic, configure a stable `wss://` endpoint or Android network security policy.

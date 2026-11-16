# Backlog

## Ready
- Investigate WebSocket closure after initial user creation on the target device.

## Blocked
- None for rendering; the connected target device has passed the visual smoke test.

## Deferred
- Configure a stable `wss://` domain or Android network security policy if the target device rejects cleartext `ws://`.
- Configure a release keystore and fixed application id before distributing upgrade APKs.
- Validate URP independently before removing the Android built-in render fallback.

## Do Not Forget
- Keep app version/build number updated before every new cloud build.
- Update `.codex-app/state.md` and the latest session handoff after meaningful changes.
- Keep the deployed backend on `156.254.7.63:4001` available while testing the APK.

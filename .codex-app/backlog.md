# Backlog

## Ready
- Verify the resulting APK and install it on a connected Android device.

## Blocked
- Real-device launch/logcat verification is blocked until an Android device is connected over ADB.

## Deferred
- Configure a stable `wss://` domain or Android network security policy if the target device rejects cleartext `ws://`.
- Configure a release keystore and fixed application id before distributing upgrade APKs.

## Do Not Forget
- Keep app version/build number updated before every new cloud build.
- Update `.codex-app/state.md` and the latest session handoff after meaningful changes.
- Keep the deployed backend on `156.254.7.63:4001` available while testing the APK.

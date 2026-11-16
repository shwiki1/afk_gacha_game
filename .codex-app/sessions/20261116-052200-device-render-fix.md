# 2026-11-16 05:22 UTC - Device Render Verification

## Goal
Verify and finish the Android black-screen fix using the user's wireless debugging device.

## Repo Facts Read
- The user's target is Xiaomi `2311DRK48C`, Android SDK 36, 1220x2712.
- Version 13 logs showed repeated `UniversalRenderPipeline.SetupPerFrameShaderConstants` NullReferenceException and Mali Vulkan framebuffer format errors.
- Version 16 includes the Android built-in render fallback, URP/Core/ShaderGraph/VFX 14.0.12 alignment, and GLES3 build configuration.

## Changes Made
- Android build workflow uses Unity 2022.3.62f3, which acquired Personal licensing successfully.
- Upgraded Unity render packages from 12.1.11 to 14.0.12.
- Forced Android graphics APIs to OpenGLES3 in `AfkAndroidBuild`.
- Added `RuntimeRenderRepair.ConfigureAndroidRenderPipeline()` to clear URP defaults before the first scene load on Android.

## Checks Run
- Actions run `43806724103`: passed in 9m36s.
- APK: package `com.DefaultCompany.client`, version `1.0 (16)`, `arm64-v8a`.
- APK SHA-256: `eafb8acae88d854859e06a81057752e563a795eb257bc0c42ae0ce69a2a3a8c7`.
- Wireless ADB: `192.168.31.227:38399` connected.
- Device launch: map background, resource header, server name, and scene buttons visible.
- Device log: `RuntimeRenderRepair selected the built-in Android render pipeline` present.
- Device log: zero matches for `SetupPerFrameShaderConstants`, URP render NRE, and `NullReferenceException`.

## Cloud Build
- Actions run: `43806724103`, succeeded in 9m36s.
- Source commit: `ad67882ff5c54f0dcd51f49f8b1ae6f098f282bd`.
- Artifact: `AFKGacha-Android`.

## Version And Artifacts
- User-facing version: `1.0`.
- Android versionCode: `16`.
- Local artifact: `构建产物/43806724103/Android/AFKGacha.apk`.
- SHA-256: `eafb8acae88d854859e06a81057752e563a795eb257bc0c42ae0ce69a2a3a8c7`.

## Known Risks
- The target log showed the WebSocket connection closing after the initial user request; rendering is fixed but server login/user creation still needs follow-up.
- The built-in Android fallback is intentionally retained until URP is validated on the target device.

## Next Actions
- Investigate the WebSocket close on the target device.
- Keep version 16 as the known-good visual baseline.

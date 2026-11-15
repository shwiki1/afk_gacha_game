# Architecture

## Stack
- Unity 2021.3.26f1 project with a CI build on Unity 2022.3.62f3.
- C# MonoBehaviours, Unity scenes/prefabs, TextMeshPro, URP, Addressables, and Unity Package Manager packages.
- Android player uses IL2CPP and arm64-v8a.

## Runtime And Bridge Boundary
- `SocketConnection` owns the long-lived `NativeWebSocket.WebSocket` and survives scene changes with `DontDestroyOnLoad`.
- `ServerSelect` owns the static logical server name and WebSocket domain.
- The gateway endpoint is `<domain>/2`; the deployed test gateway is `ws://156.254.7.63:4001/2`.
- Protocol payloads are Google.Protobuf messages from `gateway.proto` and generated client classes.

## Module Boundaries
- `Assets/Scripts/BackendConnection`: server selection, WebSocket lifecycle, request/response handlers.
- `Assets/Scripts/Shared`: global user data, currencies, navigation, and shared UI data.
- Feature folders: battle, campaign, dungeon, summon, inventory, lineup, ascension, and AFK rewards.
- `Assets/Editor/AfkAndroidBuild.cs`: deterministic headless Android build entry point.

## Data Flow
- WebSocket open -> `GetUserAndContinue()` -> create or load a user from `PlayerPrefs` -> populate `GlobalUserData`.
- Feature managers build protobuf requests and update scene/UI state from response callbacks.
- Server-side state is PostgreSQL-backed in the deployed `mirra_backend_afk` service.

## Navigation
- `EditorBuildSettings` starts at `Assets/Scenes/Overworld.unity` and includes feature scenes for campaign, battle, summon, inventory, dungeon, and unit management.
- Shared scene objects and prefabs provide navigation and persistent global data.

## Persistence And Networking
- Local user id is stored as `userId_<ServerSelect.Name>` in `PlayerPrefs`.
- The default Android server is the deployed test backend; Unity Editor defaults to localhost.
- Network startup must tolerate unavailable servers and must never throw out of Unity lifecycle callbacks.

## API Contracts And Migrations
- API contract: `gateway.proto` plus the compatible backend branch documented in `BUILD_NOTES.zh.md`.
- No database migration is performed by the client.
- Endpoint changes must be kept in `ServerSelect` and verified with a WebSocket handshake before a CI build.

## Rules Future Agents Must Preserve
- Prefer existing project patterns.
- Do not introduce new architecture layers without recording a decision.
- Verify facts from source files before changing shared structure.
- Keep connection state false until `OnOpen`; clear both `connected` and `connecting` on `OnClose`.
- Do not commit credentials, keystores, or private asset tokens.

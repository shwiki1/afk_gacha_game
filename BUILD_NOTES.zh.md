# AFK Gacha 本地构建说明

## 项目与服务端

- 客户端：`Lambda-Forge/afk_gacha_game`，HEAD `116227e`（2024-06-12），Apache-2.0；音乐/图形按 README 为 CC 署名且相同方式共享。
- 服务端：`lambdaclass/mirra_backend`，Apache-2.0。客户端 `SocketConnection` 启动即连接 `ws://localhost:4001/2`（Editor）或自定义/欧洲测试服，登录、抽卡、养成、战役和挂机奖励都依赖 WebSocket 网关；因此必须有服务端。
- 客户端 `ProjectVersion.txt` 锁定 Unity `2021.3.26f1`。
- 协议基线：使用服务端历史分支 `origin/gh-437-get-dungeon-afk-rewards`（tip `a6d76fd3`），它包含客户端所需的 `GetDungeonAfkRewards`。主分支 HEAD `03ce828` 已删除该请求，且将字段 17 从 `LevelUpItem` 改为 `FuseItems`、字段 6 从 `supply_limit` 改为 `supply_cap`，不能直接与当前客户端混用。历史分支与客户端协议仍存在新增字段差异；未做实机联机验收。

## 源码与素材

- 客户端公开仓库约 1473 个文件，包含脚本、场景、贴图、模型、音频、`Packages` 与大部分 `ProjectSettings`。本机检出为 sparse checkout，只展开了脚本、工程设置、Editor、workflow 目录。
- README 要求自行购买/下载并导入 8 组 Asset Store 素材到 `client/Assets/ThirdParty`；仓库里仅保留了少量目录 `.meta`，`client/ProjectSettings/ProjectSettings.asset` 也被 `.gitignore` 排除。DOTween DLL、TextMesh Pro 资源、Google.Protobuf DLL、NativeWebSocket 已在仓库中。
- 项目根目录提供 `GitHub构建配置模板.txt` 和本地填写文件 `GitHub构建配置.local.txt`，可以直接用记事本修改。大型 Asset Store 素材仓库现在是可选项；没有素材仓库时 workflow 先使用公开客户端源码尝试构建。
- 直接拿公开仓库在 CI 编译，不等于已经完成美术和联机验收；当前公开源码已经可以在 CI 生成可安装 APK，但仍有缺失场景脚本告警、Asset Store 素材未补齐和服务端未联机验证。
- 本次加入的 `client/Assets/AIGenerated/` 是项目内生成的资源：属性/货币图标与道具图标为 `512x512` RGBA PNG，进度条框为 `1024x256` RGBA PNG。原图由本机 `次元游戏社/art/` 生图渠道生成，再按 `API.md` 的 `/remove` 抠图接口处理并裁剪；仓库不保存 API 密钥。

## CLI、许可与构建

- 本机 Unity CLI `1.0.0` 已安装；`~/.local/bin/unity` 是通过 `grun` 调用官方 ARM64 二进制的 Termux 启动脚本。`unity --version` 已实测返回 `1.0.0`。
- `unity doctor` 在这台 Linux arm64 手机上把 `licensingClient` 标成 `unsupported-platform`；`license status` 返回 `LICENSING_CLIENT_UNAVAILABLE`，未登录。`editors info 6000.6.9f1 --architecture arm64` 的 `downloadUrl` 为 `null`，Editor 列表为空。因此本机没有完成 Unity Editor / 许可安装；Unity 许可和 APK 构建改由 x86_64 GitHub Actions runner 完成。
- Unity Personal 对最近 12 个月总收入与融资低于 20 万美元的个人/小组织免费；超过门槛须用付费方案。当前 GameCI CLI 支持用 `UNITY_EMAIL` + `UNITY_PASSWORD` 在 x86_64 runner 上直接申请 Personal seat，不需要把新版 `UnityEntitlementLicense.xml` 当作旧式 `.ulf` 使用。
- 构建 workflow 与 Unity 静态构建入口已经写入本地工作区：
  - `.github/workflows/android-build.yml`
  - `client/Assets/Editor/AfkAndroidBuild.cs`
- x86_64 Linux 的 GitHub Actions runner 使用 `game-ci/unity-builder@v6`、Unity `2022.3.62f3` 和 Android 模块构建 APK；官方新 CLI 的 `unity ci init` 生成的 workflow 需要 Unity 服务账号和许可序列号，GameCI workflow 更适合免费 Personal 许可。为解决目标 Android 设备的 URP 运行时异常，URP/Core/ShaderGraph/VFX 已对齐到 `14.0.12`，Android 构建强制使用 GLES3。
- GitHub 构建至少需要 `UNITY_EMAIL`、`UNITY_PASSWORD`。素材仓库存在时再配置 `AFK_ASSETS_REPO`、`AFK_ASSETS_TOKEN`；`game-ci/unity-builder@v6` 使用当前 `game-ci/cli` 在 runner 上申请 Personal seat 并在构建结束时归还。如果要生成可覆盖安装的 release APK，再配置 Android keystore 字段。
- 以后配置私有素材仓库时，根目录必须包含 `Assets/ThirdParty` 和 `ProjectSettings/ProjectSettings.asset`，还要包含 README 要求的其他合法 Asset Store 导入文件。大型素材不要放进 Secret，Token 只需要该私有仓库的 `Contents: Read` 权限。
- 2026-11-15 在 fork 上实测：Android workflow 能启动；旧的 `game-ci/unity-request-activation-file@v2` 已实测返回“不再支持”，所以已删除该 activation workflow。远端 Ubuntu 24.04 x86_64 已实测 Personal 激活成功并读回，随后已执行 `license return` 和 `auth logout` 释放席位；本机 ARM64 CLI 仍不支持 licensing client。
- 公开源码首次编译还缺少 Asset Store 的 `DuloGames.UIProgressBar` 类型；项目只使用其 `fillAmount`。已加入 `client/Assets/Scripts/ThirdPartyCompat/DuloGamesCompat.cs` 作为独立编写的最小编译兼容层，没有复制 Asset Store 源码；后续可以用 AI 生成的 UI 图形替换进度条外观。
- 验收状态：GitHub Actions 已完成 Personal 激活、Unity IL2CPP 编译、ARM64 APK 归档和本地 APK 校验；目标 Xiaomi 2311DRK48C 已实机验证 versionCode 16 能显示地图/UI，且不再出现 URP NRE。当前 APK 使用默认包名 `com.DefaultCompany.client`、版本 `1.0 (16)`、`arm64-v8a`，并由 Android Debug 证书签名；要做可覆盖升级的正式包，需要配置固定包名、版本号和 Android keystore。WebSocket 登录链仍需单独排查。

## 本次构建记录

- GitHub fork：`https://github.com/shwiki1/afk_gacha_game`
- 提交：`66fa304515dded22a5a6692ee81f333f9f377de9`
- Actions：`https://github.com/shwiki1/afk_gacha_game/actions/runs/43758881628`
- Artifact：`AFKGacha-Android`，约 `106,803,232` bytes；本地文件为 `构建产物/43758881628/Android/AFKGacha.apk`。
- APK：`arm64-v8a`、IL2CPP、versionCode `11`；SHA-256：`a6764cd14366c76eedf28f3ed0099a5ca626d21ac1d6220bcfaa71c776bad575`
- 校验：`unzip -t`、`zipalign -c -v 4`、`apksigner verify` 均通过；签名方案为 v1/v2。

## 相关链接

- 客户端：https://github.com/Lambda-Forge/afk_gacha_game
- 服务端：https://github.com/lambdaclass/mirra_backend
- Unity Personal：https://unity.com/products/unity-personal
- Unity 软件条款：https://unity.com/legal/editor-terms-of-service/software
- Unity CLI 安装脚本：https://unity.com/install.sh
- Unity 技能：https://github.com/Unity-Technologies/skills/blob/main/skills/new-unity-project/SKILL.md
- GameCI 许可：https://game.ci/docs/github/activation
- GameCI builder：https://game.ci/docs/github/builder

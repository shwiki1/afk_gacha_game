# Third-Party Notices

This file records third-party components used by the GitHub Actions build
workflow. The Unity project remains licensed under the repository `LICENSE`;
the workflow actions run in CI and are not copied into the Android player.

## Generated UI and resource images

- Version: generated 2026-11-15
- License: provider terms apply; no third-party source image files were copied into this repository
- Used as: project UI, currency, attribute, and item PNG assets under `client/Assets/AIGenerated`
- Upstream: local generation channel documented in the shared `次元游戏社/art/API.md`
- Source for distributed binary: generated PNG files in this repository
- Modifications: resized and processed with the documented `/remove` background-removal API; no API credentials are stored here
- Required notices: verify the generation provider's commercial-use terms before commercial distribution

## Local DuloGames compatibility shim

- Version: repository source at `client/Assets/Scripts/ThirdPartyCompat/DuloGamesCompat.cs`
- License: repository Apache-2.0 for the independently authored shim
- Used as: source compatibility layer for the single `DuloGames.UIProgressBar.fillAmount` API used by the public scripts
- Upstream: https://assetstore.unity.com/packages/2d/gui/rpg-mmo-ui-6-99450
- Source for distributed binary: this repository's shim source
- Modifications: none to the Asset Store package; no Asset Store source code is copied or bundled
- Required notices: obtain the Asset Store package separately if its original visuals or other components are needed

## GameCI Unity Builder

- Version: `v6`
- License: MIT
- Used as: GitHub Actions build action
- Upstream: https://github.com/game-ci/unity-builder
- Source for distributed binary: https://github.com/game-ci/unity-builder/tree/v6
- Modifications: none
- Required notices: https://github.com/game-ci/unity-builder/blob/v6/LICENSE

## GitHub Actions checkout

- Version: `v4`
- License: MIT
- Used as: GitHub Actions source checkout action
- Upstream: https://github.com/actions/checkout
- Source for distributed binary: https://github.com/actions/checkout/tree/v4
- Modifications: none
- Required notices: https://github.com/actions/checkout/blob/v4/LICENSE

## GitHub Actions cache

- Version: `v4`
- License: MIT
- Used as: GitHub Actions Unity Library cache action
- Upstream: https://github.com/actions/cache
- Source for distributed binary: https://github.com/actions/cache/tree/v4
- Modifications: none
- Required notices: https://github.com/actions/cache/blob/v4/LICENSE

## GitHub Actions upload-artifact

- Version: `v4`
- License: MIT
- Used as: GitHub Actions APK and test artifact upload action
- Upstream: https://github.com/actions/upload-artifact
- Source for distributed binary: https://github.com/actions/upload-artifact/tree/v4
- Modifications: none
- Required notices: https://github.com/actions/upload-artifact/blob/v4/LICENSE

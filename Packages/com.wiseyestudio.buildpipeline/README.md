# WiseyeStudio Build Pipeline

ScriptableObject-based batch build pipeline. A `BuildPipelineScriptableObject` asset defines a build (target group, build name, version code, scene list) plus ordered lists of pre-build, post-build and after-everything build methods. Build methods are composable ScriptableObject assets; projects add their own by extending `ABuildPipelineBuildMethodScriptableObject`.

## Contents

- `BuildPipelineScriptableObject` — the pipeline definition and build runner
- `BuildMethod/` — built-in build methods: scene list, define symbols, version code, DSP buffer size, Android keystore, build-target switch, Addressables (build/clean/group include/exclude), zip/Mac compression, DoNotShip folder cleanup, copy-to-directory, Steam upload (shell script / batch), Apple notarize + entitlement signing
- `Apple/`, `Steam/` — account-data assets for upload/signing methods (keep the actual assets with secrets in your project, NOT in this package)
- `DataProvider/` — editor data providers for Addressables groups

## Prerequisites (not auto-resolved by UPM)

- **com.wiseyestudio.core** — install first
- **UniTask** — git package (`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`)
- **Odin Inspector** — paid asset, must be present in the project

`com.unity.addressables` and `com.unity.nuget.newtonsoft-json` are declared as normal dependencies and resolve automatically.

## Project-side setup

- Create pipeline and build-method assets in your project (`Create > WiseyeStudio > Build`); the assets (and any secrets like Steam/Apple account data) stay in the project.
- Project-specific build methods: extend `ABuildPipelineBuildMethodScriptableObject` in your project's editor assembly.
- Batch-mode entry points: keep a thin static wrapper class in the project that loads your pipeline asset and runs it (see Lily's `StaticBuildMethods`).

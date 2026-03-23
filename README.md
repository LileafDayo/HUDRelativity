# HUDRelativity

A Unity (URP) project that simulates the **1st-person experience of traveling at relativistic speeds** (as a fraction of the speed of light), with the goal of deepening intuitive understanding of **special relativity**.

This project focuses on:
- **Relativistic aberration** (apparent direction/shape changes) via runtime mesh vertex warping
- A **URP post-processing Doppler-style color/shift effect** driven by a “beta” (v/c) parameter
- A simple HUD for displaying and adjusting the simulation’s relative velocity

> Note: The repository’s main content lives in `Assets/`. Most scene objects are Blender models, while the simulation logic is primarily in `Assets/Scripts/`.

## My contributions

I contributed by:
- creating the relativity simulation script(s),
- building the `Assets/Scenes/LondonStreet.unity` scene (including the looping behavior),
- calibrating/tuning the project as a whole (visuals, controls, and effect parameters).

## Scene / entry point

The most comprehensive scene is:
- `Assets/Scenes/LondonStreet.unity` — a looping 1st-person scene intended as the primary demo.

(There may be other scenes such as `Assets/SampleScene.unity`, but `LondonStreet.unity` is the recommended starting point.)

## Project structure (high level)

- `Assets/`
  - `Scenes/` (Unity scenes)
  - `Objects/`, `Buildings_*`, etc. (models and scene content; many imported from Blender)
  - `Scripts/` (**core simulation scripts**)
- `Packages/` + `ProjectSettings/` (Unity package + project configuration)

## Requirements

- **Unity** with **Universal Render Pipeline (URP)** enabled (this project includes URP settings/assets).
- A GPU/graphics backend supported by your Unity + URP version.

## Getting started

1. **Clone** the repo.
2. Open the project folder in **Unity Hub** (select the repo root).
3. In Unity, open:
   - `Assets/Scenes/LondonStreet.unity`
4. Press **Play**.

## Controls (current behavior in scripts)

### Change relative velocity (beta = v/c)

`beta` is adjusted in **`BetaText`** using the Vertical axis:

- **Up/Down Arrow** (or `W/S`, depending on Unity Input settings) modifies beta.
- Beta is intentionally restricted to stay below light speed and non-negative:
  - max ≈ `0.9999`
  - min ≈ `0.01`

This beta value is shared with other scripts via a GameObject tagged **`BetaText`**.

### Camera look

`chasecam.cs` uses the legacy input axes:
- Mouse movement (`Mouse X`, `Mouse Y`) rotates the camera

## How the simulation works (scripts overview)

All scripts are in: `Assets/Scripts/`

### Main “beta” / HUD plumbing

- **`BetaText.cs`**
  - Displays current relative velocity on the HUD.
  - Updates `DopplerEffectVolume.beta` if available.
  - Exposes `public float beta` used by other scripts.
  - Depends on a GameObject tagged **`BetaText`**.

- **`BetaController.cs`** (class name `Controller`)
  - Reads `BetaText.beta` and pushes it into the URP volume stack (`DopplerEffectVolume`).
  - Also computes intensity as `Clamp01(|beta| * 2)`.

> Implementation note: both `BetaText` and `BetaController` attempt to drive the volume beta; you may want to keep only one “source of truth”.

### Relativistic aberration (mesh deformation)

- **`AberrationAngle.cs`**
  - Reads beta from the `BetaText` tag.
  - Warps the attached object’s mesh **every frame** by modifying vertices based on aberration equations.
  - Applies object scale/rotation handling to keep deformation consistent in world space.

This script should be attached to objects whose **MeshFilter mesh** you want to distort.

### URP post-processing: “Relativistic Doppler”

This project defines a custom URP Volume + renderer feature(s):

- **`DopplerEffectVolume.cs`**
  - Defines a URP Volume component:
    - `beta` in `[-0.99, 0.99]`
    - `intensity` in `[0, 5]`
  - Effect is active when `intensity > 0`

- **`DopplerEffectFeature.cs`**
  - A `ScriptableRendererFeature` that blits using shader `"Hidden/RelativisticDoppler"`.
  - Pass event: `BeforeRenderingPostProcessing`
  - Reads beta + intensity from `DopplerEffectVolume`.

- **`DopplerEffectRenderer.cs`**
  - Another `ScriptableRendererFeature` using shader `"Custom/DopplerPostProcess"`.

> You likely only need **one** renderer feature wired into your URP Renderer asset. If both exist, decide which shader/implementation you want and remove/disable the other.

### Motion / scene helpers

- **`MoveObject.cs`** (class name `MoveBox`)
  - Moves an object along X based on beta and time.
  - Uses a GameObject tagged **`Endline`** as a boundary/reference.

- **`chasecam.cs`**
  - Camera rotation via mouse axes.

- **`camtext.cs`**
  - Updates a HUD text element with “Camera sensitivity” (currently a standalone value; not yet hooked into `chasecam`).

- **`lorentzslider.cs`**
  - A UI slider-driven beta target with smoothing.
  - Computes gamma (Lorentz factor) (current formula is `gamma = 1/sqrt(1-beta)`; verify intended math if you’re using this for physics/education).

### Debug / scratch scripts

- **`WhatIsThisMesh.cs`**: Logs mesh vertices + object scale (debugging).
- **`HowDoIDoThis.cs`**: Experimental transform logic / prototype.
- **`Loop.cs`**: Incomplete placeholder (won’t compile as-is due to `private` without a type).

## Setup notes (important tags / scene wiring)

Several scripts rely on **Unity Tags** and scene objects being present:

- Tag: **`BetaText`**
  - Must exist on the GameObject that has `BetaText.cs`.

- Tag: **`Endline`**
  - Used by `MoveBox` (`MoveObject.cs`) to determine a boundary/end position.

URP / Volume requirements:
- You need a **Volume** in the scene (Global or local) with **`Relativistic Doppler`** (`DopplerEffectVolume`) added.
- The URP Renderer must include the selected **Renderer Feature** (`DopplerEffectFeature` *or* `DopplerEffectRenderer`).

## Credits

This was a 3rd-year university group project. Group members are kept anonymous, but their help and contributions were essential to the result.

## Known issues / TODO

- The repo currently includes Unity-generated folders/files (for example `Library/`, `Temp/`, `.DS_Store`). Consider ignoring these for cleaner version control.
- `Loop.cs` is incomplete and may break compilation.
- `BetaController` class is named `Controller` (file/class mismatch can be confusing).
- Multiple systems appear to set `DopplerEffectVolume.beta`. Pick one to avoid fighting updates.
- Some scripts use `GameObject.FindGameObjectWithTag()` every frame, which can be expensive; consider caching references.

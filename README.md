# HUDRelativity

A Unity (URP) project that simulates the **1st-person experience of traveling at relativistic speeds** (as a fraction of the speed of light), with the goal of deepening intuitive understanding of **special relativity**.

This project focuses on:
- **Relativistic aberration** (apparent direction/shape changes) via runtime mesh vertex warping
- A **URP post-processing Doppler-style color/shift effect** driven by a “beta” (v/c) parameter
- A simple HUD for displaying and adjusting the simulation’s relative velocity

> Note: The repository’s main content lives in `Assets/`. Most scene objects are Blender models, while the simulation logic is primarily in `Assets/Scripts/`.

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
  - A `ScriptableRendererFeature` that blits using shader `
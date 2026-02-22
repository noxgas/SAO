# VR Swordsman Prototype – Test Setup Guide

> **Short answer:** Yes — the code is ready. Follow the steps below to get into a playable state in Unity, either on desktop (no headset needed) or on a Meta Quest 2.

---

## What's in the prototype

| System | Status | File |
|---|---|---|
| Arm-swing locomotion (peak speed + power curve) | ✅ | `VRMovementSystem.cs` |
| Physics sword (one-handed & two-handed) | ✅ | `VRSwordController.cs` |
| Procedural terrain (2-mile map, biomes, resources) | ✅ | `ProceduralMapGenerator.cs` |
| Enemy AI (patrol → chase → attack with cooldown) | ✅ | `AIEnemy.cs` |
| Enemy spawner | ✅ | `EnemySpawner.cs` |
| World-space health/stamina display | ✅ | `VRHealthDisplay.cs` |
| Desktop test mode (WASD + mouse, no headset) | ✅ | `VRPrototypeTestMode.cs` |
| Quest 2 controller mapping (grip/trigger/buttons) | ✅ | `Quest2InputHandler.cs` |

---

## Option A — Desktop Test (no headset required)

### 1. Unity version
Unity **2021.3 LTS** or newer (2022.3 LTS recommended).

### 2. Required packages (install via Package Manager)
- **XR Interaction Toolkit** – `com.unity.xr.interaction.toolkit`
- **TextMeshPro** – `com.unity.textmeshpro`

### 3. Scene setup
1. Create a new Scene.
2. Add an empty **Player** GameObject at the origin. Add these components:
   - `CharacterController` (height 1.8, radius 0.3)
   - `VRMovementSystem` — tick **Desktop Test Mode** ✅
   - `VRPrototypeTestMode`
3. Add a child Camera to the Player (name it `Head Camera`).
4. Add a **Sword** GameObject anywhere in the scene:
   - `Rigidbody` (Is Kinematic = false)
   - `BoxCollider` (Is Trigger = true) — covers the blade
   - `VRSwordController` — tick **Desktop Test Mode** ✅
5. Add an **EnemySpawner** GameObject anywhere:
   - `EnemySpawner` component
   - Assign a simple capsule prefab (with `AIEnemy` component) as the **Enemy Prefab**
6. Add a **ProceduralMapGenerator** GameObject:
   - `ProceduralMapGenerator` component
   - Player Reference → your Player GameObject
   - Leave tree/rock prefabs blank for the first test (terrain still generates)
7. Hit **Play**. The on-screen HUD appears automatically.

### 4. Desktop controls

| Key | Action |
|---|---|
| WASD / Arrow keys | Move |
| Mouse | Look left/right |
| **E** | Grab / release nearest sword |
| **Left Mouse Button** (held) | Swing sword (activates hit detection) |
| **R** | Despawn all enemies (they respawn) |
| **Tab** | Toggle debug overlay |
| **Esc** | Unlock cursor |

---

## Option B — Meta Quest 2

### 1. Unity setup
1. Install **OpenXR Plugin** – `com.unity.xr.openxr`  
   *Project Settings → XR Plug-in Management → Android → OpenXR*
2. Add **Meta Quest feature set** in OpenXR settings.
3. Set build target to **Android**, Texture Compression **ASTC**.

### 2. Scene setup (same as desktop, plus)
- Assign an **XR Rig** (from XR Interaction Toolkit) as the Player root.
- Assign **Left/Right Controller** transforms to `VRMovementSystem`.
- Add `Quest2InputHandler` to the XR Rig root.
- Assign `rightHandSword`, `leftHandSword`, and hand transforms in the Inspector.

### 3. Quest 2 controls

| Input | Action |
|---|---|
| Right Grip (squeeze) | Grab sword – right hand |
| Left Grip (squeeze) | Grab sword – left hand / two-handed mode |
| Right Trigger | Basic slash skill |
| Left Trigger | Block |
| **A** button | Toggle debug overlay |
| **B** button | Respawn enemies |
| **Y** button | Open holographic menu |
| Left Thumbstick ←/→ | Snap turn (45°) |
| Arm swing | Locomotion (harder swing = faster movement) |

---

## Things to verify on first test

- [ ] Walking around the terrain (arm swing on Quest 2, WASD on desktop)
- [ ] Swing speed bar in the HUD responds to arm intensity
- [ ] Grabbing a sword (E / Right Grip)
- [ ] Hitting an enemy with the sword
- [ ] Enemy chases the player and deals damage at 1-second intervals
- [ ] Enemy despawns on death; new enemies spawn
- [ ] Terrain chunks load/unload as you move

---

## Known limitations (acceptable for prototype)

- **No VR hand renders** – hands are invisible on Quest 2; controller positions still drive movement and grabbing.
- **Tree/rock prefabs not included** – create simple cylinder/cube prefabs and assign them to `ProceduralMapGenerator` to see resource placement.
- **Enemy prefab not included** – create a Capsule with `AIEnemy`, a `Rigidbody`, and a `CapsuleCollider`; assign to `EnemySpawner`.
- **Sword prefab not included** – create a Cube (scaled tall and thin) with `Rigidbody`, `BoxCollider` (trigger), and `VRSwordController`.
- **SteamVR files** (`VRMenuSystem`, `VRInputManager`) still reference `Valve.VR` – they are pre-existing files and do not affect the prototype unless you add them to the test scene.

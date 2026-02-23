# Unity Setup Guide — VR Swordsman Prototype

This guide tells you **exactly what to install and configure** in Unity so the prototype compiles and runs — either on desktop or on a Meta Quest 2.

---

## 1 · Unity version

| Requirement | Value |
|---|---|
| **Minimum** | Unity **2021.3 LTS** |
| **Recommended** | Unity **2022.3 LTS** |
| **Not yet tested** | Unity 6 (should work; uses a `#if UNITY_6000_0_OR_NEWER` fallback for `Rigidbody.velocity`) |

Download Unity Hub → [https://unity.com/download](https://unity.com/download)  
In Hub, install a **2022.3 LTS** editor with the **Android Build Support** module (needed for Quest 2).

---

## 2 · Required Unity packages

Open **Window → Package Manager** and install these packages.

### 2a. TextMesh Pro  *(already bundled, just import)*

`com.unity.textmeshpro`

1. Package Manager → search **TextMeshPro** → Install  
2. When Unity asks *"Import TMP Essential Resources"* → click **Import**  
   *(This adds fonts and shaders to your project; the HUD and menu will not render without it.)*

---

### 2b. XR Interaction Toolkit

`com.unity.xr.interaction.toolkit`

1. Package Manager → click the **+** button → **Add package by name**
2. Paste: `com.unity.xr.interaction.toolkit`
3. Version **2.5.x** or newer → Install
4. Also install the **Starter Assets** sample that appears in the package page  
   *(This gives you a ready-made XR Rig prefab with the right structure.)*

---

### 2c. OpenXR Plugin  *(Quest 2 only — skip for desktop)*

`com.unity.xr.openxr`

1. Package Manager → **Add package by name** → `com.unity.xr.openxr` → Install
2. When the restart prompt appears → **Restart**

---

## 3 · Project Settings

### 3a. Enable XR (both desktop preview and Quest 2)

**Edit → Project Settings → XR Plug-in Management**

| Tab | Setting |
|---|---|
| **PC, Mac & Linux** | Tick ☑ **OpenXR** |
| **Android** | Tick ☑ **OpenXR** |

After ticking OpenXR on each platform, click the small **⚠ yellow warning icon** that appears — it will prompt you to fix required settings automatically. Click **Fix All**.

#### On the Android tab, click the OpenXR gear icon and add these Feature Groups:

- **Meta Quest support** (adds the Quest 2 profile)
- **Hand Tracking** (optional — not used by current code but useful later)

---

### 3b. Build Settings for Quest 2

**File → Build Settings**

1. Select **Android** → **Switch Platform** (takes a minute)
2. Tick ☑ **Build App Bundle (Google Play)** — leave **unchecked** for sideloading
3. Click **Player Settings…**:

| Setting | Value |
|---|---|
| Company Name | (your name) |
| Product Name | SAO Prototype |
| Minimum API Level | **Android 10.0 (API 29)** |
| Target API Level | **Android 12L (API 32)** or Automatic |
| Texture Compression | **ASTC** |
| Color Space | **Linear** |
| Scripting Backend | **IL2CPP** |
| Target Architectures | ☑ **ARM64** only |

---

### 3c. Graphics Settings

**Edit → Project Settings → Graphics**

- **Rendering Path**: Forward  
- For Quest 2 performance: **Edit → Project Settings → Quality** → set Default quality level to **Low** for Android

---

## 4 · Create the three required prefabs

The scripts are ready but there are no art assets. Create these simple stand-in prefabs:

### 4a. Sword Prefab

1. **GameObject → 3D Object → Cube** — rename to `Sword`
2. Set Transform **Scale** to `(0.05, 1.0, 0.05)` (tall thin blade)
3. Add components:
   - `Rigidbody` — set **Is Kinematic = false**, **Use Gravity = true**
   - `Box Collider` — tick **Is Trigger = true** (covers the blade for hit detection)
   - `VRSwordController` — set **Sword Type = One Handed**; tick **Desktop Test Mode = ✅** for PC testing
4. Drag it from Hierarchy into your **Assets** folder to make it a prefab → delete from scene

---

### 4b. Enemy Prefab

1. **GameObject → 3D Object → Capsule** — rename to `Enemy`
2. Add components:
   - `Rigidbody` — set **Is Kinematic = false**, enable **Freeze Rotation X Z**
   - `Capsule Collider` (already added by default)
   - `AIEnemy` — default values are fine for first test
3. Drag into **Assets** → delete from scene

---

### 4c. Player Root (scene object, not a prefab)

The Player is assembled in the scene — see Section 5.

---

## 5 · Assemble the test scene

### 5a. Desktop test (no headset)

1. **File → New Scene** (Basic or Empty)
2. Delete the default `Directional Light` and `Main Camera`

#### Player

3. **GameObject → Create Empty** — name it `Player` — position `(0, 0, 0)`
4. Add components to **Player**:

| Component | Key settings |
|---|---|
| `Character Controller` | Height `1.8`, Radius `0.3`, Skin Width `0.08` |
| `VRMovementSystem` | **Desktop Test Mode ☑** |
| `VRPrototypeTestMode` | (no settings needed) |
| `SAOHUDController` | (no settings needed — auto-builds HUD at runtime) |
| `SAOMenuController` | (no settings needed — auto-builds menu at runtime) |
| `Player` | Level `1` |
| `PlayerCombat` | Leave defaults |

5. **Right-click Player → Create Empty Child** — name it `Head Camera`  
   Set local position `(0, 1.6, 0)`
6. Add `Camera` component to `Head Camera` — set **Tag = MainCamera**

#### Sword

7. Drag your **Sword prefab** into the scene — position anywhere near `(0, 1, 1)`

#### Enemy Spawner

8. **Create Empty** — name it `EnemySpawner`
9. Add `EnemySpawner` component
10. Set **Enemy Prefab** → drag your Enemy prefab in

#### Terrain

11. **Create Empty** — name it `MapGenerator`
12. Add `ProceduralMapGenerator` component
    - **Player Reference** → drag `Player` in (or leave blank — auto-finds Camera.main)
    - Leave Tree/Rock Prefabs blank for first test

#### Lighting (minimum)

13. **GameObject → Light → Directional Light** — leave defaults

14. **Hit Play** ▶  
    - Terrain generates, player auto-snaps to surface  
    - HUD appears bottom-left  
    - Press **M** to open the SAO menu  
    - Press **E** near the sword to grab it  
    - Hold **Left Mouse Button** to swing

---

### 5b. Quest 2 test

1. Follow all of Section 5a (creates the scene structure)
2. **Delete** the `Player` root you made
3. **Drag the XR Rig prefab** from  
   `Assets/Samples/XR Interaction Toolkit/2.5.x/Starter Assets/Prefabs/XR Origin (XR Rig).prefab`  
   into the scene at position `(0, 0, 0)`
4. Add these components **to the XR Origin root**:

| Component | Key settings |
|---|---|
| `VRMovementSystem` | Left Controller = `LeftHand Controller` transform; Right = `RightHand Controller` |
| `SAOHUDController` | leave blank |
| `SAOMenuController` | leave blank |
| `Player` | Level `1` |
| `PlayerCombat` | leave defaults |
| `Quest2InputHandler` | right/left sword + transforms auto-found OR drag them in |

5. The `Head Camera` is already present in the XR Rig as `Main Camera` — ensure its Tag is `MainCamera`
6. Add `EnemySpawner` and `ProceduralMapGenerator` as in desktop steps 8–12

---

## 6 · Build and deploy to Quest 2

1. **File → Build Settings → Android** — ensure your scene is in the list
2. Connect Quest 2 via USB and enable **Developer Mode** on the headset  
   *(Meta Developer account + Oculus app on phone required once)*
3. Click **Build and Run**  
   Unity compiles and sideloads the APK directly to the headset (~3–5 min first time)

> **Tip:** Use **Meta Quest Developer Hub** (free download) for easier device management and log viewing.

---

## 7 · Package summary (quick reference)

| Package | Where to get | Required for |
|---|---|---|
| **TextMesh Pro** | Package Manager (bundled) | All UI text — HUD, menu |
| **XR Interaction Toolkit 2.5+** | Package Manager | XR Rig, controller tracking |
| **OpenXR Plugin** | Package Manager | Quest 2 controller input |
| **Android Build Support** | Unity Hub (module) | Building APK for Quest 2 |

**You do NOT need:**
- SteamVR / Valve SDK (pre-existing SteamVR files in the repo are disabled with `#if STEAMVR_PRESENT` guards and will not cause compile errors)
- Meta/Oculus SDK (the code uses Unity's built-in `UnityEngine.XR` API)
- Any paid Asset Store packages

---

## 8 · Troubleshooting

| Error | Fix |
|---|---|
| `The type or namespace 'TMPro' could not be found` | Install TextMeshPro + import TMP Essential Resources |
| `The type or namespace 'XR' does not exist` | Install XR Interaction Toolkit |
| `Unable to find any OpenXR runtime` | Enable OpenXR in Project Settings → XR Plug-in Management |
| Player falls through the floor | `ProceduralMapGenerator` is not in the scene, OR wait 2 frames for terrain collider to register |
| No terrain visible | Player starts at the correct origin `(0,0,0)` — the map is centred on that point; if you moved the player, set it back to `(0,0,0)` before pressing Play |
| Quest 2: controllers not tracking | Check XR Plug-in Management → Android → OpenXR is enabled and Meta Quest feature set is added |
| Quest 2: APK installs but crashes | Minimum API level must be 29+; Scripting Backend must be IL2CPP; Architecture must include ARM64 |
| `SteamVR_Input` errors | These files are now guarded — if you still see them, ensure the `#if STEAMVR_PRESENT` is on line 1 of each affected file |

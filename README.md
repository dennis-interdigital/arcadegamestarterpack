# Arcade Game Starter Pack

A Unity C# starter pack for arcade-style game projects. 

---

## Requirements

| | |
|---|---|
| **Unity Version** | 2022.3.57f1 (LTS) |
| **Render Pipeline** | Built-in |
| **Scripting Backend** | Mono / IL2CPP |
| **Target Platform** | Mobile (Android primary), expandable |

---

## How to Import

1. Create a new Unity project (2022.3 LTS or higher are recommended).
2. Open **Assets → Import Package → Custom Package**.
3. Navigate to `Assets/Package/ProjectStarterPack.unitypackage` and import all.
4. Follow the **First-Time Setup** section below.

---

## First-Time Setup

### 1. Rename the Namespace

Every script uses the placeholder namespace `EditYourNameSpace`.  
Replace it globally before writing any game code.

- **Visual Studio:** `Ctrl + R, Ctrl + R` on any occurrence, check *Rename in all files*.
- **Rider:** `Shift + F6` on any occurrence.
- **Visual Studio Code:** `F2` on any occurence (or Right click -> Rename Symbol).
- Search string: `EditYourNameSpace`
- Replace with: your game's namespace, e.g. `MyGame`

### 2. Rename the Assembly Definition

Open `Assets/Game/Scripts/edityournamespace.asmdef` in a text editor and change:
- `"name"` → match your namespace, e.g. `"MyGame"`
- `"rootNamespace"` → same value

Rename the `.asmdef` file itself to match (e.g. `MyGame.asmdef`).

### 3. Register Your Scene

Add your main scene to Unity's build settings:
**File → Build Settings → Add Scene Main**

### 4. Enable the Debug Overlay (Optional)

The debug overlay is stripped from builds by default.  
To enable it in the editor:

**InterDigital → Toggle USE_DEBUG**

This adds the `USE_DEBUG` scripting define to the active build target.  
Toggle it off before making a release build.

---

## Architecture

### GameManager

`GameManager` is the entry point. It owns the game loop and holds references to all managers.

- `Awake` — sets frame rate, initializes `CoroutineCache`
- `Start` — loads save data, creates plain C# managers, then calls `InitManagers()`
- `FixedUpdate` — manually drives `StageManager.DoUpdate` and `UIManager.DoUpdate`

**To add a new manager:**
1. Go to Assets/Game/Scripts/Managers. Create a new script
2. Add a `public MyManager myManager;` use MonoBehavior if you intent to hold GameObjects (mainly for gameplay scripts) or declare and `new` it in `Start` (plain C# class).
3. Call `myManager.Init(this)` inside `InitManagers()`.
4. Call `myManager.DoUpdate(dt)` inside `FixedUpdate` if it needs a tick.

### StageManager

The gateway to all gameplay objects and logic. Receives `GameManager` via `Init`.  
Add your gameplay systems, spawners, and game-state logic here.

### UIManager

Manages all UIs and popups.

- Only one **UI** screen is active at a time. Switching hides the current one first.
- Multiple **Popups** can be active simultaneously. Show/hide calls are queued to prevent animation overlap.

**To add a new UIs or Popups:**
1. Go to Assets/Game/Scripts/UI for UI scripts or /Popup for popups.
2. Create a class extending `BaseUI`. Naming convention: must include `UI` after the name. (e.g. `MainMenuUI`) or `Popup` for popups (e.g. `SettingsPopup`)
3. Add its value to `UIState` (or `PopupState` for popups).
4. Serialize the prefab reference on the `UIManager` inspector.
5. Register it in `UIManager.Init` with `uiBlueprintDict.Add(...)` (or `popupBlueprintDict.Add(...)` for popup).

**BaseUI lifecycle:**
```
Init(gameManager)   ← wiring only, no animation
    ↓
Showing(payload)    ← coroutine, play open animation here
    ↓
[active]
    ↓
Hiding()            ← coroutine, play close animation here
```

Any DOTween animation inside `Showing` or `Hiding` must be `yield return`-ed  
(e.g. `yield return tween.WaitForCompletion()`).

### AudioManager

Plays one BGM track at a time (with fade), and one-shot SFX.

**To add audio:**
1. Add an entry to the `BGM` or `SFX` enum in `AudioManager.cs`.
2. Place the audio clip inside `Resources/Audio/BGM/` or `Resources/Audio/SFX/`, named exactly after the enum value.
3. In the Unity Editor, select the `AudioSO` asset and run **Gather Assets** (custom editor button) to populate the ScriptableObject.

### CurrencyManager

Plain C# class managing the player's coin balance.  
Reads and writes `GameManager.userData.coin` directly.  
Call `Save()` on `GameManager` after any mutation you want to persist.

### DebugManager

Provides an in-game debug overlay with tabs, buttons, inspectors, dropdowns, and an input-button combo.  
All functionality is compiled out unless `USE_DEBUG` is defined.

**DebugHandler** is where you register your game-specific debug entries:
```csharp
debugManager.AddButton("Spawn Enemy", () => { stageManager.SpawnEnemy(); }, "Gameplay");
debugManager.AddInspector("Coin", () => userData.coin.ToString(), "Data");
```

---

## Save System

Save data lives in `PlayerPrefs` under the key defined in `Parameter.PlayerPrefKey.SAVE_DATA`.  
The `UserData` class is serialized to JSON by `JsonUtility`.

- **Auto-save** on `OnApplicationQuit`.
- **Manual save** via `gameManager.Save()`.
- **Clear data** via **InterDigital → ClearUserData** in the Unity Editor menu.

> ⚠ There is no schema versioning. If you change `UserData` fields after shipping, existing saves will not migrate automatically.

---

## Project Structure

```
Assets/
  Game/
    Editor/             # Editor-only utilities (GameEditorUtilities)
    Prefabs/            # Game prefabs
    Resources/          # Runtime-loaded assets (Audio clips)
    Scenes/             # Game scenes
    Scripts/
      Debug/            # Debug overlay system
      Managers/         # GameManager, StageManager, AudioManager, CurrencyManager, and other managers in the future
      SO/               # Any ScriptableObjects (currently AudioSO)
      UI/               # UIManager, BaseUI, sample screens and popups
      Util/             # CoroutineCache, Parameter, Util
      UserData.cs       # Save data container
      edityournamespace.asmdef
  Plugins/
    IngameDebugConsole/ # Third-party in-game console (MIT)
  TextMesh Pro/
  Package/
    ProjectStarterPack.unitypackage
```

---

## Editor Menu Reference

| Menu Item | Description |
|---|---|
| InterDigital / ClearUserData | Deletes all PlayerPrefs and persistent data |
| InterDigital / Toggle USE_DEBUG | Adds or removes the `USE_DEBUG` scripting define for the active build target |

---

## Third-Party Plugins

| Plugin | License | Location |
|---|---|---|
| IngameDebugConsole | MIT | `Assets/Plugins/IngameDebugConsole` |
| TextMesh Pro | Unity Companion | `Assets/TextMesh Pro` |

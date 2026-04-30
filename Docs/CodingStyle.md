# Coding Style & Naming Conventions

This document covers all coding conventions used in this project.  
When in doubt, match the existing code rather than invent something new.

---

## Naming

### Classes
PascalCase. Name reflects the single responsibility of the class.
```csharp
GameManager
AudioManager
CurrencyManager
DebugTabUI
CoroutineCache
```

### MonoBehaviour Managers
Suffix with `Manager`.
```csharp
AudioManager
UIManager
StageManager
CurrencyManager
DebugManager
```

### UI Classes
Full-screen UIs: suffix with `UI`.  
Popups: suffix with `Popup`.
```csharp
SampleUI
MainMenuUI
SettingsPopup
SamplePopup
```

### ScriptableObjects
Suffix with `SO`.
```csharp
AudioSO
```

### Interfaces
Prefix with `I`.
```csharp
ICoroutineProvider
IUserDataProvider
```

### Enums
PascalCase for type name. PascalCase for values.  
Always include a `COUNT` sentinel as the last value (used for iteration and "no state" sentinel).
```csharp
public enum UIState
{
    Sample,
    COUNT
}

public enum PopupState
{
    None,
    Sample
}
```

### Constants
SCREAMING_SNAKE_CASE.
```csharp
const string SAVE_DATA = "SaveData";
const float WAIT_FOR_END_OF_FRAME = -1.0f;
const string DEFINE_DEBUG = "USE_DEBUG";
```

### Fields (private / internal)
camelCase. No underscore prefix.
```csharp
GameManager gameManager;
Dictionary<BGM, AudioClip> bgmDict;
bool isCoroutinePopupShowRunning;
```

### Fields — Unity component prefixes
Type-based prefix for component references, to distinguish from plain data fields at a glance.

| Prefix | Type |
|---|---|
| `rt` | RectTransform |
| `obj` | GameObject |
| `text` | TextMeshProUGUI |
| `button` | Button |
| `prefab` | Prefab reference |

```csharp
[SerializeField] RectTransform rtUiContainer;
[SerializeField] GameObject objDebugUI;
[SerializeField] TextMeshProUGUI textFPS;
[SerializeField] Button buttonDebug;
[SerializeField] DebugCanvas prefabDebugCanvas;
```

### Method Parameters
Prefix with `in` to distinguish from class fields.
```csharp
public void Init(GameManager inGameManager)
public void Init(DebugManager inDebugManager)
public DebugInspector(string inName, TextMeshProUGUI inText, Func<string> inCallback)
```

### Methods
PascalCase. Verb-first.
```csharp
Init()
DoUpdate()
ShowUI()
HidePopup()
PlayBGM()
AddCoin()
IsCoinSufficient()
GatherAssets()
```

Coroutine methods that back a public method are named with `-ing` suffix:
```csharp
ShowUI()        → ShowingUI()
HidePopup()     → HidingPopup()
```

### Local Variables
camelCase. Descriptive, not abbreviated (except loop counters `i`, `n`, `k`).
```csharp
float totalHeight;
bool userDataExist;
string jsonUserData;
AudioClip toPlay;
```

Loop counters use single letters `i`, `n`, `k`:
```csharp
for (int i = 0; i < count; i++) { ... }
```

---

## Field Declaration & Visibility

### Inspector fields — `[SerializeField]`
Write `[SerializeField]` on its own line or inline. Omit the `private` keyword (it is implicit).
```csharp
// ✅
[SerializeField] RectTransform rtUiContainer;
[SerializeField] AudioSO audioSO;

// ❌ — do not write private explicitly
[SerializeField] private AudioSource sfxSource;
```

### Inspector grouping — `[Header]` and `[Space]`
Use `[Header]` to group related serialized fields. Use `[Space(10f)]` to add visual separation between groups.
```csharp
[Header("Debug Elements")]
[SerializeField] DebugCanvas prefabDebugCanvas;
[SerializeField] DebugTabUI prefabDebugTabUI;
[Space(10f)]
[SerializeField] Button prefabDebugButtonUI;
```

### Public fields
Only fields that other managers need to read directly are `public`. Do not make fields public for convenience.
```csharp
public StageManager stageManager;    // managers need to access this
public bool gameReady;               // read by other systems
```

Use `[HideInInspector]` for public fields that should not appear in the inspector:
```csharp
[HideInInspector] public CoroutineCache coroutine;
```

### Private fields
No access modifier needed — private is the default.
```csharp
GameManager gameManager;
bool init;
Coroutine bgmCoroutine;
```

---

## Method Order Inside a Class

1. Fields (serialized, then public, then private)
2. `Init(GameManager inGameManager)` — always first method
3. `DoUpdate(float dt)` — always second method if present
4. Other methods (ordered by logical flow / call order)
5. Getter / query methods at the bottom
   ```csharp
   public bool IsPopupActive(PopupState state) { ... }
   public BaseUI GetCurrentActivePopup(PopupState state) { ... }
   ```

---

## BaseUI Subclass Structure

All UI screens and popups inherit from `BaseUI`. The following rules apply:

**`Init`, `Showing`, `Hiding` — always override all three.**  
Even if there is no animation, the override must be present.

**`DoUpdate` — override only if the screen needs a per-frame tick.**  
If nothing updates every frame, omit it entirely. Do not add an empty override.

**Logic and getter methods — add freely at the bottom of the class.**  
Any screen-specific logic (e.g. refreshing display data, querying state) lives here.

```csharp
public class MainMenuUI : BaseUI
{
    [SerializeField] Button buttonPlay;
    [SerializeField] TextMeshProUGUI textCoinCount;

    // 1. Init — wiring only, no animation
    public override void Init(GameManager inGameManager)
    {
        base.Init(inGameManager);
        buttonPlay.onClick.AddListener(OnClickPlay);
    }

    // 2. DoUpdate — only if this screen needs a per-frame tick
    //    Omit entirely if not needed
    public override void DoUpdate(float dt)
    {
        RefreshCoinDisplay();
    }

    // 3. Showing — open animation goes here
    public override IEnumerator Showing(params object[] payload)
    {
        yield return null;
    }

    // 4. Hiding — close animation goes here
    public override IEnumerator Hiding()
    {
        yield return null;
    }

    // 5. Screen-specific logic and getters at the bottom
    void OnClickPlay()
    {
        uiManager.ShowUI(UIState.Game);
    }

    void RefreshCoinDisplay()
    {
        textCoinCount.SetText(gameManager.userData.coin.ToString());
    }
}
```

---

## Return Value Pattern

Always declare a `result` variable, assign it, and return it. Do not return inline.  
This makes it easy to add a breakpoint on the return and inspect the value.

```csharp
// ✅
public bool IsCoinSufficient(int amount)
{
    bool result = userData.coin >= amount;
    return result;
}

// ❌
public bool IsCoinSufficient(int amount)
{
    return userData.coin >= amount;
}
```

---

## Braces & Brackets

Allman style — opening brace on its own line, always, even for single-line bodies.
```csharp
// ✅
void Awake()
{
    Application.targetFrameRate = 120;
}

if (gameReady)
{
    stageManager.DoUpdate(dt);
}

// ❌ — no same-line braces
void Awake() {
    Application.targetFrameRate = 120;
}

// ❌ — no braceless single-line if
if (gameReady)
    stageManager.DoUpdate(dt);
```

---

## Spacing

One blank line between methods.  
No blank lines between field declarations (unless separating `[Header]` groups).  
One blank line after the last field block before the first method.

```csharp
public class SampleManager : MonoBehaviour
{
    [SerializeField] SomeField someField;

    GameManager gameManager;
    bool someFlag;

    public void Init(GameManager inGameManager)
    {
        ...
    }

    public void DoUpdate(float dt)
    {
        ...
    }

    void SomeMethod()
    {
        ...
    }
}
```

---

## Regions

`#region` is used only in `UIManager` to separate the UI section from the Popup section.  
Do not introduce new `#region` blocks elsewhere. If a class needs regions to stay readable, it is likely doing too much.

```csharp
#region UI
// ...
#endregion

#region Popup
// ...
#endregion
```

---

## Comments

### Template instructions — `//READ ME`
Used for instructions that guide the programmer when setting up a new project.  
These are permanent markers in the template — do not remove them.
```csharp
//READ ME manager initialization here
//READ ME managers doupdate here, uiManager is last
//READ ME call first UI below
```

### Pending fixes / work — `//TODO`
Used for known issues or improvements to revisit.
```csharp
// TODO: rename class to AudioSOEditor (and rename this file to match)
```

### XML doc comments
Used on `BaseUI` virtual methods to explain lifecycle rules and restrictions.  
Use them on any public virtual method where the override contract is non-obvious.
```csharp
/// <summary>
/// Only for initialization. Do not put animation here!
/// </summary>
public virtual void Init(GameManager inGameManager) { ... }
```

Do not use XML comments on straightforward public methods — they add noise without value.

---

## Namespace

All game code lives in one namespace defined at project setup.  
The placeholder is `EditYourNameSpace` — rename it to your game's name on first import.

```csharp
namespace EditYourNameSpace
{
    public class GameManager : MonoBehaviour { ... }
}
```

Global namespace (no namespace) is reserved for utility classes that are intentionally project-agnostic:
```csharp
// Global namespace — intentional
public static class Parameter { ... }
public class UserData { ... }
public static class Util { ... }
public class CoroutineCache : MonoBehaviour { ... }
```

---

## Assembly Definition

All game scripts compile into a single assembly: `EditYourNameSpace` (from `edityournamespace.asmdef`).  
Rename both the file and the `"name"` field inside it to match your namespace on first import.

---

## File & Folder Structure

```
Scripts/
  Managers/     — game-specific managers (StageManager, CurrencyManager, TrackingManager...)
  Support/      — plugin-based managers and GameSupport coordinator
  UI/           — UIManager, BaseUI, screen scripts
  UI/Popup/     — Popup scripts
  Debug/        — Debug overlay scripts
  SO/           — ScriptableObject scripts and assets
  Util/         — Shared utility (CoroutineCache, Util, Parameter)
  UserData.cs   — Save data container
```

New screens → `Scripts/UI/`  
New popups → `Scripts/UI/Popup/`  
New game-specific managers → `Scripts/Managers/`  
New plugin-based managers → `Scripts/Support/`  
New ScriptableObjects → `Scripts/SO/`  
New gameplay scripts → `Scripts/Gameplay/`  
&nbsp;&nbsp;&nbsp;&nbsp;`Gameplay/` and `Support/` do not exist in the template. Create them when the first script is added.  
&nbsp;&nbsp;&nbsp;&nbsp;Empty folders are not tracked in git — do not create them until they have content.

---

## GameSupport Pattern

Plugin-based managers (Firebase tracking, ads, IAP, etc.) are separated from game-specific managers.
They live in `Scripts/Support/` and are coordinated by a single `GameSupport` plain C# class.

`GameSupport` is declared and `new`'d in `GameManager.Start`, init'd in `InitManagers()`, same as any other manager.

```csharp
// GameSupport.cs — Scripts/Support/
public class GameSupport
{
    public TrackingManager trackingManager;
    // public AdManager adManager;
    // public IAPManager iapManager;

    public void Init(GameManager inGameManager)
    {
        trackingManager = new TrackingManager();
        trackingManager.Init(inGameManager);
    }
}
```

```csharp
// GameManager.cs — Start()
gameSupport = new GameSupport();

// GameManager.cs — InitManagers()
gameSupport.Init(this);
```

If a specific SDK forces a `MonoBehaviour` (e.g. an ad SDK that needs a scene `GameObject`),  
promote only that manager to a component on a `GameSupport` prefab — do not change the overall pattern.

**Rule:** game logic never calls plugin managers directly.  
All tracking, ads, and IAP calls go through `gameManager.gameSupport.trackingManager`, etc.

---

## Prefab Naming

Prefab names match their class name exactly, including casing.
```
GameManager.prefab
AudioManager.prefab       ← capital M
Canvas (UIManager).prefab
StageManager.prefab
SampleUI.prefab
SamplePopup.prefab
```

---

## Scripting Define Symbols

`USE_DEBUG` — enables the in-game debug overlay.  
Toggle via **InterDigital → Toggle USE_DEBUG** in the Unity Editor menu.  
Ships **ON** in the template (for development). Turn off before release build.

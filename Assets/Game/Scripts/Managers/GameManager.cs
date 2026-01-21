using UnityEngine;

namespace EditYourNameSpace
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector] public CoroutineCache coroutine;

        public StageManager stageManager;
        public UserData userData;

        //READ ME other managers are here (must be public), uiManager is last.
        public CurrencyManager currencyManager;

        public UIManager uiManager;
        public AudioManager audioManager;

        public DebugManager debugManager;
        public DebugHandler debugHandler;

        public bool gameReady;

        void Awake()
        {
            Application.targetFrameRate = 120;

            coroutine = gameObject.AddComponent<CoroutineCache>();
            coroutine.Init();
        }

        void Start()
        {
            Load();

            //READ ME manager declaration here
            currencyManager = new CurrencyManager();

            InitManagers();
        }

        //READ ME manager initialization here
        public void InitManagers()
        {
            //READ ME Init Manager here
            currencyManager.Init(this);

            uiManager.Init(this);
            audioManager.Init(this);

            debugManager.Init();
            debugHandler = new DebugHandler();
            debugHandler.Init(this);

            gameReady = true;

            //READ ME call first UI below
            uiManager.ShowUI(UIState.Sample);
        }

        void FixedUpdate()
        {
            if (gameReady)
            {
                float dt = Time.deltaTime;

                //READ ME managers doupdate here, uiManager is last
                stageManager.DoUpdate(dt);

                uiManager.DoUpdate(dt);

                debugManager.DoUpdate(dt);
            }
        }

        void Load()
        {
            bool userDataExist = PlayerPrefs.HasKey(Parameter.PlayerPrefKey.SAVE_DATA);
            if (userDataExist)
            {
                string jsonUserData = PlayerPrefs.GetString(Parameter.PlayerPrefKey.SAVE_DATA);
                userData = JsonUtility.FromJson<UserData>(jsonUserData);
            }
            else
            {
                userData = new UserData();
                userData.Init(true);
                Save();
            }
        }

        public void Save()
        {
            if (userData != null)
            {
                string jsonUserData = JsonUtility.ToJson(userData);
                PlayerPrefs.SetString(Parameter.PlayerPrefKey.SAVE_DATA, jsonUserData);
            }
        }

        void OnApplicationQuit()
        {
            Save();
        }
    }

}

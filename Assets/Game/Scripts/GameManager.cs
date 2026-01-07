using EditYourNameSpace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CoroutineCache coroutine;

    public StageManager stageManager;
    public UserData userData;

    //READ ME other managers are here (must be public), uiManager is last.
    public CurrencyManager currencyManager;

    public UIManager uiManager;


    public bool gameReady;

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
        currencyManager.Init(this);
        uiManager.Init(this);
    }

    void FixedUpdate()
    {
        if (gameReady)
        {
            float dt = Time.deltaTime;

            //READ ME managers doupdate here, uiManager is last
            stageManager.DoUpdate(dt);

            uiManager.DoUpdate(dt);
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
        string jsonUserData = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(Parameter.PlayerPrefKey.SAVE_DATA, jsonUserData);
    }
    void OnApplicationQuit()
    {
        Save();
    }
}

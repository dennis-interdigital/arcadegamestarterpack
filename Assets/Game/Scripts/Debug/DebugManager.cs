using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class DebugInspector
    {
        public string name;
        public TextMeshProUGUI text;
        public Func<string> callback;

        public DebugInspector(string inName, TextMeshProUGUI inText, Func<string> inCallback)
        {
            name = inName;
            text = inText;
            callback = inCallback;
        }
    }

    public class DebugManager : MonoBehaviour
    {
        [Header("Debug Elements")]
        [SerializeField] DebugCanvas prefabDebugCanvas;
        [SerializeField] DebugTabUI prefabDebugTabUI;
        [Space(10f)]
        [SerializeField] Button prefabDebugButtonUI;
        [SerializeField] TextMeshProUGUI prefabDebugInspectorUI;
        [SerializeField] TMP_Dropdown prefabDebugDropdownUI;
        [SerializeField] DebugInputButtonUI prefabDebugInputButtonUI;
        [Space(10f)]
        [SerializeField] GameObject prefabInGameDebugConsole;

        public float debugButtonHeight;
        public float debugInspectorHeight;
        public float debugInputButtonHeight;
        public float debugDropdownHeight;

        DebugCanvas canvas;

        List<DebugInspector> inspectorList;
        Dictionary<string, DebugTabUI> debugTabDict;

        public DebugLogManager inGameConsole;

        bool init;

        public void Init()
        {
            inspectorList = new List<DebugInspector>();
            debugTabDict = new Dictionary<string, DebugTabUI>();

            canvas = Instantiate(prefabDebugCanvas);
            canvas.Init(this);
            canvas.SetDebugActive(false);

            CreateDebugTab(Parameter.Debug.TABNAME_HOME);

            GameObject consoleObj = Instantiate(prefabInGameDebugConsole);
            inGameConsole = consoleObj.GetComponent<DebugLogManager>();

            Application.logMessageReceived += HandleLog;
            init = true;
        }

        public void DoUpdate(float dt)
        {
            if (init)
            {
                if (canvas.gameObject.activeSelf)
                {
                    foreach (DebugInspector inspector in inspectorList)
                    {
                        string format = $"{inspector.name}:\n{inspector.callback()}";
                        inspector.text.SetText(format);
                    }
                }
            }
        }

        void OnApplicationQuit()
        {
#if USE_DEBUG
            Application.logMessageReceived -= HandleLog;
#endif
        }

        public Button AddButton(string name, UnityAction callback, string tabName)
        {
            DebugTabUI tab = GetTab(tabName);

            if (tab == null)
            {
                Debug.LogError($"Tab {tabName} is null!");
                return null;
            }
            else
            {
                tab.debugButtonCount++;
                tab.SetHeight();

                RectTransform rtContainer = tab.rtContainer;
                Button button = Instantiate(prefabDebugButtonUI, rtContainer);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.SetText(name);
                button.onClick.AddListener(callback);
                button.name = $"DebugButton{name}";
                return button;
            }
        }

        public DebugInspector AddInspector(string name, Func<string> callback, string tabName)
        {
            DebugInspector result = null;

            DebugTabUI tab = GetTab(tabName);

            if (tab == null)
            {
                Debug.LogError($"Tab {tabName} is null!");
            }
            else
            {
                RectTransform rtContainer = tab.rtContainer;
                TextMeshProUGUI textInspector = Instantiate(prefabDebugInspectorUI, rtContainer);

                tab.debugInspectorCount++;
                tab.SetHeight();

                result = new DebugInspector(name, textInspector, callback);
                inspectorList.Add(result);
            }

            return result;
        }

        public DebugInputButtonUI AddInputButton(string name, TMP_InputField.ContentType contentType, UnityAction<string> finishEditCallback, UnityAction buttonCallback, string tabName)
        {
            DebugInputButtonUI result = null;

            DebugTabUI tab = GetTab(tabName);

            if (tab == null)
            {
                Debug.LogError($"Tab {tabName} is null!");
            }
            else
            {
                tab.debugInputButtonCount++;
                tab.SetHeight();

                RectTransform rtcontainer = tab.rtContainer;
                result = Instantiate(prefabDebugInputButtonUI, rtcontainer);
                result.Init(name, finishEditCallback, buttonCallback, contentType);
            }

            return result;
        }

        public TMP_Dropdown AddDropdown(string name, UnityAction<int> callback, string[] options, string tabName)
        {
            TMP_Dropdown result = null;
            DebugTabUI tab = GetTab(tabName);

            if (tab == null)
            {
                Debug.LogError($"Tab {tabName} is null!");
                tab.debugDropdownCount++;
                tab.SetHeight();

                List<string> optionList = options.ToList<string>();

                RectTransform rtContraienr = tab.rtContainer;
                result = Instantiate(prefabDebugDropdownUI, rtContraienr);
                result.name = $"DebugDropdown{name}";
                result.ClearOptions();
                result.AddOptions(optionList);
                result.value = 0;
                result.onValueChanged.AddListener(callback);
            }

            return result;
        }

        DebugTabUI GetTab(string tabname)
        {
            DebugTabUI result = null;
            bool hasKey = debugTabDict.ContainsKey(tabname);

            if (hasKey)
            {
                result = debugTabDict[tabname];
            }
            else
            {
                result = CreateDebugTab(tabname);
            }

            return result;
        }

        public void ShowTab(string tabName)
        {
            foreach (KeyValuePair<string, DebugTabUI> kvp in debugTabDict)
            {
                bool show = kvp.Key.Equals(tabName);
                kvp.Value.gameObject.SetActive(show);
            }
        }

        DebugTabUI CreateDebugTab(string tabName)
        {
            DebugTabUI result = Instantiate(prefabDebugTabUI, canvas.rtTabContainer);
            result.Init(this);
            result.name = $"DebugTab_{tabName}";
            result.textTabName.SetText(tabName);
            debugTabDict.Add(tabName, result);

            string homeTabName = Parameter.Debug.TABNAME_HOME;

            if (tabName != homeTabName)
            {
                AddButton(tabName, () => { ShowTab(tabName); }, homeTabName);
                AddButton(homeTabName, () => { ShowTab(homeTabName); }, tabName);
            }

            return result;
        }

        public void SetInGameConsole(bool active)
        {
            if (active)
            {
                inGameConsole.ShowLogWindow();
            }
            else
            {
                inGameConsole.HideLogWindow();
            }
        }

        void HandleLog(string logString, string stackTrace, LogType logType)
        {
            if (logType == LogType.Error || logType == LogType.Exception)
            {
                inGameConsole.ShowLogWindow();
            }
        }
    }
}


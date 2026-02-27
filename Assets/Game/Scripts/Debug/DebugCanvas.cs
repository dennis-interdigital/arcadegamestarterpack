using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class DebugCanvas : MonoBehaviour
    {
        [SerializeField] Button buttonDebug;
        [SerializeField] Button buttonHideDebug;
        [SerializeField] GameObject objDebugUI;
        public RectTransform rtTabContainer;

        [Header("TopLeft")]
        [SerializeField] TextMeshProUGUI textFPS;
        [SerializeField] TextMeshProUGUI textMemory;
        [SerializeField] TextMeshProUGUI textVersion;

        DebugManager debugManager;

        public void Init(DebugManager inDebugManager)
        {
            debugManager = inDebugManager;
            buttonDebug.onClick.AddListener(OnClickDebug);
            buttonHideDebug.onClick.AddListener(OnClickHideDebug);
        }

        void Update()
        {
#if USE_DEBUG
            float dt = Time.deltaTime;
            dt += (Time.unscaledDeltaTime - dt) * 0.1f;
            float fps = 1f / dt;
            fps = Mathf.Ceil(fps);
            textFPS.SetText($"fps:{fps}");

            long totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            float memoryInMB = totalAllocatedMemory / (1024 * 1024);
            string memoryString = memoryInMB.ToString("0");
            textMemory.SetText($"Mem:{memoryString} MB");

            string version = Application.version;
            textVersion.SetText($"ver:{version}");
#endif
        }

        public void SetDebugActive(bool active)
        {
            objDebugUI.SetActive(active);
            buttonDebug.gameObject.SetActive(!active);

            if (active)
            {
                debugManager.ShowTab(Parameter.Debug.TABNAME_HOME);
            }
        }

        void OnClickDebug()
        {
            SetDebugActive(true);
        }

        void OnClickHideDebug()
        {
            SetDebugActive(false);
        }
    }
}



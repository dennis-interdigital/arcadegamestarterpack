using UnityEngine;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class DebugCanvas : MonoBehaviour
    {
        [SerializeField] Button buttonDebug;
        [SerializeField] Button buttonHideDebug;
        [SerializeField] GameObject objDebugUI;
        public RectTransform rtTabContainer;

        DebugManager debugManager;

        public void Init(DebugManager inDebugManager)
        {
            debugManager = inDebugManager;
            buttonDebug.onClick.AddListener(OnClickDebug);
            buttonHideDebug.onClick.AddListener(OnClickHideDebug);
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



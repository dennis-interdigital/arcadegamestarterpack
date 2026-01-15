using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class DebugTabUI : MonoBehaviour
    {
        public RectTransform rtContainer;
        public TextMeshProUGUI textTabName;
        [SerializeField] VerticalLayoutGroup layoutGroup;

        DebugManager debugManager;

        [HideInInspector] public int debugButtonCount;
        [HideInInspector] public int debugInspectorCount;
        [HideInInspector] public int debugInputButtonCount;
        [HideInInspector] public int debugDropdownCount;

        public void Init(DebugManager inDebugManager)
        {
            debugManager = inDebugManager;
        }

        public void SetHeight()
        {
            float paddings = layoutGroup.padding.top + layoutGroup.padding.bottom;

            int totalDebugItems = debugButtonCount + debugInspectorCount + debugInputButtonCount + debugDropdownCount - 1;
            float totalSpacing = totalDebugItems * layoutGroup.spacing;

            float totalElementHeights =
                debugButtonCount * debugManager.debugButtonHeight +
                debugInspectorCount * debugManager.debugInspectorHeight +
                debugInputButtonCount * debugManager.debugInputButtonHeight +
                debugDropdownCount * debugManager.debugDropdownHeight;

            float totalHeight = paddings + totalSpacing + totalElementHeights;
            float screenHeight = Screen.height;

            float height = Mathf.Max(totalHeight, screenHeight);

            rtContainer.sizeDelta = new Vector2(rtContainer.sizeDelta.x, height);
            rtContainer.anchoredPosition = Vector2.zero;
        }
    }
}
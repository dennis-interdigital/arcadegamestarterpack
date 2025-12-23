using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditYourNameSpace
{
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

    public class UIManager : MonoBehaviour
    {
        [SerializeField] RectTransform rtUiContainer;
        [SerializeField] RectTransform rtPopupContainer;

        [Header("UI Prefabs")]
        [SerializeField] SampleUI sampleUIPrefab;

        [Header("Popup Prefabs")]
        [SerializeField] SamplePopup samplePopupPrefab;

        Dictionary<UIState, GameObject> uiBlueprintDict;
        Dictionary<PopupState, GameObject> popupBlueprintDict;

        public Dictionary<PopupState, BaseUI> currActivePopups;

        public List<PopupState> currPopupShowQueue;
        public List<PopupState> currPopupHideQueue;
        public List<object[]> currPoupShowParamQueue;
        public List<object[]> currPopupHideParamQueue;

        public UIState currentUIState;
        public BaseUI currentActiveUI;

        StageManager stageManager;

        Coroutine currUICoroutine;
        Coroutine currPopupShowCoroutine;
        Coroutine currPopupHideCoroutine;
        bool isCoroutinePopupShowRunning;
        bool isCoroutinePopupHideRunning;

        public void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;

            uiBlueprintDict = new Dictionary<UIState, GameObject>();
            popupBlueprintDict = new Dictionary<PopupState, GameObject>();

            currPopupShowQueue = new List<PopupState>();
            currPopupHideQueue = new List<PopupState>();
            currPoupShowParamQueue = new List<object[]>();
            currPopupHideParamQueue = new List<object[]>();

            uiBlueprintDict.Add(UIState.Sample, sampleUIPrefab.gameObject);

            popupBlueprintDict.Add(PopupState.Sample, samplePopupPrefab.gameObject);
            
        }

        public void DoUpdate(float dt)
        {
            if (currentActiveUI != null)
            {
                currentActiveUI.DoUpdate(dt);
            }

            foreach (KeyValuePair<PopupState, BaseUI> kvp in currActivePopups)
            {
                BaseUI popup = kvp.Value;
                popup.DoUpdate(dt);
            }
        }

        #region UI
        public void ShowUI(UIState state, params object[] payload)
        {

        }

        IEnumerator ShowingUI(UIState state, params object[] payload)
        {
            if (currentUIState != UIState.COUNT)
            {
                yield return HidingUI(state, payload);
            }

            BaseUI ui = CreateUI(state);
            if (ui != null)
            {
                yield return ui.Showing(payload);
            }
            else
            {
                Debug.Log($"UI {state} is null!");
            }
        }

        IEnumerator HidingUI(UIState state, params object[] payload)
        {
            if (currentUIState == state)
            {
                BaseUI ui = currentActiveUI;
                yield return ui.Hiding();

                DestroyUI(state);

                currentUIState = UIState.COUNT;
            }
            else
            {
                Debug.LogError($"UI {state} is not active! current active UIState is {currentUIState}!");
            }
        }

        BaseUI CreateUI(UIState state)
        {
            BaseUI result = null;

            if (currentUIState != state)
            {
                GameObject go = Instantiate(uiBlueprintDict[state], rtUiContainer);
                Transform transform = go.transform;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;

                BaseUI baseUI = go.GetComponent<BaseUI>();
                baseUI.Init(stageManager);

                currentActiveUI = baseUI;
                result = baseUI;
            }

            return result;
        }

        void DestroyUI(UIState state)
        {
            if (currentActiveUI != null)
            {
                Destroy(currentActiveUI.gameObject);
                currentActiveUI = null;
            }
        }
        #endregion

        #region Popup
        public void ShowPopup(PopupState state, params object[] payload)
        {

        }

        IEnumerator ShowingPopup(PopupState state, params object[] payload)
        {
            yield return null;
        }

        public void HidePopup(PopupState state, params object[] payload)
        {

        }

        IEnumerator HidingPopup(PopupState state, params object[] payload)
        {
            yield return null;
        }
        #endregion

    }
}



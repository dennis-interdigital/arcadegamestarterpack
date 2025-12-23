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

        Dictionary<PopupState, BaseUI> currActivePopups;

        Queue<PopupState> currPopupShowQueue;
        Queue<PopupState> currPopupHideQueue;
        Queue<object[]> currPoupShowParamQueue;

        [HideInInspector] public UIState currentUIState;
        [HideInInspector] public BaseUI currentActiveUI;

        StageManager stageManager;

        bool isCoroutinePopupShowRunning;
        bool isCoroutinePopupHideRunning;

        public void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;

            uiBlueprintDict = new Dictionary<UIState, GameObject>();
            popupBlueprintDict = new Dictionary<PopupState, GameObject>();

            currPopupShowQueue = new Queue<PopupState>();
            currPopupHideQueue = new Queue<PopupState>();
            currPoupShowParamQueue = new Queue<object[]>();

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
            StartCoroutine(ShowingUI(state, payload));
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
            if (!isCoroutinePopupShowRunning)
            {
                isCoroutinePopupShowRunning = true;
                StartCoroutine(ShowingPopup(state, payload));
            }
            else
            {
                QueueShowPopup(state, payload);
            }
        }

        public void HidePopup(PopupState state)
        {
            if (!isCoroutinePopupHideRunning)
            {
                isCoroutinePopupHideRunning = true;
                StartCoroutine(HidingPopup(state));
            }
            else
            {
                QueueHidePopup(state);
            }
        }

        IEnumerator ShowingPopup(PopupState state, params object[] payload)
        {
            BaseUI popup = CreatePopup(state);
            if (popup != null)
            {
                yield return popup.Showing(payload);
                isCoroutinePopupShowRunning = false;

                NextShowQueue();
            }
            else
            {
                Debug.Log($"Popup {state} is null!");
                isCoroutinePopupShowRunning = false;
            }
        }

        IEnumerator HidingPopup(PopupState state)
        {
            if (currActivePopups.ContainsKey(state))
            {
                BaseUI popup = currActivePopups[state];
                yield return popup.Hiding();
                DestroyPopup(state);
                isCoroutinePopupHideRunning = false;

                NextHideQueue();
            }
            else
            {
                Debug.LogError($"Popup {state} is not active!");
                isCoroutinePopupHideRunning = false;
            }
        }

        void QueueShowPopup(PopupState state, params object[] payload)
        {
            currPopupShowQueue.Enqueue(state);
            currPoupShowParamQueue.Enqueue(payload);
        }

        void QueueHidePopup(PopupState state)
        {
            currPopupHideQueue.Enqueue(state);
        }

        void NextShowQueue()
        {
            if (currPopupShowQueue.Count > 0)
            {
                PopupState state = currPopupShowQueue.Dequeue();
                object[] payload = currPoupShowParamQueue.Dequeue();
                ShowPopup(state, payload);
            }
        }

        void NextHideQueue()
        {
            if (currPopupHideQueue.Count > 0)
            {
                PopupState state = currPopupHideQueue.Dequeue();
                HidePopup(state);
            }
        }

        BaseUI CreatePopup(PopupState state)
        {
            BaseUI result = null;

            if (!currActivePopups.ContainsKey(state))
            {
                GameObject go = Instantiate(popupBlueprintDict[state], rtPopupContainer);
                Transform transform = go.transform;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;

                BaseUI baseUI = go.GetComponent<BaseUI>();
                currActivePopups.Add(state, baseUI);

                result = baseUI;
            }
            else
            {
                Debug.Log($"Popup {state} already exist!");
            }

            return result;
        }

        void DestroyPopup(PopupState state)
        {
            if (currActivePopups.ContainsKey(state))
            {
                BaseUI baseUI = currActivePopups[state];
                currActivePopups.Remove(state);

                Destroy(baseUI.gameObject);
            }
        }
        #endregion

        public bool IsPopupActive(PopupState state)
        {
            bool result = currActivePopups.ContainsKey(state);
            return result;
        }

        public BaseUI GetCurrentActivePopup(PopupState state)
        {
            BaseUI result = currActivePopups[state];
            return result;
        }
    }
}



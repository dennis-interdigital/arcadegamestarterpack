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

        Dictionary<PopupState, BaseUI> currentActivePopups;

        Queue<PopupState> currentPopupShowQueue;
        Queue<PopupState> currentPopupHideQueue;
        Queue<object[]> currentPoupShowParamQueue;

        [Header("Runtime")]
        public UIState currentUIState;
        public BaseUI currentActiveUI;

        GameManager gameManager;

        bool isCoroutinePopupShowRunning;
        bool isCoroutinePopupHideRunning;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;

            uiBlueprintDict = new Dictionary<UIState, GameObject>();
            popupBlueprintDict = new Dictionary<PopupState, GameObject>();

            currentActivePopups = new Dictionary<PopupState, BaseUI>();

            currentPopupShowQueue = new Queue<PopupState>();
            currentPopupHideQueue = new Queue<PopupState>();
            currentPoupShowParamQueue = new Queue<object[]>();

            uiBlueprintDict.Add(UIState.Sample, sampleUIPrefab.gameObject);

            popupBlueprintDict.Add(PopupState.Sample, samplePopupPrefab.gameObject);

            currentUIState = UIState.COUNT;
        }

        public void DoUpdate(float dt)
        {
            if (currentActiveUI != null)
            {
                currentActiveUI.DoUpdate(dt);
            }

            foreach (KeyValuePair<PopupState, BaseUI> kvp in currentActivePopups)
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
                yield return HidingUI(currentUIState);
            }

            BaseUI ui = CreateUI(state);
            if (ui != null)
            {
                yield return ui.Showing(payload);
                currentUIState = state;
            }
            else
            {
                Debug.Log($"UI {state} is null!");
            }
        }

        IEnumerator HidingUI(UIState state)
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
                baseUI.Init(gameManager);

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
            if (currentActivePopups.ContainsKey(state))
            {
                BaseUI popup = currentActivePopups[state];
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
            currentPopupShowQueue.Enqueue(state);
            currentPoupShowParamQueue.Enqueue(payload);
        }

        void QueueHidePopup(PopupState state)
        {
            currentPopupHideQueue.Enqueue(state);
        }

        void NextShowQueue()
        {
            if (currentPopupShowQueue.Count > 0)
            {
                PopupState state = currentPopupShowQueue.Dequeue();
                object[] payload = currentPoupShowParamQueue.Dequeue();
                ShowPopup(state, payload);
            }
        }

        void NextHideQueue()
        {
            if (currentPopupHideQueue.Count > 0)
            {
                PopupState state = currentPopupHideQueue.Dequeue();
                HidePopup(state);
            }
        }

        BaseUI CreatePopup(PopupState state)
        {
            BaseUI result = null;

            if (!currentActivePopups.ContainsKey(state))
            {
                GameObject go = Instantiate(popupBlueprintDict[state], rtPopupContainer);
                Transform transform = go.transform;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;

                BaseUI baseUI = go.GetComponent<BaseUI>();
                baseUI.Init(gameManager);
                currentActivePopups.Add(state, baseUI);

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
            if (currentActivePopups.ContainsKey(state))
            {
                BaseUI baseUI = currentActivePopups[state];
                currentActivePopups.Remove(state);

                Destroy(baseUI.gameObject);
            }
        }
        #endregion

        public bool IsPopupActive(PopupState state)
        {
            bool result = currentActivePopups.ContainsKey(state);
            return result;
        }

        public BaseUI GetCurrentActivePopup(PopupState state)
        {
            BaseUI result = currentActivePopups[state];
            return result;
        }
    }
}



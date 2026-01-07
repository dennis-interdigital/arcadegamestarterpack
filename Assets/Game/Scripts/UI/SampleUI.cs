using EditYourNameSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class SampleUI : BaseUI
    {
        [SerializeField] Button buttonSamplePopup;
        public override void Init(GameManager inGameManager)
        {
            base.Init(inGameManager);

            buttonSamplePopup.onClick.AddListener(OnClickSamplePopup);
        }

        public override IEnumerator Showing(params object[] payload)
        {
            yield return null;
        }

        public override IEnumerator Hiding()
        {
            yield return null;
        }

        void OnClickSamplePopup()
        {
            uiManager.ShowPopup(PopupState.Sample);
        }
    }
}
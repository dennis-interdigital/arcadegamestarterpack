using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class SamplePopup : BaseUI
    {
        [SerializeField] Button buttonSample;

        public override void Init(GameManager inGameManager)
        {
            base.Init(inGameManager);
            buttonSample.onClick.AddListener(OnClickSample);
        }

        //READ ME 
        //if you dont use any show or hide, you can erase them.
        public override IEnumerator Showing(params object[] payload)
        {
            yield return null;
        }

        public override IEnumerator Hiding()
        {
            yield return null;
        }

        void OnClickSample()
        {
            Debug.Log("Sample button clicked");
            uiManager.HidePopup(PopupState.Sample);
        }
    }
}


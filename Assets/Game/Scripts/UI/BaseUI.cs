using EditYourNameSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditYourNameSpace
{
    public class BaseUI : MonoBehaviour
    {
        protected StageManager stageManager;
        protected UIManager uiManager;

        public virtual void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;
            uiManager = stageManager.uiManager;
        }

        public virtual void DoUpdate(float dt)
        {

        }

        public virtual IEnumerator Showing(params object[] payload)
        {
            yield return null;
        }

        public virtual IEnumerator Hiding()
        {
            yield return null;
        }
    }
}


using System.Collections;
using UnityEngine;

namespace EditYourNameSpace
{
    public class BaseUI : MonoBehaviour
    {
        protected GameManager gameManager;
        protected UIManager uiManager;

        /// <summary>
        /// Only for initialization. Do not put animation here!
        /// </summary>
        public virtual void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;
            uiManager = gameManager.uiManager;
        }

        public virtual void DoUpdate(float dt)
        {

        }

        /// <summary>
        /// Showing is called after Init. animation does play here
        /// 
        /// <para>NOTE:</para>
        /// Any DoTween animation started inside this coroutine
        /// must be yielded (WaitForCompletion / WaitForKill).
        /// Do NOT start tweens without yielding them,
        /// or this will disrupt the UI Framework flow.
        /// </summary>
        /// <returns>
        /// IEnumerator for StartCoroutine.
        /// </returns>
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


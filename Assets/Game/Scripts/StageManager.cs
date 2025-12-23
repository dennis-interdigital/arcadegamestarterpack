using UnityEngine;

//READ ME: Edit using:
//VS: ctrl+r, ctrl+r
//Rider: Shift+F6
namespace EditYourNameSpace 
{
    public class StageManager : MonoBehaviour
    {
        public UIManager uiManager;

        bool gameReady = false;

        void Start()
        {
            uiManager.Init(this);

            gameReady = true;
        }

        void FixedUpdate()
        {
            if (gameReady)
            {
                float dt = Time.deltaTime;

                //DoUpdate here
            }
        }
    }
}

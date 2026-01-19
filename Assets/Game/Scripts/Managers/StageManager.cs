using System.Collections;
using UnityEngine;

//READ ME: Edit using:
//VS: ctrl+r, ctrl+r
//Rider: Shift+F6
namespace EditYourNameSpace 
{
    public class StageManager : MonoBehaviour
    {
        GameManager gameManager;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;
        }

        public void DoUpdate(float dt)
        {

        }
    }
}

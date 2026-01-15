using System;
using UnityEngine;

namespace EditYourNameSpace
{
    public class DebugHandler
    {

        GameManager gameManager;
        DebugManager debugManager;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;
            debugManager = gameManager.debugManager;

            string tabName = "SAMPLE Button";
            debugManager.AddButton("Debug Button", () => { Debug.Log("Debug Button Clicked"); }, tabName);

            tabName = "SAMPLE Inspector";
            debugManager.AddInspector("Time", () => { return DateTime.Now.ToString(); }, tabName);
            //READ ME add everything here, above "MISC"


            tabName = "MISC";
            debugManager.AddButton("Show Console", () =>
            {
                debugManager.SetInGameConsole(true);
            }, tabName);
        }
    }
}


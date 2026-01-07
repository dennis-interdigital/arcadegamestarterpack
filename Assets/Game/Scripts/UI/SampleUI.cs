using EditYourNameSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleUI : BaseUI
{
    public override void Init(GameManager inGameManager)
    {
        base.Init(inGameManager);
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
}

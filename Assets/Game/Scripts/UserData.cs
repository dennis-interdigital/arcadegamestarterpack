using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public int coin;

    public void Init(bool firstTimePlay)
    {
        if (firstTimePlay)
        {
            coin = 0;
        }
    }
}

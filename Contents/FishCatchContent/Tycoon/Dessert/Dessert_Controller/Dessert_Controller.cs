using JHchoi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dessert_Controller : IFood
{
    protected override void CatchSoundPlay()
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Dessert_Catch);
    }
}

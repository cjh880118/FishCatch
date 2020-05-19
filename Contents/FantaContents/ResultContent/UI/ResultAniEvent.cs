using JHchoi;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResultAniEvent : MonoBehaviour
{
    
    public void AniEvent_ScoreUp()
    {
        Message.Send<ResultScoreUpMsg>(new ResultScoreUpMsg());
    }

    public void AniEvent_UISound_Clear()
    {
        SoundManager.Instance.PlaySound((int)SoundType_System.Clear);

    }


}

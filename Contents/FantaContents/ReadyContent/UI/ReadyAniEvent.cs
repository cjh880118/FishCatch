using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using CellBig;

public class ReadyAniEvent : MonoBehaviour
{
    public void AniEvent_UISound_Ready()
    {
        SoundManager.Instance.PlaySound((int)SoundType_System.Ready);
    }

    public void AniEvent_UISound_Go()
    {
        SoundManager.Instance.PlaySound((int)SoundType_System.Go);
    }

    public void AniEvent_ReadyGoEnd()
    {
        Message.Send<ReadyGoEndMesg>(new ReadyGoEndMesg());
    }
}

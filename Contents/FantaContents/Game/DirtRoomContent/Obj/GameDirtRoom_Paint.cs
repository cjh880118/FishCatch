using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.Contents.Event;

public class GameDirtRoom_Paint : MonoBehaviour
{
    public DirtRoom_SpriteFootColor spriteFootColor;
    public GameDirtRoom_EnvironmentObject environmentObject;

    public void Hit()
    {
        if (!environmentObject.isHit)
            return;

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.DirtRoom_Paint);
        Message.Send<StainFootMsg>(new StainFootMsg(true, spriteFootColor));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.Contents.Event;

public class GameFruit_FruitBomb : GameFruit_Fruit
{
    public Color cameraColor;

    protected override void Active()
    {
        base.Active();

        addScore = -200;
    }

    public override void Hit()
    {
        base.Hit();

        Message.Send<ShakeCameraMsg>(new ShakeCameraMsg(0.5f, 15.0f));
        Message.Send<ColorCameraMsg>(new ColorCameraMsg(0.3f, cameraColor));
    }

    protected override void HitSound()
    {
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Fruit_Explosion);
    }

    protected override void DeActive()
    {
        base.DeActive();
        SoundManager.Instance.StopSound((int)SoundType_GameFX.Mole_Fuse);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.Contents.Event;

public class GameMole_MoleBomb : GameMole_Mole
{
    public Color cameraColor;

    protected override void Active()
    {
        base.Active();

        addScore = -200;

        ChangeState(MoleState.Show);
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mole_Fuse);
    }

    public override void Hit()
    {
        base.Hit();

        Message.Send<ColorCameraMsg>(new ColorCameraMsg(0.3f, cameraColor));
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mole_Explosion);
    }

    protected override void DeActive()
    {
        base.DeActive();
        SoundManager.Instance.StopSound((int)SoundType_GameFX.Mole_Fuse);
    }
}

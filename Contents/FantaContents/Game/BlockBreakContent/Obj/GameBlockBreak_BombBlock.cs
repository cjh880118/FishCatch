using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameBlockBreak_BombBlock : GameBlockBreak_Block
{
    internal float m_fBoomTimer = 0.0f;
    internal float m_fBoomTimer_Min = 0.0f;
    internal float m_fBoomTimer_Max = 0.0f;

    bool isBoom = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        isBoom = false;

        m_fBoomTimer_Min = 8.0f;
        m_fBoomTimer_Max = 13.0f;

        StartCoroutine(BombTimer());
    }

    IEnumerator BombTimer()
    {
        m_fBoomTimer = Random.Range(m_fBoomTimer_Min, m_fBoomTimer_Max);
        yield return new WaitForSeconds(m_fBoomTimer);

        isBoom = true;

        StartCoroutine(Die());

        Message.Send<FreezeBlockMsg>(new FreezeBlockMsg(m_nX, m_nY));
        
        yield return null;
    }

    protected override void HitSound()
    {
        base.HitSound();
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.BlockBreak_HitIceBomb);
    }

    protected override void ScoreUpdate()
    {
        base.ScoreUpdate();
        if (!isBoom)
        {
            Message.Send<ADDScore>(new ADDScore(-score));
            scoreTextControl.SetScore(-score);
        }
    }
}

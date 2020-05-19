using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi;
using JHchoi.UI.Event;

public class GameblockBreak_NormalBlock : GameBlockBreak_Block
{
    protected override void OnEnable()
    {
        base.OnEnable();

        int rnd = Random.Range(0, candySprite.Length);

        mainSprite.sprite = candySprite[rnd];
        m_pSprite_Freeze.SetActive(false);
    }

    public override void Freeze()
    {
        base.Freeze();
        m_pSprite_Freeze.gameObject.SetActive(true);
    }

    protected override void HitSound()
    {
        base.HitSound();

        if (m_nHp == 0)
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.BlockBreak_HitBubble);
        else
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.BlockBreak_HitFreezeBubble);
    }

    protected override void ScoreUpdate()
    {
        base.ScoreUpdate();
        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);
    }
}

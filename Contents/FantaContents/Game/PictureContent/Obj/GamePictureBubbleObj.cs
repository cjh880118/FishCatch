using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi.Contents.Event;

public class GamePictureBubbleObj : Object_Root
{
    public Collider m_pCollider = null;
    public Animator m_pAni_Bubble = null;
    public Animator m_pAni_Ice = null;
    public Animator m_pAni_Boom = null;

    GamePictureBubble m_pMng = null;

    int m_nFreezeHp = 0;
    float m_fBoomTime_Min = 0.0f;
    float m_fBoomTime_Max = 0.0f;


    int m_nHp = 0;
    internal bool m_bBoom = false;
    internal bool m_bFreeze = false;

    internal int m_nX, m_nY;
    internal bool m_bLife = false;

    int m_nScore = 0;
    int m_nDownScore = 0;

    public GameObject IceBreak;
    public GameObject IceHit;
    public GameObject iceExplosion;
    public GameObject BubblePop;

    int currentPictureNum;

    void OnEnable()
    {
        m_nFreezeHp = 3;
        m_fBoomTime_Min = 7.0f;
        m_fBoomTime_Max = 12.0f;

        m_nScore = 15;
        m_nDownScore = 30;

        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();

        
    }

    void AddMessage()
    {
        Message.AddListener<CurrentPictureNumMsg>(OnCurrentPictureNumMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<CurrentPictureNumMsg>(OnCurrentPictureNumMsg);
    }

    void OnCurrentPictureNumMsg(CurrentPictureNumMsg msg)
    {
        currentPictureNum = msg.Num;
    }

    public void SetMng(GamePictureBubble pMng)
    {
        m_pMng = pMng;
    }

    public override void Active()
    {
        base.Active();
    }

    public void Active(Vector3 pPos, bool bBoom)
    {
        Active();
        m_bLife = true;
        bActive = true;

        m_pCollider.enabled = true;
        transform.localPosition = pPos;
        m_nHp = 1;

        m_bBoom = bBoom;
        m_pAni_Bubble.gameObject.SetActive(!bBoom);
        m_pAni_Ice.gameObject.SetActive(false);
        m_pAni_Boom.gameObject.SetActive(bBoom);
        m_bFreeze = false;
        if (m_bBoom == true)
        {
            m_pAni_Boom.SetTrigger("IDLE");
            StartCoroutine("Cor_BoomTimer");
        }
        else
        {

            m_pAni_Bubble.SetTrigger("IDLE");
        }
    }

    public void SetXY(int nX, int nY)
    {
        m_nX = nX;
        m_nY = nY;
    }

    public void Freeze()
    {
        m_bFreeze = true;
        m_pAni_Ice.gameObject.SetActive(true);
        m_pAni_Ice.SetTrigger("IDLE");
        m_pAni_Bubble.SetTrigger("FROZEN");
        m_nHp = m_nFreezeHp;
    }

    IEnumerator Cor_BoomTimer()
    {
        float fTime = Random.Range(m_fBoomTime_Min, m_fBoomTime_Max);
        bool bDanger = false;
        for (float i = 0.0f; i < fTime; i += Time.deltaTime)
        {
            if (bDanger == false && (i / fTime * 1.0f) > 0.6f)
            {
                bDanger = true;
                m_pAni_Boom.SetTrigger("DANGER");
            }
            yield return null;
        }

        yield return null;
        JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Picture_Boom);
        yield return null;
        m_pMng.Freeze(m_nX, m_nY);
        this.gameObject.SetActive(false);
        FXCreate(iceExplosion);

        yield return null;
    }


    public void Hit()
    {
        if (m_bLife == false) return;
        if (bActive == false) return;
        m_pCollider.enabled = false;

        m_nHp--;
        if (m_nHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine("Cor_Hit");
        }
    }

    IEnumerator Cor_Hit()
    {
        if (m_bFreeze == true)
        {
            JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Picture_IceHIt);

            if (m_nHp == 1)
            {
                m_pAni_Ice.gameObject.SetActive(false);
                m_pAni_Ice.SetTrigger("IDLE");
                m_pAni_Bubble.SetTrigger("IDLE");
                FXCreate(IceBreak);

                yield return new WaitForSeconds(0.2f);
                m_pCollider.enabled = true;
                yield break;
            }
            FXCreate(IceHit);

            m_pAni_Ice.SetTrigger("HIT");
        }
        yield return new WaitForSeconds(0.2f);
        m_pCollider.enabled = true;
    }

    public void Die()
    {
        if (m_bLife == false) return;
        if (bActive == false) return;

        Message.Send<JHchoi.UI.Event.ADDScore>(new JHchoi.UI.Event.ADDScore(100));
        m_bLife = false;

        if (m_bBoom == true)
        {
            JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Pictrue_BoomHit);
            StopCoroutine("Cor_BoomTimer");
            Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(currentPictureNum, this.gameObject));

            this.gameObject.SetActive(false);
            FXCreate(BubblePop);
        }
        else
        {
            JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Picture_Hit);
            m_pAni_Bubble.SetTrigger("BANG");
            StartCoroutine("Cor_Die");
            FXCreate(BubblePop);
        }

        
    }

    IEnumerator Cor_Die()
    {
        yield return new WaitForSeconds(0.2f);
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(currentPictureNum, this.gameObject));

        bActive = false;
        this.gameObject.SetActive(false);
    }

    void FXCreate(GameObject fx)
    {
        GameObject createObj;
        Vector3 pos;
        createObj = Instantiate(fx);
        createObj.SetActive(false);
        createObj.transform.SetParent(this.transform.parent);
        pos = this.transform.position;
        createObj.SetActive(true);
        createObj.transform.position = pos;
        DestroyFx(createObj);
    }

    void DestroyFx(GameObject fx)
    {
        Destroy(fx, 0.5f);
    }
}

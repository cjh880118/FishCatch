using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public enum Target_State
{
    E_OPEN,
    E_WAIT,
    E_CLOSE,

    E_MAX,
}

public class GameWeston_WestonTarget : MonoBehaviour
{
    [System.Serializable]
    public struct TargetMove
    {
        public Vector3 fLeft;
        public Vector3 fRight;
    }
     
    public Collider m_pCollider = null;
    public GameObject m_pBoom = null;
    public Animation m_pAni = null;

    public GameObject[] m_pEffectPoint = null;
    public ParticleSystem m_pParticle = null;
    public ParticleSystem m_pParticle2 = null;

    internal Target_State m_eState;
    public bool m_bLife = false;

    public bool m_bMove = false;
    public TargetMove m_stMoveData;
    public ScoreTextControl scoreTextControl;

    float m_fWaitTime = 0.0f;
    int m_nScore = 0;
    int m_nBoomScore = 0;

    Coroutine m_pCor = null;

    protected void OnEnable()
    {
        m_nScore = 100;
        m_nBoomScore = 100;
        m_bLife = false;

        m_pParticle.gameObject.SetActive(false);
        m_pParticle2.gameObject.SetActive(false);

        Close(false);
    }

    void OnDisable()
    {
        m_pCor = null;

        if (m_pCor != null)
            StopCoroutine(m_pCor);
    }

    public void Active()
    {

    }


    public void DeActive()
    {

    }

    public void Open(float fWaitTime, bool bBoom = false)
    {
        m_pBoom.SetActive(bBoom);
        m_fWaitTime = fWaitTime;
        m_bLife = true;

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
            m_pCor = null;
        }
        m_pCor = StartCoroutine(Cor_Open());
    }

    IEnumerator Cor_Open()
    {
        m_eState = Target_State.E_OPEN;
        m_pCollider.enabled = false;

        m_pAni.Play("Target_Open");

        m_pParticle.gameObject.SetActive(false);
        m_pParticle2.gameObject.SetActive(false);

        //m_pSound.Play("Init");

        if (m_pBoom.activeSelf == true)
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Weston_FuseBomb);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Weston_InitTarget);

        yield return new WaitForSeconds(0.3f);

        m_pCor = StartCoroutine(Cor_Wait(m_fWaitTime));

        yield return null;
    }

    IEnumerator Cor_Wait(float fTime)
    {
        m_eState = Target_State.E_WAIT;
        m_pCollider.enabled = true;

        m_pAni.Play("Target_Wait");

        if (m_bMove == true)
            StartCoroutine("Cor_Move");

        yield return new WaitForSeconds(fTime);

        Close(false);

        yield return null;
    }

    IEnumerator Cor_Move()
    {
        Vector3 CurrPos = transform.localPosition;
        while (true)
        {
            CurrPos = transform.localPosition;
            for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 0.05f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_stMoveData.fLeft, i);
                if (Mathf.Abs(Vector3.Distance(transform.localPosition, m_stMoveData.fLeft)) < 0.1f) break;
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 0.05f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_stMoveData.fRight, i);
                if (Mathf.Abs(Vector3.Distance(transform.localPosition, m_stMoveData.fRight)) < 0.1f) break;
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
    }


    public void Close(bool bHit)
    {
        m_bLife = false;

        m_pBoom.SetActive(false);

        //m_pSound.Stop("Fuse");

        SoundManager.Instance.StopSound((int)SoundType_GameFX.Weston_FuseBomb);

        StopCoroutine("Cor_Move");

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
            m_pCor = null;
        }
        m_pCor = StartCoroutine(Cor_Close(bHit));
    }

    IEnumerator Cor_Close(bool bHit)
    {
        if (bHit == true)
        {
            m_pAni.Play("Target_Hit");
        }
        else
        {
            m_pAni.Play("Target_Close");
        }

        m_pCollider.enabled = false;
        m_eState = Target_State.E_CLOSE;

        yield return null;
    }

    public void Hit()
    {
        if (m_eState != Target_State.E_WAIT) return;

        if (m_pBoom.activeSelf == true)
        {
            m_pBoom.SetActive(true);
            scoreTextControl.SetScore(-m_nBoomScore);
            Message.Send<ADDScore>(new ADDScore(-m_nBoomScore));
            Message.Send<ColorCameraMsg>(new ColorCameraMsg(0.1f, new Color(1f, 0f, 0f, 0.7f)));

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Weston_Explosion);
        }
        else
        {
            scoreTextControl.SetScore(m_nScore);
            Message.Send<ADDScore>(new ADDScore(m_nScore));

            int a = Random.Range(0, m_pEffectPoint.Length);

            m_pParticle.gameObject.SetActive(true);
            m_pParticle.Play();

            m_pParticle2.gameObject.SetActive(true);
            m_pParticle2.Play();

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Weston_ShotgunFire);
        }

        Close(true);
    }
}

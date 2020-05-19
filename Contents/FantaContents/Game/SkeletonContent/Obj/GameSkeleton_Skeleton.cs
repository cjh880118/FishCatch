using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public enum E_SKELETON_STATE
{
    E_Idle = 0,
    E_Open1,
    E_Open2,
    E_Jump,
    E_Die,
    E_Boom,
    E_MAX,
}

public class GameSkeleton_Skeleton : MonoBehaviour
{
    E_SKELETON_STATE m_eState = E_SKELETON_STATE.E_MAX;

    public GameObject m_pBoom = null;
    public Animator m_pAni = null;
    public CapsuleCollider m_pCollider = null;

    public ScoreTextControl scoreTextControl;

    public GameObject hitParticle;
    public GameObject boomParticle;

    float m_fHideTime_Min = 0.0f;
    float m_fHideTime_Max = 0.0f;
    float m_fBoomPercent = 0.0f;
    int m_nScore = 0;
    int m_nBoomScore = 0;

    float m_fHideTime = 0.0f;
    int m_nCurrHoleNum = 0;

    Coroutine m_pCoroutine = null;

    GameSkeleton_Tomb m_pTomb = null;

    private void OnEnable()
    {
        m_fHideTime_Min = 5.0f;
        m_fHideTime_Max = 15.0f;
        m_fBoomPercent = 10.0f;
        m_nScore = 100;
        m_nBoomScore = 200;
    }

    private void OnDisable()
    {
        if (m_pCoroutine != null)
        {
            StopCoroutine(m_pCoroutine);
            m_pCoroutine = null;
        }
    }

    public void Active()
    {
        m_fHideTime = Random.Range(m_fHideTime_Min, m_fHideTime_Max);

        if (Random.Range(0.0f, 100.0f) < m_fBoomPercent)
        {
            m_pBoom.SetActive(true);
            ChangeAniState(E_SKELETON_STATE.E_Boom);
        }
        else
        {
            ChangeAniState(E_SKELETON_STATE.E_Idle);
            m_pBoom.SetActive(false);
        }

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void SetHole(GameSkeleton_Tomb pTomb)
    {
        m_pTomb = pTomb;
    }

    public void DeActive()
    {
        if (m_pCoroutine != null)
        {
            StopCoroutine(m_pCoroutine);
            m_pCoroutine = null;
        }
        m_pCollider.enabled = false;

        hitParticle.SetActive(false);
        boomParticle.SetActive(false);

        transform.gameObject.SetActive(false);
    }

    public void Hit()
    {
        if (!m_pCollider.enabled)
            return;

        ChangeAniState(E_SKELETON_STATE.E_Die);
    }

    public void ChangeAniState(E_SKELETON_STATE eState)
    {
        m_eState = eState;

        if (m_pCoroutine != null)
        {
            StopCoroutine(m_pCoroutine);
            m_pCoroutine = null;
        }

        switch (eState)
        {
            case E_SKELETON_STATE.E_Idle:
                m_pCoroutine = StartCoroutine(Cor_State_Idle());
                break;
            case E_SKELETON_STATE.E_Open1:
                m_pCoroutine = StartCoroutine(Cor_State_Open());
                break;
            case E_SKELETON_STATE.E_Open2:
                m_pCoroutine = StartCoroutine(Cor_State_Open());
                break;
            case E_SKELETON_STATE.E_Jump:
                m_pCoroutine = StartCoroutine(Cor_State_Jump());
                break;
            case E_SKELETON_STATE.E_Die:
                m_pCoroutine = StartCoroutine(Cor_State_Die());
                break;
            case E_SKELETON_STATE.E_Boom:
                m_pCoroutine = StartCoroutine(Cor_State_Boom());
                break;
        }
    }

    IEnumerator Cor_State_Idle()
    {
        m_pCollider.enabled = false;
        m_pAni.SetTrigger(m_eState.ToString());

        yield return new WaitForSeconds(0.2f);

        switch (Random.Range(0, 2))
        {
            case 0:
                ChangeAniState(E_SKELETON_STATE.E_Open1);
                break;
            case 1:
                ChangeAniState(E_SKELETON_STATE.E_Open2);
                break;
        }

        yield return null;
    }

    IEnumerator Cor_State_Open()
    {
        m_pAni.SetTrigger(m_eState.ToString());
        yield return new WaitForSeconds(0.3f);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Skeleton_Laugh);

        m_pCollider.enabled = true;
        yield return new WaitForSeconds(m_fHideTime);
        ChangeAniState(E_SKELETON_STATE.E_Jump);
        yield return null;
    }

    IEnumerator Cor_State_Boom()
    {
        m_pBoom.SetActive(true);
        m_pAni.SetTrigger(m_eState.ToString());

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Skeleton_FuseBomb);
        yield return new WaitForSeconds(0.3f);

        m_pCollider.enabled = true;
        yield return new WaitForSeconds(m_fHideTime);

        ChangeAniState(E_SKELETON_STATE.E_Jump);

        yield return null;
    }

    IEnumerator Cor_State_Jump()
    {
        m_pAni.SetTrigger("E_Jump");

        yield return new WaitForSeconds(1.0f);
        m_pCollider.enabled = false;
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Skeleton_AvoidSkull);

        for (float i = 0.0f; i < 1.5f; i += Time.deltaTime)
        {
            transform.position += Vector3.up * 250.0f * Time.deltaTime;
            yield return null;
        }

        m_pTomb.Close();
        yield return new WaitForSeconds(0.4f);

        DeActive();
        yield return null;
    }


    IEnumerator Cor_State_Die()
    {
        if (m_pBoom.activeSelf == true)
        {
            m_pBoom.SetActive(false);
            boomParticle.SetActive(true);
            scoreTextControl.SetScore(-m_nBoomScore);
            Message.Send<ADDScore>(new ADDScore(-m_nBoomScore));
            Message.Send<ShakeCameraMsg>(new ShakeCameraMsg(0.5f, 1.0f));
            Message.Send<ColorCameraMsg>(new ColorCameraMsg(0.1f, new Color(1, 0f, 0f, 0.7f)));

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Skeleton_Explosion);
        }
        else
        {
            hitParticle.SetActive(true);
            scoreTextControl.SetScore(m_nScore);
            Message.Send<ADDScore>(new ADDScore(m_nScore));
        }

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Skeleton_Death);

        m_pAni.SetTrigger(m_eState.ToString());
        m_pCollider.enabled = false;
        yield return new WaitForSeconds(0.4f);

        m_pTomb.Close();
        yield return new WaitForSeconds(0.4f);

        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        yield return new WaitForSeconds(0.4f);

        DeActive();
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameOctopusObj : MonoBehaviour
{
    public enum E_OCTOPUS_SPAWNDIRECT
    {
        E_LEFT,
        E_MID,
        E_RIGHT,

        E_MAX,
    }

    public Collider m_pCollider = null;
    public Animation m_pAni_Object = null;
    public Animation m_pAni_Char = null;
    public SkinnedMeshRenderer m_pRender = null;

    public Vector3[] m_pDestination = null;


    Coroutine m_pCor = null;

    int m_nDownScore = 0;
    float m_fAttackTime = 0.0f;
    Vector3 m_pCurr_Destination;
    bool m_bLife = false;

    private void OnEnable()
    {
        m_nDownScore = 1500;
        m_fAttackTime = 5.0f;
    }

    public void Destroy()
    {
        m_pCollider = null;
        m_pAni_Object = null;
        m_pAni_Char = null;
        m_pDestination = null;
        m_pRender = null;
        m_fAttackTime = 5.0f;

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
            m_pCor = null;
        }
    }

    public void Active()
    {
        m_pAni_Object.Play("Octopus_Updown");
        m_pAni_Char.Play("swim");
        m_pCollider.enabled = true;
        m_pRender.material.color = Color.white;
        m_bLife = true;
        m_pCor = StartCoroutine("Cor_Move");
    }

    public void Active(Vector3 pPos)
    {
        transform.position = pPos;
        float fDistance = GetDistance(transform.localPosition, m_pDestination[0]).z;
        int nNum = 0;
        for (int i = 1; i < m_pDestination.Length; i++)
        {
            if (GetDistance(transform.localPosition, m_pDestination[i]).z < fDistance)
            {
                nNum = i;
            }
        }
        m_pCurr_Destination = m_pDestination[nNum];
        Active();
    }

    public void DeActive()
    {
        m_pCollider.enabled = false;
    }

    IEnumerator Cor_Move()
    {
        Vector3 pPos = transform.localPosition;
        transform.LookAt(2.0f * transform.localPosition - new Vector3(313.4f, -18.8f, -185.0f));
        for (float i = 0.0f; i < m_fAttackTime; i += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(pPos, m_pCurr_Destination, i / m_fAttackTime * 1.0f);
            yield return null;
        }
        StartCoroutine("Cor_ColorBan");
        yield return new WaitForSeconds(1.5f);
        m_pCor = StartCoroutine("Cor_Attack");
        yield return null;
    }

    IEnumerator Cor_Attack()
    {
        m_pCollider.enabled = false;
        m_bLife = false;
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Octopus_Shit);
        m_pAni_Char.Play("attack");
        yield return new WaitForSeconds(0.1f);
        Message.Send<OctopusShitCreateMsg>(new OctopusShitCreateMsg(transform.position));
        yield return new WaitForSeconds(1.0f);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        m_pAni_Object.Play("Octopus_Dodge");
        yield return new WaitForSeconds(2.0f);
        DeActive();
        yield return null;
    }

    IEnumerator Cor_ColorBan()
    {
        m_pRender.material.color = Color.blue;

        bool bRe = false;

        while (m_bLife)
        {
            for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * 3.0f)
            {
                if (bRe == false)
                {
                    m_pRender.material.color = Color.Lerp(Color.white, Color.blue, i);
                }
                else
                {
                    m_pRender.material.color = Color.Lerp(Color.blue, Color.white, i);
                }
                yield return null;
            }
            bRe = !bRe;
            yield return null;
        }

        m_pRender.material.color = Color.white;
        yield return null;
    }

    public void Hit()
    {
        //if (m_bActive == false || m_bLife == false) return;
        if (m_bLife == false) return;

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
            m_pCor = null;
        }
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Octopus_Hit);
        m_pCor = StartCoroutine("Cor_Hit");
        Message.Send<ADDScore>(new ADDScore(100));
    }

    IEnumerator Cor_Hit()
    {
        // 옮길 것
        //Scene_Game_Octopus.I.m_pGame.m_pParticleMng.Create("Boom", transform.position + Vector3.back * 50.0f);
        m_bLife = false;
        m_pCollider.enabled = false;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        m_pAni_Object.Play("Octopus_Dodge");
        yield return new WaitForSeconds(2.0f);
        DeActive();
        yield return null;
    }

    Vector3 GetDistance(Vector3 stPos1, Vector3 stPos2)
    {
        Vector3 vPos;
        vPos.x = stPos2.x - stPos1.x;
        vPos.y = stPos2.y - stPos1.y;

        vPos.z = Mathf.Sqrt((vPos.x * vPos.x) + (vPos.y * vPos.y));

        return vPos;
    }
}

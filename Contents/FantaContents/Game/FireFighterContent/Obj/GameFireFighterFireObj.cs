using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi;

public class GameFireFighterFireObj : MonoBehaviour
{
    public Collider m_pCollider = null;
    public ParticleSystem m_pFire = null;
    public ParticleSystem m_pFire_Big = null;

    int m_nScore = 100;
    int m_nDownScore = 100;
    float m_fBigTime_Min = 10.0f;
    float m_fBigTime_Max = 15.0f;
    int m_nHp_Big = 3;
    float m_fBigFireAttackTime = 2.2f;

    float m_fBigTime = 0.0f;
    int m_nHp = 0;
    internal bool m_bLife = false;
    public GameObject WaterFX;
    public ScoreTextControl TextControl;

    public void Destroy()
    {
        StopAllCoroutines();
    }

    public void Active(Vector3 pPos)
    {
        transform.position = pPos;

        m_fBigTime = Random.Range(m_fBigTime_Min, m_fBigTime_Max);
        m_nHp = 1;
        m_pCollider.enabled = true;
        m_pFire_Big.gameObject.SetActive(false);
        m_pFire.gameObject.SetActive(true);
        m_pFire.Play();
        m_bLife = true;
        StartCoroutine(Cor_Burn());

        m_pFire.transform.localScale = Vector3.one;
    }

    IEnumerator Cor_Burn()
    {

        yield return new WaitForSeconds(m_fBigTime);
        m_nHp = m_nHp_Big;
        m_pFire_Big.gameObject.SetActive(true);
        m_pFire_Big.Play();
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Init);

        while (m_bLife == true)
        {
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Big);
            //Scene_Game_FireFighter.I.m_pGame.Score_Down(m_nDownScore, transform.position);
            yield return new WaitForSeconds(m_fBigFireAttackTime);
        }

        yield return null;
    }

    public void DeActive()
    {
        m_pCollider.enabled = false;
        m_pCollider.gameObject.SetActive(false);
    }

    public void Hit()
    {
        m_nHp--;
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Hit);
        FXCreate(WaterFX);
        if (m_nHp <= 0)
        {
            Die();
        }
        else
        {
            if (m_nHp == 1)
            {
                if (m_pFire_Big.gameObject.activeSelf == true) m_pFire_Big.Stop();
            }
            StartCoroutine("Cor_HitDelay");
        }
    }

    IEnumerator Cor_HitDelay()
    {
        m_pCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        m_pCollider.enabled = true;
    }

    void FXCreate(GameObject obj)
    {
        GameObject createObj;
        Vector3 pos;
        createObj = Instantiate(obj);
        createObj.SetActive(false);
        createObj.transform.SetParent(this.transform.parent);
        pos = this.transform.position;
        createObj.SetActive(true);
        createObj.transform.position = pos;
        DestroyFx(createObj);
    }
    void DestroyFx(GameObject fx)
    {
        Destroy(fx, 2f);
    }

    public void Die()
    {
        StopCoroutine("Cor_Burn");
        StartCoroutine("Cor_Die");

        //옮길 것
        //Scene_Game_FireFighter.I.m_pGame.Score_Up(m_nScore, transform.position);
    }

    IEnumerator Cor_Die()
    {
        m_bLife = false;
        m_pFire.Stop();
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireFighter_Off);
        m_pCollider.enabled = false;
        if (m_pFire_Big.gameObject.activeSelf == true) m_pFire_Big.Stop();

        for (float i = 0.0f; i < 1.2f; i += Time.deltaTime)
        {
            m_pFire.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, i / 1.2f * 1.0f);
            yield return null;
        }
        //yield return new WaitForSeconds(1.2f);
        DeActive();
        yield return null;
    }

    public void HitPoint(Vector3 hitpoint)
    {
        TextControl.transform.parent.position = hitpoint;
        TextControl.SetScore(100);
    }
}

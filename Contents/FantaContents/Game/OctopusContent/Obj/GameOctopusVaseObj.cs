using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CellBig.Contents.Event;

public class GameOctopusVaseObj : MonoBehaviour
{
    public ParticleSystem m_pWhriParticle = null;
    public Rigidbody m_pRigi = null;
    public MeshRenderer m_pRender = null;
    public Collider m_pCollider = null;

    public GameObject m_pVase = null;
    public GameOctopusVaseFragment m_pFragment = null;
    public ScoreTextControl TextControl;

    float m_fBreakTime_Min = 0.0f;
    float m_fBreakTime_Max = 0.0f;

    float m_fBreakTime = 0.0f;
    int m_nScore = 0;

    bool m_bLife = false;
    private void OnEnable()
    {
        m_bLife = false;
        m_fBreakTime_Min = 3.0f;
        m_fBreakTime_Max = 7.0f;
        m_pFragment.Enter();
        m_pFragment.gameObject.SetActive(false);
    }

    public void Destroy()
    {
        //m_pWhriParticle = null;
        //m_pRigi = null;
        //m_pRender = null;
        //m_pVase = null;
       
        if (m_pFragment != null)
        {
            m_pFragment.Destroy();
            //m_pFragment = null;
        }
        //m_pCollider = null;
    }

    public void Active()
    {
        m_fBreakTime = UnityEngine.Random.Range(m_fBreakTime_Min, m_fBreakTime_Max);
        m_nScore = 100;

        m_pRigi.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        m_pRigi.AddForce(0.0f, UnityEngine.Random.Range(70000.0f, 100000.0f) * -1.0f, 0);

        m_pRender.material.SetColor("_SpecColor", Color.white);

        m_pVase.SetActive(true);
        m_pFragment.gameObject.SetActive(false);

        m_bLife = true;
        m_pCollider.enabled = true;

        transform.localPosition = new Vector3(UnityEngine.Random.Range(-15.0f, 655.0f),
                                            262.0f);
        transform.localRotation = Quaternion.Euler(0, 0, Rand._Rand(0, 70) - 35);

        StartCoroutine("Cor_Move");
    }

    public void DeActive()
    {
        m_bLife = false;
        StopCoroutine("Cor_ColorBan");
        StopCoroutine("Cor_Move");
        StopCoroutine("Cor_Die");
        m_pFragment.gameObject.SetActive(false);
        m_pCollider.enabled = false;
        this.gameObject.SetActive(false);
    }

    IEnumerator Cor_Move()
    {
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Octopus_InitPot);
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Octopus_Move);
        bool bColor = false;
        for (float i = 0.0f; i < m_fBreakTime; i += Time.deltaTime)
        {

            if (bColor == false && i > m_fBreakTime / 2.0f)
            {
                bColor = true;
                StartCoroutine("Cor_ColorBan");
            }
            yield return null;
        }

        Die();

        yield return null;
    }

    public void Die(bool bHit = false)
    {
        StopCoroutine(Cor_Move());
        StartCoroutine(Cor_Die(bHit));
    }

    IEnumerator Cor_Die(bool bHit)
    {
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Octopus_Break);
        m_pCollider.enabled = false;
        m_pWhriParticle.gameObject.SetActive(false);
        m_pVase.SetActive(false);
        // 둘 다 옮길 것
        m_pFragment.gameObject.SetActive(true);
        m_pFragment.AddForce(5000.0f);

        // 옮길 것 
        //if (bHit == true) Scene_Game_Octopus.I.m_pGame.Score_Up(m_nScore, transform.position);
        if (!bHit) Message.Send<OctopusCreateMsg>(new OctopusCreateMsg(transform.position));
       
        // 옮길 것
        //Scene_Game_Octopus.I.m_pGame.m_pParticleMng.Create("BubbleHit", transform.position);
        yield return new WaitForSeconds(2.0f);
        DeActive();
        yield return null;
    }

    public void HitPoint(Vector3 hitpoint)
    {
        Message.Send<CellBig.UI.Event.ADDScore>(new CellBig.UI.Event.ADDScore(100));
        TextControl.transform.parent.position = hitpoint;
        TextControl.SetScore(100);
    }

    IEnumerator Cor_ColorBan()
    {
        m_pRender.material.color = Color.red;
        m_pRender.material.SetColor("_SpecColor", Color.red);

        bool bRe = false;

        while (m_bLife)
        {
            for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * 3.0f)
            {
                if (bRe == false)
                {
                    m_pRender.material.color = Color.Lerp(Color.white, Color.red, i);
                }
                else
                {

                    m_pRender.material.color = Color.Lerp(Color.red, Color.white, i);
                }
                m_pRender.material.SetColor("_SpecColor", m_pRender.material.color);
                yield return null;
            }
            bRe = !bRe;
            yield return null;
        }

        m_pRender.material.color = Color.white;
        m_pRender.material.SetColor("_SpecColor", m_pRender.material.color);
        yield return null;
    }

    public class Rand
    {
        static uint[] state = new uint[16];
        static uint index = 0;

        static Rand()
        {
            System.Random random = new System.Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 16; i++)
            {
                state[i] = (uint)random.Next();
            }
        }

        internal static int _Rand(int minValue, int maxValue)
        {
            return (int)((Next() % (maxValue - minValue)) + minValue);
        }

        public static uint Next(uint maxValue)
        {
            return Next() % maxValue;
        }

        public static uint Next()
        {
            uint a, b, c, d;

            a = state[index];
            c = state[(index + 13) & 15];
            b = a ^ c ^ (a << 16) ^ (c << 15);
            c = state[(index + 9) & 15];
            c ^= (c >> 11);
            a = state[index] = b ^ c;
            d = a ^ ((a << 5) & 0xda442d24U);
            index = (index + 15) & 15;
            a = state[index];
            state[index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);

            return state[index];
        }
    }
}

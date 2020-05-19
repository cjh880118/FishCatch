using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Contents.Event;
using System;

public class GamePictureObj : MonoBehaviour
{
    public SpriteRenderer m_pSprite_Picture = null;
    public SpriteRenderer m_pIce = null;
    public SpriteRenderer m_pSprite_Frame = null;

    public Sprite[] m_pFrames = null;

    public bool isCheckInit = false;

    internal string m_sName;
    internal string m_sOwner;

    bool m_bIceOff = false;
    bool m_bInScreen = false;

    float m_fWidth_Interval = 1.0f;
    float m_fHeight_Interval = 1.0f;
    int m_nCurrMaxBlock = 0;

    public GamePictureBubble m_pBubbleMng;

    public void Enter()
    {
        m_bInScreen = false;
        m_pBubbleMng = transform.Find("BubbleMng").GetComponent<GamePictureBubble>();
        m_pBubbleMng.Enter();
    }

    public void Destroy()
    {
        //m_pSprite_Picture = null;
        //m_pIce = null;
        //m_pSprite_Frame = null;
        if (m_pBubbleMng != null) m_pBubbleMng.Destroy();
        //m_pBubbleMng = null;

        //m_pFrames = null;
    }

    public void PictureUpdate()
    {
        //if (m_bInScreen) m_pBubbleMng.update
    }

    public void MoveToScreen(bool bInScreen)
    {
        StartCoroutine(Cor_MoveScreen(bInScreen));
    }

    IEnumerator Cor_MoveScreen(bool bInScreen)
    {
        m_bInScreen = false;
        if (bInScreen == true)
        {
            for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 1.3f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, i);
                yield return null;
            }
            transform.localPosition = Vector3.zero;
            m_bInScreen = true;
            m_bIceOff = true;
        }
        else
        {
            Vector3 a = new Vector3(-17.8f, 0.0f, 0.0f);
            for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 1.3f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, a, i);
                yield return null;
            }
            transform.localPosition = a;
            m_bInScreen = false;
            //m_pBubbleMng.AllDie();
        }
        yield return null;
    }

    public void IceOff()
    {
        if (m_bIceOff == false) return;
        //  StartCoroutine("Cor_IceOff");
        // 옮기기
        int nCurr = m_pBubbleMng.GetCurrBubble1List();
        Debug.LogError(m_pBubbleMng.GetCurrBubble1List());
        float fPer = (float)nCurr / (float)m_nCurrMaxBlock * 0.5f;
        //  if (fPer <= 0.1f) fPer = 0.1f;

        m_pIce.color = Color.Lerp(Color.clear, Color.white, 0.5f + fPer);
    }

    public void IceOff2()
    {
        StartCoroutine("Cor_IceOff");
        m_bIceOff = false;
    }

    IEnumerator Cor_IceOff()
    {
        Color a = m_pIce.color;
        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * 0.8f)
        {
            m_pIce.color = Color.Lerp(a, Color.clear, i);
            yield return null;
        }
        yield return null;
    }

    void ReadMapAndCreate(List<Dictionary<string, string>> data)
    {
        int nRI = 0;
        int nX, nY = 0;
        float fStartX, fStartY = 0.0f;
        nX = data[0].Count;
        nY = data.Count;

        fStartX = 1.9f;
        fStartY = 5.75f;
        float a = 0.0f;

        for (int i = 0; i < nY; i++)
        {
            int j = 0;
            foreach (string pObj in data[i].Values)
            {
                a = 0.0f;
                if (i % 2 == 0) a = 0.4f;
                if (pObj == "1")
                {
                    Vector3 pSPos = new Vector3(fStartX + a + j * m_fWidth_Interval, fStartY - i * m_fHeight_Interval, 0.0f);
                    m_pBubbleMng.Create(pSPos, j, i, false);
                    //Message.Send<PictureBubbleCreateMsg>(new PictureBubbleCreateMsg(pSPos, j, i, false));

                    nRI++;
                }
                else if (pObj == "2")
                {
                    Vector3 pSPos = new Vector3(fStartX + a + j * m_fWidth_Interval, fStartY - i * m_fHeight_Interval, 0.0f);
                    m_pBubbleMng.Create(pSPos, j, i, true);
                    //Message.Send<PictureBubbleCreateMsg>(new PictureBubbleCreateMsg(pSPos, j, i, true));
                    nRI++;
                }

                j++;
            }
        }
        m_nCurrMaxBlock = nRI;
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Picture_BubbleInit);
        

        return;
    }

    public void Create(string sName, string sOwner, Sprite pSprite, List<Dictionary<string, string>> data, bool bFirst = false)
    {
        Debug.Log("GanePictureObj : Create");

        if (bFirst == false) transform.localPosition = new Vector3(17.8f, 0.0f, 0.0f);

        m_sName = sName;
        m_sOwner = sOwner;
        m_pSprite_Picture.sprite = pSprite;

        m_pIce.color = Color.white;
        m_pSprite_Frame.sprite = m_pFrames[Rand._Rand(0, m_pFrames.Length)];
        ReadMapAndCreate(data);
        // Scene_Game.I.m_pGameRoot.m_pSound.Play("Init_Bubble");
    }

    public class Rand
    {
        static uint[] state = new uint[16];
        static uint index = 0;

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

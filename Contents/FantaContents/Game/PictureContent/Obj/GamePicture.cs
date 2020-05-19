using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using CellBig.Contents.Event;

public class GamePicture : MonoBehaviour
{
    [System.Serializable]
    public struct PictureInfo
    {
        public Sprite m_pSprite;
        public string m_sName;
        public string m_sAuthor;
    }

    public TextAsset[] m_pMapData = null;
    public PictureInfo[] m_pPictureInfo = null;
    public GamePictureObj[] m_pPicture = null;


    public Animator m_pTag = null;
    public Text m_pTag_Name = null;
    public Text m_pTag_Person = null;

    int m_nCurrPic = -1;
    int m_nCurrObject = -1;

    int currentPictureNum;

    private void Awake()
    {
        for (int i = 0; i < m_pPicture.Length; i++)
        {
            m_pPicture[i].Enter();
        }

        m_nCurrPic = -1;
        m_nCurrObject = -1;

        AddMessage();
    }
    public void Enter()
    {   
        Create(true);
        m_pTag.gameObject.SetActive(false);
    }

    public void Destroy()
    {
        m_pMapData = null;
        m_pPictureInfo = null;
        for (int i = 0; i < m_pPicture.Length; i++)
        {
            m_pPicture[i].Destroy();
        }
        m_pPicture = null;
        m_pTag = null;
        m_pTag_Name = null;
        m_pTag_Person = null;
    

        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<CurrentPictureNumMsg>(OnCurrentPictureNumMsg);
        Message.AddListener<PictureBubbleTagIn>(Tag_In);
        Message.AddListener<PictureBubbleTagOut>(Tag_Out);
        Message.AddListener<PictureBubbleCreate>(CreateMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<CurrentPictureNumMsg>(OnCurrentPictureNumMsg);
        Message.RemoveListener<PictureBubbleTagIn>(Tag_In);
        Message.RemoveListener<PictureBubbleTagOut>(Tag_Out);
        Message.RemoveListener<PictureBubbleCreate>(CreateMsg);
    }

    void OnCurrentPictureNumMsg(CurrentPictureNumMsg msg)
    {
        currentPictureNum = msg.Num;
    }

    IEnumerator PlayPicture()
    {
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.C) == true)
            {
                Create();
            }
            //if (m_pTag.gameObject.activeSelf == false) m_pPicture[m_nCurrObject].IceOff();

            yield return null;
        }
    }

    public void CreateMsg(PictureBubbleCreate msg)
    {
        Create();
    }


    public void Create(bool bFirst = false)
    {
        int nRand = -1;
        for (int i = 0; i < 300; i++)
        {
            nRand = Rand._Rand(0, m_pPictureInfo.Length);
            if (nRand != m_nCurrPic)
            {
                m_nCurrPic = nRand;
                break;
            }
        }
        if (m_nCurrObject != -1) { m_pPicture[m_nCurrObject].MoveToScreen(false); }
        if (m_nCurrObject == -1) m_nCurrObject = 0;
        else if (m_nCurrObject == 0) m_nCurrObject = 1;
        else if (m_nCurrObject == 1) m_nCurrObject = 0;

        List<Dictionary<string, string>> data = null;
        data = CSVReader.Read(m_pMapData[0]);

        string picName = Localizing_Mng.I.ConvertLanguage(Int32.Parse(m_pPictureInfo[m_nCurrPic].m_sName));
        string picAuthor = Localizing_Mng.I.ConvertLanguage(Int32.Parse(m_pPictureInfo[m_nCurrPic].m_sAuthor));

        m_pPicture[m_nCurrObject].Create(picName,
            picAuthor, m_pPictureInfo[m_nCurrPic].m_pSprite,
            data, bFirst);

        m_pTag_Name.text = picName;
        m_pTag_Person.text = picAuthor;

        m_pPicture[m_nCurrObject].MoveToScreen(true);
    }

    public void Tag_In(PictureBubbleTagIn msg)
    {
        m_pTag.gameObject.SetActive(true);
        m_pTag.SetTrigger("IN");
        m_pPicture[m_nCurrObject].IceOff2();
    }

    public void Tag_Out(PictureBubbleTagOut msg)
    {
        StartCoroutine("Cor_TagOut");
    }

    IEnumerator Cor_TagOut()
    {
        m_pTag.SetTrigger("OUT");
        yield return new WaitForSeconds(0.5f);
        m_pTag.gameObject.SetActive(false);
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

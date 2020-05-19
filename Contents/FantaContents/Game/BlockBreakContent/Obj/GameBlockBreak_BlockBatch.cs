using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using CellBig.Contents.Event;

public enum BlockType
{
    Normal,
    Bomb,
}

public class GameBlockBreak_BlockBatch : MonoBehaviour
{
    public List<string> data = new List<string>();
    public List<Vector3> dataVec = new List<Vector3>();
    public List<int> dataX = new List<int>();
    public List<int> dataY = new List<int>();

    float m_fWidth_Interval = 32.0f;
    float m_fHeight_Interval = 32.0f;
    float m_fStartPosX = 0.0f;
    float m_fStartPosY = 0.0f;
    float m_fEndPosX = 700.0f;
    float m_fEndPosY = -450.0f;
    int m_nCurrMapNum = 0;

    float offSetX = -330;
    float offSetY = 200;
    float offSetZ = 400;

    //float m_fWidth_Interval = 32.0f;
    //float m_fHeight_Interval = 32.0f;
    //float m_fStartPosX = 0.0f;
    //float m_fStartPosY = 155.0f;
    //float m_fEndPosX = 628.0f;
    //float m_fEndPosY = -220.0f;
    //int m_nCurrMapNum = 0;

    //float offSetX = -300;
    //float offSetY = 15;
    //float offSetZ = 400;

    public List<GameBlockBreak_Block> blockList = new List<GameBlockBreak_Block>();

    public TextAsset[] m_pMapData = null;

    bool isBatch;

    public void CreateNewBlockMap()
    {
        blockList.Clear();

        int nRand = -1;
        for (int i = 0; i < 100; i++)
        {
            nRand = UnityEngine.Random.Range(0, m_pMapData.Length);
            if (nRand != m_nCurrMapNum)
                break;
        }

        ReadMapAndCreate(nRand);
    }

    void ReadMapAndCreate(int nNum)
    {
        m_nCurrMapNum = nNum;

        StringReader sr = new StringReader(m_pMapData[nNum].text);

        string source = sr.ReadLine();
        string[] values;                // 쉼표로 구분된 데이터들을 저장할 배열 (values[0]이면 첫번째 데이터 )
        int nRI = 0;
        int nX, nY = 0;

        data.Clear();
        dataVec.Clear();
        dataX.Clear();
        dataY.Clear();

        while (source != null)
        {
            values = source.Split(',');

            if (values.Length == 0)
            {
                sr.Close();
                return;
            }
            if (nRI == 0)
            {
                nX = Convert.ToInt16(values[0]);
                nY = Convert.ToInt16(values[1]);

                m_fWidth_Interval = Mathf.Abs(m_fStartPosX - m_fEndPosX) / (float)(nX - 1);
                m_fHeight_Interval = Mathf.Abs(m_fStartPosY - m_fEndPosY) / (float)(nY - 1);
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Vector3 pSPos = new Vector3(m_fStartPosX + (i * m_fWidth_Interval), m_fStartPosY - ((nRI - 1) * m_fHeight_Interval), 0.0f);
                    pSPos = new Vector3(pSPos.x + offSetX, pSPos.y + offSetY, pSPos.z + offSetZ);

                    data.Add(values[i]);
                    dataVec.Add(pSPos);
                    dataX.Add(i);
                    dataY.Add((nRI - 1));
                }
            }

            nRI++;
            source = sr.ReadLine();    // 한줄 읽는다.
        }

        Message.Send<BlockDataMsg>(new BlockDataMsg(data, dataVec, dataX, dataY));
    }

    public void Freeze(int nX, int nY)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            GameBlockBreak_Block block = blockList[i];
            if (block == null || block.m_bLife == false || block.m_pCollider.enabled == false) continue;

            if (Mathf.Abs(nX - block.m_nX) <= 1 && Mathf.Abs(nY - block.m_nY) <= 1)
            {
                block.Freeze();
                SoundManager.Instance.PlaySound((int)SoundType_GameFX.BlockBreak_FreezingBubble);
            }
        }
    }

    public IEnumerator BlockUpdate()
    {
        blockList.Clear();
        yield return new WaitForSeconds(1.0f);
        CreateNewBlockMap();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents.Event;

public class GameWeston_ObjectControl : MonoBehaviour
{
    [System.Serializable]
    public struct Target_Pattern
    {
        public int nSpawnNum;
        public int nBoomNum;
        public float fWaitTime;
        public float fAfterTime;
        public Target_Pattern(int nSpawn, int nBoom, float fWait, float fAfter)
        {
            nSpawnNum = nSpawn;
            nBoomNum = nBoom;
            fWaitTime = fWait;
            fAfterTime = fAfter;
        }
    }

    List<Target_Pattern> m_pPattern = null;

    [SerializeField]
    GameObject targetBox;

    public GameWeston_WestonTarget[] gameWeston_WestonTargets;

    public List<int> isPossibleCreateList = new List<int>();

    public TextAsset[] m_pMapData = null;

    int m_nCurrPa = 0;
    float m_fTime = 0.0f;

    void Awake()
    {
        gameWeston_WestonTargets = targetBox.GetComponentsInChildren<GameWeston_WestonTarget>(true);

        m_pPattern = new List<Target_Pattern>();
        ReadData(0);
    }

    public IEnumerator Cor_PlayContent_Weston()
    {
        while (true)
        {
            m_fTime += Time.deltaTime;
            if (m_fTime > m_pPattern[m_nCurrPa].fAfterTime)
            {
                int[] nBoomNum = new int[m_pPattern[m_nCurrPa].nSpawnNum];
                for (int i = 0; i < m_pPattern[m_nCurrPa].nSpawnNum; i++)
                {
                    nBoomNum[i] = 1;
                }
                int nN = 0;
                for (int i = 0; i < m_pPattern[m_nCurrPa].nBoomNum; i++)
                {
                    nN = Random.Range(0, m_pPattern[m_nCurrPa].nSpawnNum);
                    if (nBoomNum[nN] == 2)
                    {
                        i--;
                        continue;
                    }
                    nBoomNum[nN] = 2;
                }

                for (int i = 0; i < m_pPattern[m_nCurrPa].nSpawnNum; i++)
                {
                    for (int j = 0; j < 300; j++)
                    {
                        int nRan = Random.Range(0, gameWeston_WestonTargets.Length);
                        if (gameWeston_WestonTargets[nRan].m_bLife == false)
                        {
                            if (nBoomNum[i] == 2)
                            {
                                gameWeston_WestonTargets[nRan].Open(m_pPattern[m_nCurrPa].fWaitTime, true);
                            }
                            else
                            {
                                gameWeston_WestonTargets[nRan].Open(m_pPattern[m_nCurrPa].fWaitTime, false);
                            }

                            break;
                        }
                    }
                }

                m_fTime = 0.0f;
                m_nCurrPa++;
                if (m_nCurrPa >= m_pPattern.Count) m_nCurrPa = 0;
            }

            yield return null;
        }
    }

    void ReadData(int nNum)
    {
        List<Dictionary<string, string>> data = null;
        data = CSVReader.Read(m_pMapData[nNum]);
        int nX = data[0].Count;
        int nY = data.Count;
        for (int i = 0; i < nY; i++)
        {
            m_pPattern.Add(new Target_Pattern(int.Parse(data[i]["SpawnNum"]),
                int.Parse(data[i]["BoomNum"]),
                float.Parse(data[i]["WaitTime"]),
                float.Parse(data[i]["NextTime"])));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFireFighterBuild : MonoBehaviour
{
    [SerializeField]
    private GameFireFighterBuildTile[] m_pTiles = null;


    int m_nCurrTile = -1;
    int m_nCnt = 0;
    int m_nMinPosNum = 0;

    //float m_fIntervalValue = -57.0f;
    float m_fIntervalValue = -560.0f;

    private void Awake()
    {
        m_nCurrTile = 0;
        m_nCnt = 0;
    }

    public void Enter()
    {

        for (int i = 0; i < m_pTiles.Length; i++)
        {
            m_pTiles[i].Enter();
        }

        
        //m_nCurrTile = 0;
    }

    public void Destroy()
    {
        if (m_pTiles != null)
        {
            for (int i = 0; i < m_pTiles.Length; i++)
            {
                m_pTiles[i].Destroy();
                m_pTiles[i] = null;
            }
            m_pTiles = null;
        }
    }

    
    public void SetNextTile()
    {
        int nNextTile = m_nCurrTile + 1;
        if (nNextTile >= m_pTiles.Length) nNextTile = 0;

        m_nCurrTile = nNextTile;
    }

    public void MoveTile()
    {
        m_nCnt++;
        if (m_nCnt >= 2)
        {
            m_pTiles[GetMinPosNum()].SetNextTilePos(m_pTiles[GetMaxPosNum()].gameObject, m_fIntervalValue * (m_pTiles.Length));
        }
    }

    int GetMinPosNum()
    {
        int nMin = 0;
        float fPos = m_pTiles[0].transform.localPosition.x;
        for (int i = 0; i < m_pTiles.Length; i++)
        {
            if (fPos < m_pTiles[i].transform.localPosition.x)
            {
                fPos = m_pTiles[i].transform.localPosition.x;
                nMin = i;
            }
        }
        return nMin;
    }
    int GetMaxPosNum()
    {
        int nMin = 0;
        float fPos = m_pTiles[0].transform.localPosition.x;
        for (int i = 0; i < m_pTiles.Length; i++)
        {
            if (fPos < m_pTiles[i].transform.localPosition.x)
            {
                fPos = m_pTiles[i].transform.localPosition.x;
                nMin = i;
            }
        }
        return nMin;
    }


    public GameObject GetCurrTileTruckPos()
    {
        return m_pTiles[m_nCurrTile].GetTruckPoint();
    }

    public GameObject GetNextTileTruckPos()
    {
        int nNextTile = m_nCurrTile + 1;
        if (nNextTile >= m_pTiles.Length) nNextTile = 0;
        return m_pTiles[nNextTile].GetTruckPoint();
    }

    public GameFireFighterBuildTile GetCurrTile()
    {
        return m_pTiles[m_nCurrTile];
    }
}

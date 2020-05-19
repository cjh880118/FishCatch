using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameFireFighterBuildTile : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pFirePoint = null;
    [SerializeField]
    private GameObject m_pTruckPoint = null;

    internal List<GameObject> m_pPoints = null;

    Vector3 m_pFirstPos;

    public void Enter()
    {
        m_pPoints = new List<GameObject>();
        m_pPoints.Clear();
        m_pFirstPos = transform.localPosition;
        for (int i = 0; i < m_pFirePoint.transform.childCount; ++i)
        {
            m_pPoints.Add(m_pFirePoint.transform.GetChild(i).gameObject);
            m_pFirePoint.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
        //  Debug.Log(m_pFirePoint.transform.childCount);
    }

    public void Destroy()
    {
        if (m_pPoints != null)
        {
            m_pPoints.Clear();
            m_pPoints = null;
        }
        m_pTruckPoint = null;
        m_pFirePoint = null;
    }

    public GameObject GetRandPoint()
    {
        if (m_pPoints.Count == 0) return null;

        return m_pPoints[Rand._Rand(0, m_pPoints.Count)];
    }

    public void SetNextTilePos(GameObject pNextTile, float fInterval)
    {
        Vector3 pPos = transform.localPosition;
        pPos.x = pNextTile.transform.localPosition.x + (fInterval);
        transform.localPosition = pPos;
    }

    public GameObject GetTruckPoint()
    {
        return m_pTruckPoint;
    }

    public void SetFirstPos()
    {
        transform.localPosition = m_pFirstPos;
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



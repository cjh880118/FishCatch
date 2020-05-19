using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents.Event;

public class GameSlimeSlime : MonoBehaviour
{
    int m_nMinCount = 3;
    int m_nMaxCount = 20;
    bool m_bEndGame = false;
    public bool m_bMoveSound = false;

    float m_fSpawnTime = 0.0f;
    float m_fSpawnTime_Ago = 0.0f;
    float m_fSpawnTime_Ago_Min = 1.0f;
    float m_fSpawnTime_Ago_Max = 5.0f;
    float m_fDistance = 0.1f;
    float[] m_fCreatArr01 = { 30.0f, 20.0f, 20.0f, 10.0f, 10.0f, 10.0f };
    float[] m_fCreatArr02 = { 10.0f, 10.0f, 10.0f, 20.0f, 20.0f, 30.0f };

    public List<GameObject> PoolObject = new List<GameObject>();
    public float CurrTime;
    public float MaxTime;
    public int PoolCount = 0;

    int RandomChoose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public void Enter()
    {
        m_fSpawnTime_Ago = 0.0f;
        m_bEndGame = false;
        DestroySlime();
        StopCoroutine(PlaySlime());
        StartCoroutine(PlaySlime());
    }

    public void SetupList(CellBig.Common.ObjectPool objpool)
    {
        for (int i = 0; i < objpool.transform.childCount; i++)
            PoolObject.Add(objpool.transform.GetChild(i).gameObject);
    }

    public void DestroySlime()
    {
        StopAllCoroutines();
        //if (transform.childCount != 0)
        for (int i = 0; i < PoolObject.Count; i++)
        {
            PoolObject[i].GetComponent<GameSlimeSlimeObj>().DeActive();
            PoolObject[i].GetComponent<GameSlimeSlimeObj>().Destroy();
        }
        
    }

    IEnumerator PlaySlime()
    {
        while(true)
        {
            CurrTime -= Time.deltaTime;

            if (PoolObject.Count < m_nMaxCount) m_fSpawnTime += Time.deltaTime;

            if (m_fSpawnTime > m_fSpawnTime_Ago)
            {
                Create();
                m_fSpawnTime_Ago = Random.Range(m_fSpawnTime_Ago_Min, m_fSpawnTime_Ago_Max);
                m_fSpawnTime = 0.0f;
            }

            GameSlimeSlimeObj pTempVirus = null;
            Vector3 pPos = Vector3.zero;
            for (int i = 0; i < PoolObject.Count; i++)
            {
                pTempVirus = PoolObject[i].GetComponent<GameSlimeSlimeObj>();
               
                pPos = pTempVirus.transform.position;

                if(pTempVirus.m_bMove && m_bMoveSound == false)
                {
                    StartCoroutine(Cor_MoveSound());
                }

                if (pTempVirus.m_bVirusLive)
                {
                    if ((pTempVirus.m_nVirusType == 2 || pTempVirus.m_nVirusType == 3 || pTempVirus.m_nVirusType == 4) &&
                        pTempVirus.m_fLifeTime > 3.0f)
                    {
                        pTempVirus.m_bVirusLive = false;
                        Create(Random.Range(0, 5), (pTempVirus.transform.position + Vector3.left * m_fDistance));
                        Create(Random.Range(0, 5), (pTempVirus.transform.position + Vector3.right * m_fDistance));
                    }
                    else if (pTempVirus.m_nVirusType == 5 && pTempVirus.m_fLifeTime > 4.0f)
                    {
                        pTempVirus.m_bVirusLive = false;
                        Create(5, (pTempVirus.transform.position + Vector3.left * m_fDistance));
                        Create(5, (pTempVirus.transform.position + Vector3.right * m_fDistance));
                        Create(5, (pTempVirus.transform.position + Vector3.up * m_fDistance));
                        Create(5, (pTempVirus.transform.position + Vector3.down * m_fDistance));
                    }
                }
            }
            if (PoolObject.Count == 0) Create();
            if (CurrTime == 0 && m_bEndGame == false)
            {
                m_bEndGame = true;
                EndAllDie();

            }

            yield return null;
        }
    }

    public void Create()
    {
        if (PoolObject.Count >= m_nMaxCount) return;
        int nRandType = 0;

        if (GetCurGameTimeRatio() > 0.5f) nRandType = RandomChoose(m_fCreatArr01);
        else nRandType = RandomChoose(m_fCreatArr02);

        string sRandColor = "";
        switch (nRandType)
        {
            case 0:
                sRandColor = "Blue";
                break;
            case 1:
                sRandColor = "Yellow";
                break;
            case 2:
                sRandColor = "Orange";
                break;
            case 3:
                sRandColor = "Green";
                break;
            case 4:
                sRandColor = "Violet";
                break;
            case 5:
                sRandColor = "Red";
                break;
            default:
                sRandColor = "Blue";
                break;
        }

        if (transform.parent.parent.Find("Virus_" + sRandColor + "Pool").GetChild(0).gameObject != null)
        {
            GameObject tempObj = transform.parent.parent.Find("Virus_" + sRandColor + "Pool").GetChild(0).gameObject;
            tempObj.GetComponent<GameSlimeSlimeObj>().SetMng(this);
            tempObj.transform.SetParent(this.transform);
            tempObj.SetActive(true);
            tempObj.GetComponent<GameSlimeSlimeObj>().Active(true, nRandType);
            PoolObject.Add(tempObj);
        }

    }

    public void Create(int nObjNum, Vector3 pPos)
    {
        if (PoolObject.Count >= m_nMaxCount) return;
        int nRandType = 0;

        string sRandColor = "";

        switch (nObjNum)
        {
            case 0:
                sRandColor = "Blue";
                break;
            case 1:
                sRandColor = "Yellow";
                break;
            case 2:
                sRandColor = "Orange";
                break;
            case 3:
                sRandColor = "Green";
                break;
            case 4:
                sRandColor = "Violet";
                break;
            case 5:
                sRandColor = "Red";
                break;
            default:
                sRandColor = "Blue";
                break;
        }

        GameObject tempObj =
        transform.parent.parent.Find("Virus_" + sRandColor + "Pool").GetChild(0).gameObject;
        Debug.LogError(tempObj.name);
        tempObj.GetComponent<GameSlimeSlimeObj>().SetMng(this);
        tempObj.transform.SetParent(this.transform);
        tempObj.SetActive(true);
        tempObj.GetComponent<GameSlimeSlimeObj>().Active(nObjNum, pPos);
        PoolObject.Add(tempObj);
    }

    public void CheckCreate()
    {
        if (PoolObject.Count < m_nMinCount) Create();
    }

    public void EndAllDie()
    {
        for (int i = 0; i < PoolObject.Count; i++)
        {
            GameSlimeSlimeObj pSrc = PoolObject[i].GetComponent<GameSlimeSlimeObj>();
            if (pSrc == null || pSrc.m_pCollider == null || pSrc.m_pCollider.enabled == false) return;
            pSrc.EndDie();
        }
    }

    float GetCurGameTimeRatio()
    {
        return (CurrTime / MaxTime);
    }

    IEnumerator Cor_MoveSound()
    {
        m_bMoveSound = true;
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Slime_Jump);
        yield return new WaitForSeconds(0.2f);
        m_bMoveSound = false;
        yield return null;
    }
}

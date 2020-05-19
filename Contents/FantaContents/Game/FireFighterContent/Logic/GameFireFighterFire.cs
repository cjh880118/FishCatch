using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents.Event;

public class GameFireFighterFire : MonoBehaviour
{
    public Camera m_pViewCamera = null;
    public GameFireFighterBuild Build;
    List<string> m_pCheckList = new List<string>();

    public List<GameObject> FireList = new List<GameObject>();
    public int FireCount = 0;
    public void OnEnable()
    {
        StartCoroutine(Cor_Fire());
    }

    public void Destroy()
    {
        FireCount = 0;
        AllDie();
        StopAllCoroutines();
    }

    public void SetupList()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            FireList.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    IEnumerator Cor_Fire()
    {
        while(true)
        {
            if (!IsLiveObject())
            {
                Message.Send<GameFireFighterNextLevelMsg>(new GameFireFighterNextLevelMsg());
            }

            yield return null;
        }
    }

    public void CreateObject(int nNum)
    {
        GameObject pCheckObj = null;
        m_pCheckList.Clear();

        for (int i = 0; i < nNum; i++)
        {
            while (true)
            {
                pCheckObj = Build.GetCurrTile().GetRandPoint();
                for (int j = 0; j < m_pCheckList.Count; j++)
                {
                    if (m_pCheckList[j] == pCheckObj.name)
                    {
                        continue;
                    }
                }
                break;
            }
            m_pCheckList.Add(pCheckObj.name);

            if (FireCount < FireList.Count)
            {
                FireList[FireCount].SetActive(true);
                FireList[FireCount].GetComponent<GameFireFighterFireObj>().Active(pCheckObj.transform.position);
                FireCount++;
            }
        }
    }

    public void AllDie()
    {
        for (int i = 0; i < FireList.Count; i++)
        {
            if (FireList[i].GetComponent<GameFireFighterFireObj>().m_bLife == true)
            {
                //FireList[i].GetComponent<GameFireFighterFireObj>().Die();
                FireList[i].GetComponent<GameFireFighterFireObj>().Destroy();
            }
        }
    }

    public bool IsLiveObject()
    {
        for (int i = 0; i < FireList.Count; i++)
            if (FireList[i].activeSelf) return true;
        return false;
    }
}

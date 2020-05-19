using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Contents.Event;

public class GameOctopus : MonoBehaviour
{
    public Camera mCamera;

    public List<GameObject> OctopusList = new List<GameObject>();
    public int OctopusCount = 0;

    public void Enter()
    {
        Message.AddListener<OctopusCreateMsg>(Create);
        SetupList();
        StartCoroutine(PlayOctopus());
    }

    public void Destroy()
    {
        OctopusCount = 0;
        Message.RemoveListener<OctopusCreateMsg>(Create);

        for (int i = 0; i < OctopusList.Count; i++)
        {
            OctopusList[i].SetActive(false);
        }

    }

    void SetupList()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            OctopusList.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    IEnumerator PlayOctopus()
    {
        while(true)
        {
            yield return null;
        }
    }

    public void Create(OctopusCreateMsg msg)
    {
        if (OctopusCount < OctopusList.Count)
        {
            OctopusList[OctopusCount].SetActive(true);
            OctopusList[OctopusCount].GetComponent<GameOctopusObj>().Active(msg.Position);
            OctopusCount++;
        }
    }
}

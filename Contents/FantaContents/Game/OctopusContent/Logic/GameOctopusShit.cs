using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents.Event;

public class GameOctopusShit : MonoBehaviour
{
    public Camera mCamera = null;

    public List<GameObject> ShitList = new List<GameObject>();

    int ShitCount = 0;

    public void Enter()
    {
        Message.AddListener<OctopusShitCreateMsg>(CreateShit);
        SetupList();
    }

    public void Destroy()
    {
        Message.RemoveListener<OctopusShitCreateMsg>(CreateShit);
        for (int i = 0; i < ShitList.Count; i++)
            ShitList[i].SetActive(false);
    }

    void SetupList()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            ShitList.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    public void CreateShit(OctopusShitCreateMsg msg)
    {
        if (ShitCount > ShitList.Count)
            return;
        else
        {
            ShitList[ShitCount].SetActive(true);
            msg.Position = mCamera.WorldToScreenPoint(msg.Position);
            msg.Position = mCamera.ScreenToWorldPoint(msg.Position);
            msg.Position.z = 0.0f;
            ShitList[ShitCount].GetComponent<GameOctopusShitObj>().Acitve(msg.Position);

            ShitCount++;
        }
    }
}

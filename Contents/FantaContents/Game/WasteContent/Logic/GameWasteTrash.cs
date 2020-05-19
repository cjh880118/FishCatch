using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWasteTrash : MonoBehaviour
{
    public CellBig.Common.ObjectPool ObjectPool;
    public List<GameObject> TrashObjList = new List<GameObject>();

    public int SpawnSizeMin = 1;
    public int SpawnSizeMax = 5;
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    int prevCount = 0;
    public void Setup()
    {
        for (int i = 0; i < ObjectPool.transform.childCount; i++)
            TrashObjList.Add(ObjectPool.transform.GetChild(i).gameObject);
    }

    public void Destroy()
    {
        for (int i = 0; i < TrashObjList.Count; i++)
            TrashObjList[i].SetActive(false);
        TrashObjList.Clear();
    }

    public void Create()
    {
        int CurrentSize = Random.Range(SpawnSizeMin, SpawnSizeMax);

        for (int i = 0; i < TrashObjList.Count; i++)
        {
            if (!TrashObjList[i].activeSelf)
            {
                for (int j = 0; j < CurrentSize;)
                {
                    Debug.LogError(i+j);
                    TrashObjList[i + j].SetActive(true);
                    TrashObjList[i + j].GetComponent<GameWasteTrashObj>().RadomObject(RadomPos_Area(spawnAreaCenter, spawnAreaSize));
                    if (j == CurrentSize - 1)
                        return;
                    else
                        j++;

                }
                break;
            }
        }
    }

    public Vector3 RadomPos_Area(Vector3 pos, Vector3 size)
    {
        return (RandomPos(pos.x - size.x * 0.5f, pos.x + size.x * 0.5f,
            pos.y - size.y * 0.5f, pos.y + size.y * 0.5f,
            pos.z - size.z * 0.5f, pos.z + size.z * 0.5f));
    }
    public Vector3 RandomPos(float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        return new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));
    }

}

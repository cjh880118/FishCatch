using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi.Contents.Event;

public class GameFallen_ObjectControl : MonoBehaviour
{
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    public float Min_Distance = 0.3f;

    public List<GameFallen_Leaf> leavesList = new List<GameFallen_Leaf>();

    float minx;
    float maxx;
    float miny;
    float maxy;
    float minz;
    float maxz;

    private void Awake()
    {
        minx = spawnAreaCenter.x - spawnAreaSize.x * 0.5f;
        maxx = spawnAreaCenter.x + spawnAreaSize.x * 0.5f;
        miny = spawnAreaCenter.y - spawnAreaSize.y * 0.5f;
        maxy = spawnAreaCenter.y + spawnAreaSize.y * 0.5f;
        minz = spawnAreaCenter.z - spawnAreaSize.z * 0.5f;
        maxz = spawnAreaCenter.z + spawnAreaSize.z * 0.5f;
    }

    public IEnumerator SetSpawn(GameFallen_Leaf leaf)
    {
        Debug.Log("SetSpawn");

        RandomPos(leaf,
                                                minx,
                                                maxx,
                                                miny,
                                                maxy,
                                                minz,
                                                maxz);

        yield return null;
    }

    void RandomPos(GameFallen_Leaf leaf, float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        Vector3 tempPos = Vector3.zero;
        bool isPass = true;

        tempPos = new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));

        for (int index = 0; index < leavesList.Count-1; index++)
        {
            if (Vector3.Distance(leavesList[index].transform.position, tempPos) < Min_Distance)
            {
                Debug.Log("Min");
                isPass = false;
                leaf.DeActive();
                break;
            }
        }

        if (isPass)
            leaf.SetSpawn(tempPos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRockHolder_ObjectControl : MonoBehaviour
{
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    public float Min_Distance = 3.0f;

    public List<GameRockHolder_Rock> rockList = new List<GameRockHolder_Rock>();

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

    public IEnumerator SetSpawn(GameRockHolder_Rock rock)
    {
        RandomPos(rock,
                                                minx,
                                                maxx,
                                                miny,
                                                maxy,
                                                minz,
                                                maxz);

        yield return null;
    }

    void RandomPos(GameRockHolder_Rock rock, float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        Vector3 tempPos = Vector3.zero;
        bool isPass = true;

        tempPos = new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));

        for (int index = 0; index < rockList.Count - 1; index++)
        {
            if (Vector3.Distance(rockList[index].transform.position, tempPos) < Min_Distance)
            {
                isPass = false;
                StartCoroutine(SetSpawn(rock));
                break;
            }
        }

        if (isPass)
            rock.SetSpawn(tempPos);
    }
}

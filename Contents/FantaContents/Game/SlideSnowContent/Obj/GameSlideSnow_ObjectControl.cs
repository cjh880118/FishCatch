using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig.Contents.Event;

public class GameSlideSnow_ObjectControl : MonoBehaviour
{
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    public float Min_Distance = 1.2f;

    public List<GameSlideSnow_Snow> snowsList = new List<GameSlideSnow_Snow>();

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

    public IEnumerator SetSpawn(GameSlideSnow_Snow snow)
    {
        RandomPos(snow,
                                                minx,
                                                maxx,
                                                miny,
                                                maxy,
                                                minz,
                                                maxz);

        yield return null;
    }

    void RandomPos(GameSlideSnow_Snow snow, float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        Vector3 tempPos = Vector3.zero;
        bool isPass = true;

        tempPos = new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));

        for (int index = 0; index < snowsList.Count - 1; index++)
        {
            if (Vector3.Distance(snowsList[index].transform.position, tempPos) < Min_Distance)
            {
                isPass = false;
                snow.DeActive();
                break;
            }
        }

        if (isPass)
            snow.SetSpawn(tempPos);
    }
}

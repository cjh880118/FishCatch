using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePaints_ObjectControl : MonoBehaviour
{
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    public float Min_Distance = 0.5f;

    public List<GamePaints_Paint> paintList = new List<GamePaints_Paint>();

    public IEnumerator SetSpawn(GamePaints_Paint paint)
    {
        yield return StartCoroutine(RandomPos(paint,
            spawnAreaCenter.x - spawnAreaSize.x * 0.5f, spawnAreaCenter.x + spawnAreaSize.x * 0.5f,
            spawnAreaCenter.y - spawnAreaSize.y * 0.5f, spawnAreaCenter.y + spawnAreaSize.y * 0.5f,
            spawnAreaCenter.z - spawnAreaSize.z * 0.5f, spawnAreaCenter.z + spawnAreaSize.z * 0.5f));

        yield return null;
    }

    IEnumerator RandomPos(GamePaints_Paint paint, float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        int count = 0;
        bool[] isPass = new bool[paintList.Count];

        Vector3 tempPos = Vector3.zero;

        while (count < isPass.Length)
        {
            isPass = new bool[paintList.Count];
            tempPos = new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));

            count = 0;

            for (int index = 0; index < paintList.Count; index++)
            {
                if (Vector3.Distance(paintList[index].transform.localPosition, tempPos) >= Min_Distance)
                {
                    isPass[index] = true;
                    count++;
                }
                else
                    isPass[index] = false;
            }

            yield return null;
        }

        paint.SetSpawn(tempPos);

        yield return null;
    }
}


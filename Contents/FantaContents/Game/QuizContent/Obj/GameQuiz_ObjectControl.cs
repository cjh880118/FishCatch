using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQuiz_ObjectControl : MonoBehaviour
{
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = Vector3.one;

    public float Min_Distance = 0.5f;

    public List<GameQuiz_Figure> figureList = new List<GameQuiz_Figure>();

    public IEnumerator SetSpawn(GameQuiz_Figure figure)
    {
        yield return StartCoroutine(RandomPos(figure,
            spawnAreaCenter.x - spawnAreaSize.x * 0.5f, spawnAreaCenter.x + spawnAreaSize.x * 0.5f,
            spawnAreaCenter.y - spawnAreaSize.y * 0.5f, spawnAreaCenter.y + spawnAreaSize.y * 0.5f,
            spawnAreaCenter.z - spawnAreaSize.z * 0.5f, spawnAreaCenter.z + spawnAreaSize.z * 0.5f));

        yield return null;
    }

    IEnumerator RandomPos(GameQuiz_Figure figure, float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        int count = 0;
        bool[] isPass = new bool[figureList.Count];

        Vector3 tempPos = Vector3.zero;

        while (count < isPass.Length)
        {
            isPass = new bool[figureList.Count];
            tempPos = new Vector3(Random.Range(minx, maxx), Random.Range(miny, maxy), Random.Range(minz, maxz));

            count = 0;

            for (int index = 0; index < figureList.Count; index++)
            {
                if (Vector3.Distance(figureList[index].transform.localPosition, tempPos) >= Min_Distance)
                {
                    isPass[index] = true;
                    count++;
                }
                else
                    isPass[index] = false;
            }
            yield return null;
        }

        figure.SetSpawn(tempPos);

        yield return null;
    }
}


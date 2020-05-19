using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVertex_ObjectControl : MonoBehaviour
{
    public Transform spawnPointBox;
    public Transform[] spawnPoint;
    Transform[] tempSpawnPoints;

    public bool[] isPossibleSpawn;

    public void Start()
    {
        tempSpawnPoints = spawnPointBox.GetComponentsInChildren<Transform>();

        spawnPoint = new Transform[tempSpawnPoints.Length - 1];
        isPossibleSpawn = new bool[tempSpawnPoints.Length - 1];

        for (int index = 1; index < tempSpawnPoints.Length; index++)
        {
            spawnPoint[index - 1] = tempSpawnPoints[index];
            isPossibleSpawn[index - 1] = true;
        }
    }

    public void SetSpawn(GameVertex_Vertex vertex, int index)
    {
        vertex.SetSpawn(index, spawnPoint[index]);
        isPossibleSpawn[index] = false;
    }

    public void ResetSpawn(int index)
    {
        isPossibleSpawn[index] = true;
    }
}

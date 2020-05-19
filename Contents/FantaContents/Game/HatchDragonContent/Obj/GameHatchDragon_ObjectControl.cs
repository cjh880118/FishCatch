using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHatchDragon_ObjectControl : MonoBehaviour
{
    public Transform[] spawnPoint;

    public bool[] isPossibleSpawn;

    void Start()
    {
        isPossibleSpawn = new bool[spawnPoint.Length];

        for(int index = 0; index < isPossibleSpawn.Length; index++)
            isPossibleSpawn[index] = true;
    }

    public void SetSpawn(GameHatchDragon_HatchDragon dragon, int index)
    {
        dragon.SetSpawn(index, spawnPoint[index]);
        isPossibleSpawn[index] = false;
    }

    public void ResetSpawn(int index)
    {
        isPossibleSpawn[index] = true;
    }
}

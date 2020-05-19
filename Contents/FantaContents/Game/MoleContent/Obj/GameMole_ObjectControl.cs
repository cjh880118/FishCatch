using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMole_ObjectControl : MonoBehaviour
{
    public GameMole_Hole[] gameMole_Hole;
    public GameMole_Hammer gameMole_Hammer;

    public List<int> isPossibleCreateList = new List<int>();

    bool isPossibleCreate;

    private void Start()
    {
        gameMole_Hole = GetComponentsInChildren<GameMole_Hole>();
    }

    private void OnEnable()
    {
        isPossibleCreate = true;
    }

    private void OnDisable()
    {
        isPossibleCreateList.Clear();
    }

    public void FindHole(GameMole_Mole mole)
    {
        GameMole_Hole tempHole = null;

        isPossibleCreateList.Clear();

        for (int index = 0; index < gameMole_Hole.Length; index++)
        {
            if (!gameMole_Hole[index].isActive)
                isPossibleCreateList.Add(index);
        }

        if (isPossibleCreateList.Count <= 0)
        {
            isPossibleCreate = false;
            return;
        }
        else
            isPossibleCreate = true;

        int createRandomIndex = Random.Range(0, isPossibleCreateList.Count);

        tempHole = gameMole_Hole[isPossibleCreateList[createRandomIndex]];
        tempHole.Active();

        mole.SetSpawn(tempHole, tempHole.HolePos());
    }

    public void ActiveHammer(GameMole_Mole mole)
    {
        StartCoroutine(gameMole_Hammer.Hit(mole.transform.position));
    }

    public bool IsPossibleCreate()
    {
        return isPossibleCreate;
    }
}

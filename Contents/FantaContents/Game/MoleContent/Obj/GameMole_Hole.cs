using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMole_Hole : MonoBehaviour
{
    public Transform holePos;

    public bool isActive;

    public void Active()
    {
        isActive = true;
    }

    public void DeActive()
    {
        isActive = false;
    }

    public Vector3 HolePos()
    {
        return holePos.position;
    }
}

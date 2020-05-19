using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFruit_ObjectControl : MonoBehaviour
{
    public float minPower = 12.0f;
    public float maxPower = 13.0f;

    public float minPosX_Left = -200.0f;
    public float maxPosX_Left = 250.0f;

    public float minPosX_Right = 360.0f;
    public float maxPosX_Right = 850.0f;

    public float PosY = -450f;

    float powerX = 3000f;
    float powerY = 10000f;

    public void SetSpawn(GameFruit_Fruit fruit)
    {
        float powerIndex = UnityEngine.Random.Range(minPower, maxPower);
        float direction = UnityEngine.Random.Range(0, Enum.GetNames(typeof(FruitDirection)).Length);
        float zOrder = UnityEngine.Random.Range(0, 6);

        Quaternion qua = Quaternion.Euler(UnityEngine.Random.Range(0.0f, -360.0f), UnityEngine.Random.Range(0.0f, -360.0f), UnityEngine.Random.Range(0.0f, -360.0f));

        Vector3 tempPos = Vector3.zero;

        if (direction == (int)FruitDirection.Left)
            tempPos.x = UnityEngine.Random.Range(minPosX_Left, maxPosX_Left);
        else if (direction == (int)FruitDirection.Right)
        {
            tempPos.x = UnityEngine.Random.Range(minPosX_Right, maxPosX_Right);
            powerX *= -1.0f;
        }

        tempPos.y = PosY;
        tempPos.z = zOrder * 50.0f;

        fruit.SetSpawn(tempPos, qua, powerX * powerIndex, powerY * powerIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Models;
using JHchoi.Module;

public class SensorTest : MonoBehaviour
{
    delegate void Func(int vel);
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitSensor());
        //Message.Send<IO_PowerMsg>(new IO_PowerMsg(true));
        Message.AddListener<IOStateMsg>(InputData);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnLed(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnLed(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            OnLed(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            OnLed(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            OnLed(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            OnLed(6);
    }

    void InputData(IOStateMsg vel)
    {
        string[] data = vel.Desc.Split(',');

        Message.Send<IO_LedMsg>(new IO_LedMsg(int.Parse(data[0]),  false));
        //StartCoroutine(TimmerFunc(2f, OnDamage, int.Parse(data[0])));
    }

    IEnumerator InitSensor()
    {
        //var gameModel = new GameModel();
        //gameModel.Setup();

        //yield return StartCoroutine(ModuleManager.Instance.LoadAll());
        yield return new WaitForSeconds(2f);

        Message.Send<IO_PowerMsg>(new IO_PowerMsg(true));

        yield return new WaitForSeconds(0.2f);
        for (int i = 1; i < 7; i++)
        {
            yield return new WaitForSeconds(0.2f);
            OnDamage(i);
            yield return new WaitForSeconds(0.2f);
            OnLed(i);
        }
    }

    void OnDamage( int index )
    {
        Message.Send<IO_DamageMsg>(new IO_DamageMsg(index, true));
    }

    void OnLed(int index)
    {
        Debug.Log("LED ON :" + index);
        Message.Send<IO_LedMsg>(new IO_LedMsg(index, true));
    }

    IEnumerator TimmerFunc(float timer, Func _func, int vel)
    {
        yield return new WaitForSeconds(timer);
        _func(vel);
    }
}

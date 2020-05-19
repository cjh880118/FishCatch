using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;
using JHchoi.Common;
using JHchoi.Module;
using JHchoi.Models;

public class IoBoardModule : IModule
{
    /// <summary>
    /// 타격시 센서 입력데이터를 받아오는 시간이 0.15f 정도 걸린다.
    /// pc에서 보드로 패킷을 보내고 처리하여 답변이 오는 시간 0.25f 정도 걸림
    /// pc에서 보드로 패킷을 보내는 최소 시간주기는 0.05f
    /// </summary>
    /// 

    public string PortName = "";
    public int SerialBaudRate = 0;

    private SerialPort IO_Serial = null;

    private byte[] SendMsg = null;
    private byte[] RecMsg = null;

    private int MaxByteCount = 60;

    public static List<byte> message = new List<byte>();

    Thread t1 = null;
    delegate void Func(int vel);

    public static bool ThreadRun = true;
    public static Coroutine reconnect;
    public static bool initPort = true;

    int  HitIndex = 0;
    float HitTime = 0;

    protected override void OnLoadStart()
    {
        SendMsg = new byte[MaxByteCount];
        RecMsg = new byte[MaxByteCount];

        ResetBuff();

        var sm = Model.First<SettingModel>();
        ArduinoConnect(sm.PortName, sm.PortRate);
        StartCoroutine(Reconnect());

        Message.AddListener<IO_PowerMsg>(OnPowerSend);
        Message.AddListener<IO_DamageMsg>(OnDamageSend);
        Message.AddListener<IO_LedMsg>(OnLedSend);
    }

    protected override void OnUnload()
    {
        if (IO_Serial.IsOpen)
            IO_Serial.Close();

        Message.RemoveListener<IO_PowerMsg>(OnPowerSend);
        Message.RemoveListener<IO_DamageMsg>(OnDamageSend);
        Message.RemoveListener<IO_LedMsg>(OnLedSend);        
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        ThreadRun = false;
        if (t1 != null)
        {
            t1.Interrupt();
            t1.Abort();
            OnPowerSend(new IO_PowerMsg(false));
        }
    }

    public bool ArduinoConnect(string portName = "", int baudRate = 0)
    {
        if (!string.IsNullOrEmpty(portName))
            PortName = portName;

        if (baudRate != 0)
            SerialBaudRate = baudRate;

        try
        {
            IO_Serial = new SerialPort(PortName, SerialBaudRate, Parity.None, 8, StopBits.One);            
            IO_Serial.ReadTimeout = 1;
            IO_Serial.WriteTimeout = 100;
            IO_Serial.Open();
        }
        catch (UnauthorizedAccessException exp)
        {

               initPort = false;
            return false;
        }
        catch (ArgumentOutOfRangeException exp)
        {
            initPort = false;
            return false;
        }
        catch (ArgumentException exp)
        {
            initPort = false;
            return false;
        }
        catch (IOException exp)
        {
            initPort = false;
            return false;
        }
        catch (InvalidOperationException exp)
        {
            initPort = false;
            return false;
        }
        if (t1 == null)
        {
            t1 = new Thread(new ThreadStart(ThreadFunc));
            t1.Start();
        }
        Debug.Log("ArduinoConnect OK");

        
        SetResourceLoadComplete();
        return true;
    }

    void SetSensorSensitivity()
    {
        SendMsg[2] = 0x12;
        SendMsg[4] = 25;
        OnArduinoSend();
    }

    void OnPowerSend(IO_PowerMsg msg)
    {
        Debug.Log("OnPowerSend");
        if (msg.ON)
            SendMsg[2] = 0x01;
        else
            SendMsg[2] = 0x02;

        OnArduinoSend();

        if (msg.ON)
            SetSensorSensitivity();
    }

    void OnDamageSend(IO_DamageMsg msg)
    {
        SendMsg[2] = 0x04;
        SendMsg[3] = (byte)msg.Index;
        SendMsg[4] = (byte)(msg.ON == true ? 1 : 0);
        OnArduinoSend();
    }

    void OnLedSend(IO_LedMsg msg)
    {
        SendMsg[2] = 0x0C;
        SendMsg[3] = (byte)msg.Index;
        if (msg.ON)
            SendMsg[4] = 1;
        else
            SendMsg[4] = 0;

        OnArduinoSend();
    }

    void OnArduinoSend()
    {
        if (!IO_Serial.IsOpen)
            return;
        string msgString = "";
        foreach (var item in SendMsg)
        {
            msgString += item.ToString() + ", ";
        }
        //Log.Instance.log("SendMsg = " + msgString);

        IO_Serial.Write(SendMsg, 0, MaxByteCount);

        ResetBuff();
    }

    void ResetBuff()
    {
        Array.Clear(SendMsg, 0x0, SendMsg.Length);
        SendMsg[0] = 0x01;   // 패킷 시작
        SendMsg[1] = 0xAA;   // 패킷 시작
        SendMsg[11] = 0x011; // 패킷 끝
    }

    IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(2.0f);

        if (initPort == false)
        {
            initPort = true;
            Debug.LogWarning("재연결 시도");
            ArduinoConnect();
        }
        StartCoroutine(Reconnect());
    }
       
    void ThreadFunc()
    {
        Debug.Log("ThreadFunc");
        while (ThreadRun)
        {
            if (IO_Serial == null)
                continue;

            if (!IO_Serial.IsOpen)
                continue;

            try
            {
                int count = IO_Serial.Read(RecMsg, 0, MaxByteCount);

                Debug.LogWarning("RecMsgLng : " + RecMsg.Length + " // " + count);
                for (int i = 0; i < count; i++)
                {
                    message.Add(RecMsg[i]);
                    //Debug.Log(RecMsg[i]);
                }
            }
            catch (IOException exp)
            {
                //StartCoroutine(Reconnect());
                initPort = false;
                continue;
            }
            catch (System.Exception e)
            {
                //Debug.LogWarning("System.Exception in serial.ReadLine: " + e.ToString());
                continue;
            }
        }
    }

    private void Update()
    {
        // Debug.Log(message.Count);
        //if (message.Count >= 12)
        //{
        //    for (int i = 0; i < message.Count - 11; i++)
        //    {
        //        if (message[i] == 0x01 && message[i + 11] == 0x011)
        //        {
        //            //Log.Instance.log("RecieveMsg = " + message.GetRange(i, 12));

        //            string msgString = "";
        //            foreach (var item in message.GetRange(i, 12))
        //            {
        //                msgString += item.ToString() + ", ";
        //            }
        //            //Log.Instance.log("RecoeveMsg = " + msgString);

        //            Parser(message.GetRange(i, 12));
        //            message.RemoveRange(i, 12);
        //        }
        //    }
        //}

        if (message.Count >= 12)
        {
            for (int i = 0; i < message.Count - 11; )
            {
                if (message[i] == 0x01 && message[i + 11] == 0x011)
                {
                    Parser(message.GetRange(i, 12));
                    message.RemoveRange(i, 12);
                    continue;
                }
                i++;
            }
        }
    }

    void Parser(List<byte> _message)
    {

        if (_message[1] == 0x055)
        {
            switch (_message[2])
            {
                case 0x03:      // 키버튼 
                    Debug.Log("키버튼 클릭 : " + _message[3]);   //0 ~ 24 값이 들어온다.
                    break;
                case 0x04:      // 충격센서
                    Debug.Log("층격센서 감지");   //0 ~ 6 값이 들어온다.
                                            //StartCoroutine(TimmerFunc(0.05f, OffDmg, message[3]));
                                            //StartCoroutine(TimmerFunc(0.1f, OffLed , message[3]));

                    string desc = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", _message[3], _message[4], _message[5], _message[6], _message[7], _message[8], _message[9]);
                    //Debug.LogError(desc);

                    int index = _message[3];
                    Log.Instance.log("Sensor Input = " + _message[3]);
                    if (Time.time - HitTime > 0.1f)
                    {
                        Message.Send<IOStateMsg>(new IOStateMsg(desc ));
                    }
                    else
                    {
                        if (HitIndex != index)
                            Message.Send<IOStateMsg>(new IOStateMsg(desc));
                    }

                    HitIndex = _message[3];
                    HitTime = Time.time;
                    break;
                case 0x05:      // RFID
                    Debug.Log("RFID");   // 값이 없다.
                    break;
                case 0x06:      // 키오스크
                    Debug.Log("KIOSK");   // 값이 없다.
                    break;
                case 0x07:      // 지페기
                    Debug.Log("지페 : " + _message[3]);   //0 ~ FF 값이 들어온다.
                    break;
                case 0x08:      // 코인
                    Debug.Log("코인 : " + _message[3]);   //0 ~ FF 값이 들어온다.
                    break;
                case 0x09:      // 가속도센서 10비트 데이터 읽기
                    break;
                case 0x0A:      // 가변저항 (자전거 ADC 읽기)
                    break;
                case 0x0B:      // OUT 포트 제어
                    break;
                case 0x0C:      // LED 포트 제어
                    break;
                case 0x0D:      // Test 모드
                    break;
                case 0x0E:      // 가속도 센서  ADC값 연속 전송
                    break;
                case 0x0F:      // 가변 저항 (자전거) ADC ADC값 연속 전송
                    break;
                case 0x10:      // 시스템 리셋 
                    break;
            }
        }
        if (_message[1] == 0x0BB)
        {
            switch (_message[2])
            {
                case 0x03:      // 키버튼 
                    //Debug.Log("ACK_키버튼 클릭");   //0 ~ 24 값이 들어온다.
                    break;
                case 0x04:      // 충격센서
                    //Debug.Log("ACK_층격센서 감지");   //0 ~ 6 값이 들어온다.
                     break;
                case 0x05:      // RFID
                    Debug.Log("ACK_RFID");   // 값이 없다.
                    break;
                case 0x06:      // 키오스크
                    Debug.Log("ACK_KIOSK");   // 값이 없다.
                    break;
                case 0x07:      // 지페기
                    Debug.Log("ACK_지페");   //0 ~ FF 값이 들어온다.
                    break;
                case 0x08:      // 코인
                    Debug.Log("ACK_코인");   //0 ~ FF 값이 들어온다.
                    break;
                case 0x09:      // 가속도센서 10비트 데이터 읽기
                    break;
                case 0x0A:      // 가변저항 (자전거 ADC 읽기)
                    break;
                case 0x0B:      // OUT 포트 제어
                    break;
                case 0x0C:      // LED 포트 제어
                    //Debug.Log("ACK_LED");
                    break;
                case 0x0D:      // Test 모드
                    break;
                case 0x0E:      // 가속도 센서  ADC값 연속 전송
                    break;
                case 0x0F:      // 가변 저항 (자전거) ADC ADC값 연속 전송
                    break;
                case 0x10:      // 시스템 리셋 
                    break;
            }
            //Debug.Log("ACK : "+message[2]);
        }
    }


    #region test Code
    void OffLed(int index)
    {
        Debug.Log("LED Off : " + index);
        Message.Send<IO_LedMsg>(new IO_LedMsg(index, false));
    }
    void OffDmg(int index)
    {
        Debug.Log("LED Off : " + index);
        Message.Send<IO_DamageMsg>(new IO_DamageMsg(index, false));
    }

    IEnumerator TimmerFunc(float timer, Func _func, int vel)
    {
        yield return new WaitForSeconds(timer);
        _func(vel);
    }
    #endregion

}


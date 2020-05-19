using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.T3;
using System;

public class T3SensorData
{
    public float time = 0.0f;
    public float posX = 0.0f;

    public void Reset()
    {
        time = 0.0f;
        posX = 0.0f;
    }
}

namespace CellBig.Module
{

    public class NewT3SensorModule : IModule
    {
        StationSocket mSocket = new StationSocket();
        ulong ModuleID = vCatchStationTypeDef.ModuleID_3101;
        ulong type = vCatchStationTypeDef.v4001_TYPE_4x3; // for ModuleID_4001
        uint timer = 30000; //ms
        List<T3SensorData> resultList = new List<T3SensorData>();
        Coroutine _coroutine;

        bool mStartGame1 = false;
        bool mStopGame1 = false;
        bool mTest1 = false;
        string msgs = "";

        bool mStarted = false;
        bool pause = false;

        protected override void OnLoadStart()
        {
            StartCoroutine(Setup());
        }

        public IEnumerator Setup()
        {
            yield return null;
            AddMessage();
            mStarted = false;
            pause = false;

            mStartGame1 = false;
            mStopGame1 = false;
            mTest1 = false;

            mSocket.OnConnected += OnConnected;
            mSocket.OnStoppedGame += OnStoppedGame;
            mSocket.OnMotion += OnMotion;
            mSocket.OnDegree += OnDegree;
            mSocket.OnPoint += OnPoint;
            mSocket.OnSpeedPoint += OnSpeedPoint;
            mSocket.Start(ModuleID);

            SetResourceLoadComplete();
        }

        private void AddMessage()
        {
            Message.AddListener<T3SensorCatchMsg>(SendT3Catch);
        }

        void SendT3Catch(T3SensorCatchMsg msg)
        {
            resultList.Clear();
            if (msg.T3Catch)
                OnStartCatchSensor();
            else
                OnStopCatchSensor();
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                if (pause)
                {
                    if (mStarted)
                    {
                        mSocket.StopGame();
                        mStarted = false;
                    }

                    mSocket.Close();

                    mStartGame1 = true;
                    mStopGame1 = false;
                    mTest1 = false;

                    mSocket.Start(ModuleID);
                }
            }

            pause = !focusStatus;
        }

        void OnDisable()
        {
            if (mStarted && !pause)
                mSocket.StopGame();

            mSocket.Close();
        }

        void OnStartCatchSensor()
        {
            if (mSocket.StartGame(type))
            {
                mStarted = true;
                mStopGame1 = true;
                mTest1 = true;
                msgs = "";
                OnTimer();
            }
        }

        void OnStopCatchSensor()
        {
            mSocket.StopGame();

            mStarted = false;

            mStartGame1 = true;
            mStopGame1 = false;
            mTest1 = false;
        }

        void OnTimer()
        {
            // timeout 2 seconds
            mSocket.SetTimeout(timer); // 0x0fff : infinite, max : 0x0fff (4095‬)
            Message.Send<T3SensorStartMsg>(new T3SensorStartMsg());
        }

        void OnConnected()
        {
            mSocket.OnDisconnected += OnDisconnected;

            mStartGame1 = true;
            mStopGame1 = false;
            mTest1 = false;
        }

        void OnDisconnected()
        {
            mSocket.OnConnected += OnConnected;
            mSocket.OnDisconnected -= OnDisconnected;
            mStartGame1 = false;
            mStopGame1 = false;
            mTest1 = false;

            msgs = "";
        }

        void OnStoppedGame()
        {
            mStarted = false;

            mStartGame1 = true;
            mStopGame1 = false;
            mTest1 = false;

            msgs += "Stopped Game";
        }

        void OnDegree(vCatchStationTypeDef.vCatchResult_Degree stDegree)
        {
            msgs = string.Format("# ms:{0}  pos:{1}  width:{2}\n", stDegree.msec,
                    stDegree.pos, stDegree.width);

            T3SensorData sd = new T3SensorData();
            sd.posX = stDegree.pos + 0.5f;
            sd.time = Time.time;

            resultList.Add(sd);
            Log.Instance.log("Result List Count : " + resultList.Count.ToString());

            if (resultList.Count == 1)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
                _coroutine = StartCoroutine(CheckResult(sd));
                Log.Instance.log("Result Count 1 ");
            }

            if (resultList.Count == 2)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                Log.Instance.log("Send T3ResultMsg");
                Debug.Log("Send T3ResultMsg");

                var msg = new T3ResultMsg();
                for (int i = 0; i < resultList.Count; i++)
                    msg.Datas.Add(resultList[i]);

                Message.Send<T3ResultMsg>(msg);
                resultList.Clear();
            }
        }

        IEnumerator CheckResult(T3SensorData data)
        {
            while (true)
            {
                yield return null;
                if (data.time + 2.0f < Time.time)
                {
                    Log.Instance.log(string.Format("T3_Result TimeOver : {0}, {1}", data.time, data.posX));
                    SendT3Catch(new T3SensorCatchMsg(false));
                    Message.Send<T3SensorReRequestMsg>(new T3SensorReRequestMsg());
                    break;
                }
            }
        }


        void OnPoint(vCatchStationTypeDef.vCatchResult_Point stPoint)
        {
            msgs = string.Format("# ms:{0}  idPt:{1}  status:{2}  pos:{3},{4}\n", stPoint.msec,
                    stPoint.idPoint, stPoint.status, stPoint.posX, stPoint.posY);
        }
        void OnSpeedPoint(vCatchStationTypeDef.vCatchResult_SpeedPoint stSpeedPoint)
        {
            msgs = string.Format("# ms:{0}  pos:{1},{2}  speed:{3}\r\n", stSpeedPoint.msec,
                    stSpeedPoint.posX, stSpeedPoint.posY, stSpeedPoint.speed);
        }

        void OnMotion(int[] motion)
        {
            msgs = "";
            switch (type)
            {
                case 0:
                    Debug.Assert(motion.Length == 300);
                    for (int y = 0; y < 15; y++)
                    {
                        for (int x = 0; x < 20; x++, msgs += " ")
                        {
                            msgs += string.Format("{0:D3}", motion[y * 20 + x]);
                        }
                        msgs += "\n";
                    }
                    break;
                case 1:
                    Debug.Assert(motion.Length == 12);
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 4; x++, msgs += " ")
                        {
                            msgs += string.Format("{0:D3}", motion[y * 4 + x]);
                        }
                        msgs += "\n";
                    }
                    break;
                case 2:
                    msgs += motion[0];
                    break;
            }

        }

        private void OnDestroy()
        {
            mSocket.vCatchStationProcessEnd();
        }

        protected override void OnUnload()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.AddListener<T3SensorCatchMsg>(SendT3Catch);
        }
    }
}

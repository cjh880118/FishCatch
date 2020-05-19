using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

using System.Text;

public class StationSocket
{
    ulong mModuleID = 0;

    TcpClient mClient = null;
    NetworkStream mStream = null;

    byte[] mReceivedData = new byte[0];
    static int MAX_BUFFER = 0xFFFF;

    //string moduleVesion = "";

    public bool isConnecting = false;

    public delegate void delegateConnected();
    public delegate void delegateDisconnected();
    public delegate void delegateStoppedGame();
    public delegate void delegateDegree(vCatchStationTypeDef.vCatchResult_Degree stDegree);
    public delegate void delegatePoint(vCatchStationTypeDef.vCatchResult_Point stPoint);
    public delegate void delegateSpeedPoint(vCatchStationTypeDef.vCatchResult_SpeedPoint stSpeedPoint);
    public delegate void delegateMotion(int[] motion);
    public delegateConnected OnConnected;
    public delegateDisconnected OnDisconnected;
    public delegateStoppedGame OnStoppedGame;
    public delegateDegree OnDegree;
    public delegatePoint OnPoint;
    public delegateSpeedPoint OnSpeedPoint;
    public delegateMotion OnMotion;

    public StationSocket()
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(@"vCatchStation.exe");
        info.UseShellExecute = false;
        info.Verb = "runas";

        try
        {
            System.Diagnostics.Process.Start(info);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            Debug.Log("vCatchStation start error : " + e.ToString());
        }        
    }

    public bool IsConnected()
    {
        return mClient != null && mClient.Connected;
    }

    public bool Start(ulong nModuleID)
    {
        if (IsConnected()) return true;

        if (nModuleID == vCatchStationTypeDef.ModuleID_3101 ||
            nModuleID == vCatchStationTypeDef.ModuleID_3201 ||
            nModuleID == vCatchStationTypeDef.ModuleID_3301 ||
            nModuleID == vCatchStationTypeDef.ModuleID_4001)
            mModuleID = nModuleID;
        else
        {
            Debug.Log("invalid Module ID : " + nModuleID);
            return false;
        }

        isConnecting = true;
        mClient = new TcpClient();
        try
        {
            mClient.BeginConnect("127.0.0.1", vCatchStationTypeDef.vCatchStation_TCP_PortNumber, EndConnect, null);
        }
        catch (Exception e)
        {
            Debug.Log("socket_error : " + e.ToString());
            Close();
            return false;
        }

        return false;
    }

    public void Close()
    {
        if (mStream != null)
        {
            mStream.Close();
            mStream = null;
        }
        if (mClient != null)
        {
            mClient.Close();
            mClient = null;
        }

        //moduleVesion = "";
        isConnecting = false;
        mModuleID = 0;
    }

    public bool StartGame(ulong type)
    {
        if (mClient == null) return false;
        //if (type != vCatchStationTypeDef.v4001_TYPE_1x1 && type != vCatchStationTypeDef.v4001_TYPE_4x3 && type != vCatchStationTypeDef.v4001_TYPE_20x15) return false;

        return vCatchSendPacketToStation(vCatchStationTypeDef.WM_vCatchStation_Do_vCatch, type);
    }

    public void StopGame()
    {
        if (mClient == null) return;

        vCatchSendPacketToStation(vCatchStationTypeDef.WM_vCatchStation_Command_vCatch, vCatchStationTypeDef.vCatchStation_Command_Stop);
    }

    public void SetTimeout(uint ms)
    {
        if (mClient == null) return;

        vCatchSendPacketToStation(vCatchStationTypeDef.WM_vCatchStation_Command_vCatch, vCatchStationTypeDef.vCatchStation_CMD_SetTImeout(ms));
    }

    void EndConnect(IAsyncResult result)
    {
        try
        {
            mClient.EndConnect(result);
            mStream = mClient.GetStream();

            BeginStreamRead(mStream);
        }
        catch (Exception e)
        {
            Debug.Log("socket_error : " + e.ToString());
            Close();
            return;
        }

        vCatchSendPacketToStation(vCatchStationTypeDef.WM_vCatchStation_Initialize_vCatch, mModuleID);

        isConnecting = false;

        OnConnected();
    }

    //
    void BeginStreamRead(NetworkStream stream)
    {
        byte[] bytes = new byte[MAX_BUFFER];
        stream.BeginRead(bytes, 0, MAX_BUFFER, new AsyncCallback(OnReceive), bytes);
    }

    void OnReceive(IAsyncResult result)
    {
        if (mClient == null || mStream == null)
            return;

        if (mClient != null && !mClient.Connected)
        {
            Close();
            OnDisconnected();
            return;
        }

        int nRead = mStream.EndRead(result);
        if (nRead < 0)
        {
            BeginStreamRead(mStream);
            return;
        }

        byte[] receive = result.AsyncState as byte[];
        byte[] packet = new byte[nRead + mReceivedData.Length];
        Buffer.BlockCopy(mReceivedData, 0, packet, 0, mReceivedData.Length);
        Buffer.BlockCopy(receive, 0, packet, mReceivedData.Length, nRead);

        while (packet.Length > 0)
        {
            int idxStart = 0;
            for (; idxStart < packet.Length; idxStart++)
                if (packet[idxStart] == '[')
                    break;

            int length = packet.Length - idxStart;
            if (length < 4)
            {
                mReceivedData = packet;
                break;
            }

            ushort size = 0;
            size = (ushort)(packet[idxStart + 1] | ((ushort)packet[idxStart + 2] << 8));
            if (length < size + 4)
            {
                mReceivedData = packet;
                break;
            }

            if (packet[idxStart + size + 3] != ']')
            {
                idxStart++;
                mReceivedData = new byte[packet.Length - idxStart];
                Buffer.BlockCopy(packet, idxStart, mReceivedData, 0, mReceivedData.Length);
                break;
            }

            idxStart += 3;
            uint type = BitConverter.ToUInt32(packet, idxStart);
            int nStructLen = Marshal.SizeOf(typeof(vCatchStationTypeDef.vCatchResult));
            if (nStructLen < packet.Length) nStructLen = packet.Length;
            byte[] resultvCatch = new byte[nStructLen];
            Buffer.BlockCopy(packet, idxStart + 4, resultvCatch, 0, packet.Length - idxStart - 4);

            ProcessReceive(type, ByteToStruct<vCatchStationTypeDef.vCatchResult>(resultvCatch), resultvCatch.Length);

            int nRemain = packet.Length - size - 4;
            if (nRemain > 0)
            {
                byte[] remain = new byte[nRemain];
                Buffer.BlockCopy(packet, size + 4, remain, 0, nRemain);
                packet = remain;
            }
            else
            {
                packet = new byte[0];
            }
        }

        BeginStreamRead(mStream);
    }

    void ProcessReceive(uint type, vCatchStationTypeDef.vCatchResult resultvCatch, int size)
    {
        switch (type)
        {
            case 0: // vCatchStationTypeDef.WM_COPYDATA_DataType_SensorStatus:
                    //resultvCatch->senser_status;
                if (resultvCatch.senser_status.code != vCatchStationTypeDef.vCatchStationStatusCode.No_Error)
                {
                    OnStoppedGame();
                }
                Debug.Log("StationSocket : " + String.Format("SensorStatus - code({0})\r\n", resultvCatch.senser_status.code));
                break;
            case 10: // vCatchStationTypeDef.WM_COPYDATA_DataType_Degree:
                OnDegree(resultvCatch.resultDegree);
                break;
            case 20: // vCatchStationTypeDef.WM_COPYDATA_DataType_Point:
                OnPoint(resultvCatch.resultPoint);
                break;
            case 21: // vCatchStationTypeDef.WM_COPYDATA_DataType_SpeedPoint:
                OnSpeedPoint(resultvCatch.resultSpeedPoint);
                break;
            case 40: // vCatchStationTypeDef.WM_COPYDATA_DataType_Motion20x15:
                OnMotion(vCatchStationTypeDef.vCatchStation_GetMotion20x15(resultvCatch.resultMotion20x15));
                break;
            case 41: // vCatchStationTypeDef.WM_COPYDATA_DataType_Motion4x3: 
                OnMotion(vCatchStationTypeDef.vCatchStation_GetMotion4x3(resultvCatch.resultMotion4x3));
                break;
            case 42: // vCatchStationTypeDef.WM_COPYDATA_DataType_Motion1x1:
                OnMotion(vCatchStationTypeDef.vCatchStation_GetMotion1x1(resultvCatch.resultMotion1x1));
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    bool vCatchSendPacketToStation(ulong msg, ulong dw)
    {
        byte[] buf = new byte[10];
        // magic code
        buf[0] = (byte)'[';
        // size of the packet
        const ushort size = 6;
        buf[1] = (byte)size;
        buf[2] = (byte)(size >> 8);
        // msg
        buf[3] = (byte)msg;
        buf[4] = (byte)(msg >> 8);
        // dw
        buf[5] = (byte)dw;
        buf[6] = (byte)(dw >> 8);
        buf[7] = (byte)(dw >> 16);
        buf[8] = (byte)(dw >> 24);
        // magic code
        buf[9] = (byte)']';

        try
        {
            mStream.Write(buf, 0, buf.Length);
        }
        catch (Exception e)
        {
            Debug.Log("write error : " + e.ToString());
            Debug.Assert(false);
            return false;
        }
        return true;
    }

    T ByteToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            Debug.Log("ByteStruct : " + size + " " + buffer.Length);
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

    static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2} ", b);
        return hex.ToString();
    }

    public void vCatchStationProcessEnd()
    {
        try
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("vCatchStation");
            if (p.GetLength(0) > 0)
                p[0].Kill();
        }
        catch(Exception E)
        {
            Debug.Log(E.ToString());
        }
        
    }
}


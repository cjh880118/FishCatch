using UnityEngine;
using System.Collections.Generic;


namespace JHchoi.Models
{
    public class FantaBoxSettingModel : Model
    {
        GameModel _owner;
        public FantaSenserInfo SenserInfo;

        static string KinectSettingFileName = "KinectSetting.xml";

        public void Setup(GameModel owner)
        {
            _owner = owner;
            SenserInfo = new FantaSenserInfo();
            LoadFile();
        }

        void LoadFile()
        {
            string data = GameStateXML.LoadXML(KinectSettingFileName);

            if (data == "-1")
            {
                SaveDefaultSetting();
                data = GameStateXML.LoadXML(KinectSettingFileName);
            }

            if (data.ToString() != "")
            {
                SenserInfo = (FantaSenserInfo)GameStateXML.DeserializeObject(data, "FantaSenserInfo");
            }
        }

        public void SaveDefaultSetting()
        {
            FantaSenserInfo.MPoint[] point_1 = new FantaSenserInfo.MPoint[4];
            FantaSenserInfo.MPoint[] point_2 = new FantaSenserInfo.MPoint[4];
            List<Vector2> points = new List<Vector2>() { Vector2.zero, new Vector2(640,0), new Vector2(640, 480), new Vector2(0, 480) };
            for (int i = 0; i < 4; i++)
            {
                point_1[i] = new FantaSenserInfo.MPoint(points[i].x, points[i].y);
                point_2[i] = new FantaSenserInfo.MPoint(points[i].x, points[i].y);
            }

            string ip = "127.0.0.1";

            Save_WebCamSetting(30.0f, 3.0f, 3, 3, false, false, point_1);
            Save_KinectSetting(500.0f, 600.0f, 3.0f, 3, 5, 6, false, false, true, point_1, point_2, null, 0.0f, 0, 0, ip);
            SenserInfo.WebCamData.m_bKinect = true;

            SaveSetting();
        }

        public void Save_WebCamSetting(float fThreshold, float fBlur, int nEroad, int nDlight, bool bUpDown, bool bLeftRight, FantaSenserInfo.MPoint[] pPoint)
        {
            SenserInfo.WebCamData.m_fThreshold = fThreshold;
            SenserInfo.WebCamData.m_fBlur = fBlur;
            SenserInfo.WebCamData.m_nEroad = nEroad;
            SenserInfo.WebCamData.m_nDlight = nDlight;
            SenserInfo.WebCamData.m_bFilp_UpDown = bUpDown;
            SenserInfo.WebCamData.m_bFilp_LeftRight = bLeftRight;
            
            SenserInfo.WebCamData.m_bKinect = false;
            SenserInfo.WebCamData.m_pPerspectiveCornor = pPoint;

            SaveSetting();
        }

        public void Save_KinectSetting(float fDistanceMin, float fDistanceMax, float fBlur, int nEroad, int nDlight, int nFilterDlight, bool bWallScan, bool bUpDown, bool bLeftRight,
                                         FantaSenserInfo.MPoint[] pPoint, ushort[] WallData, float fThreshold, int nCamValue, int nServerState, string sIP)
        {
            SenserInfo.KinectData.m_fDistance_Min = fDistanceMin;
            SenserInfo.KinectData.m_fDistance_Max = fDistanceMax;
            SenserInfo.KinectData.m_fBlur = fBlur;
            SenserInfo.KinectData.m_nEroad = nEroad;
            SenserInfo.KinectData.m_nDlight = nDlight;
            SenserInfo.KinectData.m_nFilterDlight = nFilterDlight;
            SenserInfo.KinectData.m_bWallScan = bWallScan;
            SenserInfo.KinectData.m_bFilp_UpDown = bUpDown;
            SenserInfo.KinectData.m_bFilp_LeftRight = bLeftRight;
            SenserInfo.KinectData.m_fThreshold = fThreshold;

            SenserInfo.KinectData.m_nCamValue = nCamValue;

            SenserInfo.KinectData.m_nServerState = nServerState;
            SenserInfo.KinectData.m_sIP = sIP;
            SenserInfo.KinectData.m_pDepthData_Wall = WallData;

            SenserInfo.WebCamData.m_bKinect = true;
            SenserInfo.WebCamData.m_pPerspectiveCornor = pPoint;

            SaveSetting();
        }

        public void Save_KinectSetting(float fDistanceMin, float fDistanceMax, float fBlur, int nEroad, int nDlight, int nFilterDlight, bool bWallScan, bool bUpDown, bool bLeftRight,
                                        FantaSenserInfo.MPoint[] pPoint, FantaSenserInfo.MPoint[] pPoint2, ushort[] WallData, float fThreshold, int nCamValue, int nServerState, string sIP)
        {
            SenserInfo.KinectData.m_fDistance_Min = fDistanceMin;
            SenserInfo.KinectData.m_fDistance_Max = fDistanceMax;
            SenserInfo.KinectData.m_fBlur = fBlur;
            SenserInfo.KinectData.m_nEroad = nEroad;
            SenserInfo.KinectData.m_nDlight = nDlight;
            SenserInfo.KinectData.m_nFilterDlight = nFilterDlight;
            SenserInfo.KinectData.m_bWallScan = bWallScan;
            SenserInfo.KinectData.m_bFilp_UpDown = bUpDown;
            SenserInfo.KinectData.m_bFilp_LeftRight = bLeftRight;
            SenserInfo.KinectData.m_fThreshold = fThreshold;

            SenserInfo.KinectData.m_nCamValue = nCamValue;

            SenserInfo.KinectData.m_nServerState = nServerState;
            SenserInfo.KinectData.m_sIP = sIP;

            SenserInfo.KinectData.m_pDepthData_Wall = WallData;

            SenserInfo.WebCamData.m_bKinect = true;
            SenserInfo.WebCamData.m_pPerspectiveCornor = pPoint;
            SenserInfo.WebCamData.m_pPerspectiveCornor2 = pPoint2;

            SaveSetting();
        }

        public void SaveSetting()
        {
            string data = GameStateXML.SerializeObject(SenserInfo, "FantaSenserInfo");
            GameStateXML.CreateXML(KinectSettingFileName, data);
        }
    }
}

public class FantaSenserInfo
{
    public FantaSenserInfo() { }

    public WebCam_Data WebCamData;
    public Kinect_Data KinectData;

    // struct
    public struct MPoint
    {
        public MPoint(float xx, float yy)
        {
            x = xx;
            y = yy;
        }
        public float x;
        public float y;
    }

    public struct WebCam_Data
    {
        public MPoint[] m_pPerspectiveCornor;
        public MPoint[] m_pPerspectiveCornor2;
        public float m_fThreshold;
        public float m_fBlur;
        public int m_nEroad;
        public int m_nDlight;
        public bool m_bFilp_UpDown;
        public bool m_bFilp_LeftRight;
        public bool m_bKinect;
    }

    public struct Kinect_Data
    {
        public float    m_fDistance_Min;
        public float    m_fDistance_Max;
        public float    m_fBlur;
        public int      m_nEroad;
        public int      m_nDlight;
        public int      m_nFilterDlight;
        public bool     m_bWallScan;
        public bool     m_bFilp_UpDown;
        public bool     m_bFilp_LeftRight;
        public ushort[] m_pDepthData_Wall;
        public float    m_fThreshold;

        public int m_nCamValue;
        public int m_nServerState;
        public string m_sIP;
        public Vector3 TopLeft;
        public Vector3 BottomRight;
    }
}

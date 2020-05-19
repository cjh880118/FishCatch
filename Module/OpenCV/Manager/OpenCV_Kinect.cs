using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCVForUnity;
using System.Collections.Generic;
using System.Threading;
using System;
using JHchoi.Models;

/// <summary>
/// 키넥트 클래스
/// </summary>
public class OpenCV_Kinect : IOpenCV
{
    // 매 프레임 계산을 시간마다로 개선
    internal float m_fRefreshTimer = 0.5f;
    DateTime oldRefreshTime;

    private KinectSensor _Sensor = null;

    Texture2D m_pKi_Texture = null;             //! 컬러데이터를 받을 텍스쳐
                                                // Texture2D m_pKi_Texture_Depth = null;

    [SerializeField]
    private ColorSourceManager m_pKi_ColorSourceMng = null;     //! 칼라값을 가져올 수있는 키넥트 기본 클래스
    [SerializeField]
    private DepthSourceManager m_pKi_DepthSourceMng = null;     //! 뎁스값을 가져올 수 있는 키넥트 기본 클래스

    ushort[] m_pDepthData = null;      //! 뎁스값을 저장 할 배열
    ushort[] m_pDepthData_Wall = null;      //! 벽뎁스 값을 저장할 배열

    int m_nDepthMap_Width = 0;              //! 뎁스맵 크기
    int m_nDepthMap_Height = 0;              //
    int m_nColorMap_Width = 0;              //! 컬러맵 크기
    int m_nColorMap_Height = 0;

    int m_nDepthMap_Down_Width = 0;       //! 뎁스맵 다운샘플링하고나서 크기
    int m_nDepthMap_Down_Height = 0;       //
    internal const int DownsampleSize = 2;  //! 다운샘플링 수치
    float invDownsampleSize = 0;

    DepthSpacePoint[] m_pDepthSpacePointList = null;        //! 컬러화면과 뎁스화면 캘리브레이션 할 때 필요한 배열들
    ColorSpacePoint[] m_pColorSpacePointList = null;        //
    Color[] m_pDepthToColorTextureBuffer = null;        //

    Mat m_pMat_Color = null;         //! 컬러값 저장용 Mat       
    Mat m_pMat_ColorResize = null;         //! 컬러값 받아서 640x480으로 만들어 주기위한 Mat

    //Mat m_pMat_ColorResize2 = null;         //! 컬러값 받아서 640x480으로 만들어 주기위한 Mat

    Mat m_pMat_Depth = null;         //! 뎁스값 저장용 Mat

    Mat m_pMat_DepthResize = null;         //! 뎁스값 받아서 640x480으로 만들어 주기 위한 Mat

    //Mat m_pMat_DepthResize2 = null;

    Mat m_pMat_Threshold = null;         //! 뎁스값 이후 필터 처리용 Mat
    Mat m_pMat_Threshold_Thread = null;         //! 뎁스값 이후 필터 처리용 Mat Thread;
    Mat m_pMat_Contours = null;         //! 감지 박스 가져오기 용 Mat
    Mat m_pMat_Hierarchy = null;         //\

    //Mat m_pMat_Threshold2 = null;         //! 뎁스값 이후 필터 처리용 Mat
    //Mat m_pMat_Contours2 = null;

    Mat m_pMat_Curr = null;
    Mat m_pMat_Ago = null;
    Mat m_pMat_Curr2 = null;
    Mat m_pMat_Ago2 = null;

    List<MatOfPoint> m_lContours2 = new List<MatOfPoint>();
    List<MatOfPoint> m_lThreadContours = new List<MatOfPoint>();

    Thread m_pThread = null;             //! 쓰레드      
    object lockObject = new object();     //
    object wallLockObject = new object();

    [SerializeField]
    internal float m_fDepth_Distance_Min = 500.0f;      //! 뎁스 최소거리
    [SerializeField]
    internal float m_fDepth_Distance_Max = 600.0f;      //! 뎁스 최대거리
    [SerializeField]
    internal bool m_bFilp_UpDown = false;       //! 위아래 반전
    [SerializeField]
    internal bool m_bFilp_LeftRight = false;       //! 좌우 반전
    [SerializeField]
    internal float m_fBlur = 3.0f;        //! 블러
    [SerializeField]
    internal int m_nEroad = 3;           //! 축소
    [SerializeField]
    internal int m_nDlight = 3;           //! 확장
    [SerializeField]
    internal int m_nFrontNoiseDlight = 13;          //! 뎁스 거리 외 인지된 부분 제거 할 크기 확장
    [SerializeField]
    internal bool m_bWallScan = false;       //! 벽스캔 On/Off
    [SerializeField]
    internal float m_fThreshold = 30.0f;


    //   List<Point> m_pLineDrawPt2 = null;     ///! Perspective 영역 좌표리스트2

    internal bool m_bColor = false;         //! 컬러값 가져올지 안가져올지 판단

    internal int m_nCamValue = 0;
    internal int m_nServerState = 0;
    internal string m_sIP = "127.0.0.1";

    // NetworkView nV = null;

    private void Awake()
    {
        m_pKi_ColorSourceMng.gameObject.SetActive(false);       //! 키넥트 기본 클래스 일단 꺼줌(켜져 있으면 쓸데없는 연산을 함)
        m_pKi_DepthSourceMng.gameObject.SetActive(false);
    }

    public override void Enter(Vector2 txtSize, bool color = true)
    {
        base.Enter(txtSize, color);

        m_bColor = color;
        m_pKi_ColorSourceMng.gameObject.SetActive(true);
        m_pKi_DepthSourceMng.gameObject.SetActive(true);

        DataSet(true);
        StartCoroutine(Init_Kinect());
    }

    public void DataSet(bool isEnter = false)
    {
        var fbsm = Model.First<FantaBoxSettingModel>();
        var data = fbsm.SenserInfo.KinectData;

        m_bWallScan = data.m_bWallScan;                   //! 저장된 데이터 가져오기
        m_fDepth_Distance_Min = data.m_fDistance_Min;     //
        m_fDepth_Distance_Max = data.m_fDistance_Max;     //
        m_bFilp_UpDown = data.m_bFilp_UpDown;             //
        m_bFilp_LeftRight = data.m_bFilp_LeftRight;       //
        m_fBlur = data.m_fBlur;                           //
        m_nEroad = data.m_nEroad;                         //
        m_nDlight = data.m_nDlight;                       //

        m_nFrontNoiseDlight = data.m_nFilterDlight;       //
        m_fThreshold = data.m_fThreshold;

        m_nCamValue = data.m_nCamValue;
        m_nServerState = data.m_nServerState;
        m_sIP = data.m_sIP;
        if (isEnter)
        {
            m_pDepthData_Wall = data.m_pDepthData_Wall;
        }
        SetActiveAreaRect(data.BottomRight, data.TopLeft);

        var Webdata = fbsm.SenserInfo.WebCamData;

        if (m_pLineDrawPt != null)
            m_pLineDrawPt.Clear();
        m_pLineDrawPt = new List<Point>();
        for (int i = 0; i < Webdata.m_pPerspectiveCornor.Length; i++)
            m_pLineDrawPt.Add(new Point(Webdata.m_pPerspectiveCornor[i].x, Webdata.m_pPerspectiveCornor[i].y));
   
    }

    /// <summary>
    /// 키넥트 로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator Init_Kinect()
    {
        Destroy_Matrix();       //! 초기화 시작하기전에 깨끗하게 정리

        m_pKi_Texture = null;
        m_bSensorReady = false;
        if (m_pThread != null && m_pThread.IsAlive)     //! 쓰레드 정지하고 제거
        {
            m_pThread.Abort();
            m_pThread = null;
        }

        _Sensor = KinectSensor.GetDefault();        //! 키넥트 센서 정보 가져오기
        while (true)
        {
            m_pKi_Texture = m_pKi_ColorSourceMng.GetColorTexture();         //! 컬러 데이터 가져와서 잘 가져와질때까지 Loop돌리기
            if (m_pKi_Texture == null)
            {
                yield return null;
                continue;
            }
            break;

        }

        while (true)
        {
            m_pDepthData = m_pKi_DepthSourceMng.GetData();            //! 뎁스 데이터 가져와서 잘 가져와질때까지 Loop돌리기
            if (m_pDepthData == null && m_pDepthData[0] == 0)
            {
                yield return null;
                continue;
            }
            break;
        }

        m_nDepthMap_Width = _Sensor.DepthFrameSource.FrameDescription.Width;         //! 뎁스맵 데이터 크기 가져오기
        m_nDepthMap_Height = _Sensor.DepthFrameSource.FrameDescription.Height;

        m_pDepthSpacePointList = new DepthSpacePoint[m_nDepthMap_Width * m_nDepthMap_Height];   //! 뎁스맵 데이터로 배열 크기 초기화
        m_pColorSpacePointList = new ColorSpacePoint[m_nDepthMap_Width * m_nDepthMap_Height];
        m_pDepthToColorTextureBuffer = new Color[m_nDepthMap_Width * m_nDepthMap_Height];

        int index = 0;
        for (int y = 0; y < m_nDepthMap_Height; y++)
        {
            for (int x = 0; x < m_nDepthMap_Width; x++)
            {
                m_pDepthSpacePointList[index] = new DepthSpacePoint();          //! 초기화
                m_pDepthSpacePointList[index].X = x;
                m_pDepthSpacePointList[index].Y = y;
                index++;
            }
        }

        invDownsampleSize = 1.0f / (float)DownsampleSize;
        m_nDepthMap_Down_Width = m_nDepthMap_Width / DownsampleSize;
        m_nDepthMap_Down_Height = m_nDepthMap_Height / DownsampleSize;      //! 다운 샘플링 크기 구하기
#if UNITY_EDITOR
        Debug.Log(m_nDepthMap_Width + "," + m_nDepthMap_Height);
#endif
        m_pKi_Texture = new Texture2D((int)m_nDepthMap_Width, (int)m_nDepthMap_Height, TextureFormat.RGBA32, false);

        SetDepthToColorTexture();

        m_pMat_Color = new Mat(m_pKi_Texture.height, m_pKi_Texture.width, CvType.CV_8UC4);
        m_pMat_Depth = new Mat((int)(m_nDepthMap_Height * invDownsampleSize), (int)(m_nDepthMap_Width * invDownsampleSize), CvType.CV_8UC4);
        Imgproc.cvtColor(m_pMat_Depth, m_pMat_Depth, Imgproc.COLOR_BGR2GRAY);

        m_pMat_DepthResize = new Mat();
        //m_pMat_DepthResize2 = new Mat();
        m_pMat_Threshold = new Mat();
        m_pMat_Threshold_Thread = new Mat();
        m_pMat_Contours = new Mat();
        m_pMat_Hierarchy = new Mat();
        m_pMat_ColorResize = new Mat();
        //m_pMat_ColorResize2 = new Mat();
        m_pPerspective = new Mat();

        //m_pMat_Threshold2 = new Mat();
        //m_pMat_Contours2 = new Mat();

        m_pMat_Curr = new Mat();
        m_pMat_Ago = new Mat();
        m_pMat_Curr2 = new Mat();
        m_pMat_Ago2 = new Mat();
        m_pThread = new Thread(SetNewHeightMap);            //! 쓰레드에 쓰레딩 돌릴 함수 넣기
        m_pThread.Start();


        yield return new WaitForSeconds(1.0f);
        m_bSensorReady = true;
    }

    /// <summary>
    /// 소멸
    /// </summary>
    public override void Destroy()
    {
        StopCoroutine("Init_Kinect");
        m_pKi_ColorSourceMng.gameObject.SetActive(false);
        m_pKi_DepthSourceMng.gameObject.SetActive(false);

        if (m_pThread != null && m_pThread.IsAlive)
        {
            m_pThread.Abort();
            m_pThread = null;
        }

        Destroy_Matrix();
        m_pDepthSpacePointList = null;
        m_pColorSpacePointList = null;
        m_pDepthToColorTextureBuffer = null;

        m_pKi_Texture = null;
        _Sensor = null;

        if (m_lContours != null)
        {
            m_lContours.Clear();
        }

        if (m_lContours2 != null)
        {
            m_lContours2.Clear();
        }

        m_pLineDrawPt = null;
    }

    /// <summary>
    /// 메트릭스 관련 변수들 메모리 제거
    /// </summary>
    void Destroy_Matrix()
    {
        SafeDispose_Mat(ref m_pMat_Color);
        SafeDispose_Mat(ref m_pMat_Depth);

        SafeDispose_Mat(ref m_pMat_DepthResize);
        //SafeDispose_Mat(ref m_pMat_DepthResize2);
        SafeDispose_Mat(ref m_pMat_Threshold);
        SafeDispose_Mat(ref m_pMat_Contours);

        SafeDispose_Mat(ref m_pMat_Hierarchy);
        SafeDispose_Mat(ref m_pMat_ColorResize);
        //SafeDispose_Mat(ref m_pMat_ColorResize2);

        SafeDispose_Mat(ref m_pPerspective);

        SafeDispose_Mat(ref m_pMat_Curr);
        SafeDispose_Mat(ref m_pMat_Ago);

        SafeDispose_Mat(ref m_pMat_Curr2);
        SafeDispose_Mat(ref m_pMat_Ago2);
        //SafeDispose_Mat(ref m_pMat_Threshold2);
        //SafeDispose_Mat(ref m_pMat_Contours2);
    }

    /// <summary>
    /// 뎁스화면과 컬러화면을 맞춰주는 함수(캘리브레이션)
    /// </summary>
    void SetDepthToColorTexture()
    {
        _Sensor.CoordinateMapper.MapDepthPointsToColorSpace(m_pDepthSpacePointList, m_pKi_DepthSourceMng.GetData(), m_pColorSpacePointList);
        Texture2D colorTexture = m_pKi_ColorSourceMng.GetColorTexture();

        for (int i = 0; i < m_pDepthToColorTextureBuffer.Length; i++)
        {
            //int x = Mathf.Clamp((int)Mathf.Floor(m_pColorSpacePointList[i].X + 0.5f), 0, m_nColorMap_Width);
            //int y = Mathf.Clamp((int)Mathf.Floor(m_pColorSpacePointList[i].Y + 0.5f), 0, m_nColorMap_Height);
            m_pDepthToColorTextureBuffer[i] = colorTexture.GetPixel((int)m_pColorSpacePointList[i].X, (int)m_pColorSpacePointList[i].Y);
        }
        m_pKi_Texture.SetPixels(m_pDepthToColorTextureBuffer);
        m_pKi_Texture.Apply();
    }

    /// <summary>
    /// 뎁스데이터 연산하는 함수(쓰레드로 돌아감)
    /// </summary>
    void SetNewHeightMap()
    {
        //! 뎁스맵 데이터를 Mat으로 바꿀 Mat
        Mat Mat_Front = new Mat((int)(m_nDepthMap_Height * invDownsampleSize), (int)(m_nDepthMap_Width * invDownsampleSize), CvType.CV_8UC4);   //! 설정한 구역뎁스값 넣을 mat
        Mat Mat_Back = new Mat((int)(m_nDepthMap_Height * invDownsampleSize), (int)(m_nDepthMap_Width * invDownsampleSize), CvType.CV_8UC4);   //! 설정한 구역 외 뎁스값 넣을 Mat
        Imgproc.cvtColor(Mat_Front, Mat_Front, Imgproc.COLOR_BGR2GRAY);
        Imgproc.cvtColor(Mat_Back, Mat_Back, Imgproc.COLOR_BGR2GRAY);

        ushort[] wallDepthData = null;

        float avg_Front = 0.0f;
        float avg_Back = 0.0f;
        while (true)
        {
            Thread.Sleep(45);

            m_pDepthData = m_pKi_DepthSourceMng.GetData();
            avg_Front = 0.0f;
            avg_Back = 0.0f;

            lock (wallLockObject)
            {
                if (m_pDepthData_Wall == null)
                {
                    wallDepthData = null;
                }
                else if (wallDepthData == null)
                {
                    wallDepthData = new ushort[m_pDepthData_Wall.Length];
                    for (int i = 0; i < m_pDepthData_Wall.Length; i++)
                    {
                        wallDepthData[i] = m_pDepthData_Wall[i];
                    }
                }
            }

            for (int y = 0; y < m_nDepthMap_Height; y += DownsampleSize)
            {
                for (int x = 0; x < m_nDepthMap_Width; x += DownsampleSize)
                {
                    int indexX = (int)(x * invDownsampleSize);
                    int indexY = (int)(y * invDownsampleSize);

                    if (indexX >= m_nDepthMap_Down_Width || indexY >= m_nDepthMap_Down_Height)
                        continue;

                    int index = (y * m_nDepthMap_Width) + x;
                    avg_Front = GetDepthDataAvg(m_pDepthData, x, y, m_nDepthMap_Width, m_nDepthMap_Height, wallDepthData); //! 다운 샘플링한 뎁스데이터 조사해서 평균 뎁스값 반환

                    if (wallDepthData == null)
                    {
                        //! 벽스캔중이 아니면 뎁스맵을 바로 Mat에 넣어버림
                        Mat_Front.put(indexY, indexX, new double[1] { (double)((avg_Front - m_fDepth_Distance_Min) / (m_fDepth_Distance_Max - m_fDepth_Distance_Min)) * 255.0f });
                    }
                    else
                    {
                        //! 벽스캔중이면 (벽뎁스 - 현재 거리)를 계산함 
                        //! 벽스캔 중이면 설정한 구역 뎁스와 설정 외 구역 뎁스를 두번 연산함
                        avg_Back = GetDepthDataAvg2(m_pDepthData, x, y, m_nDepthMap_Width, m_nDepthMap_Height, wallDepthData); //! 설정 외 구역 뎁스 
                        double fd = 0.0f;
                        double fd2 = 0.0f;

                        //! 조금이라도 인식되면 색상값을 255로 만들어버림(더 나은 인식율을 위해)
                        if (wallDepthData[index] - avg_Back > m_fDepth_Distance_Max && avg_Back > 500.0f)
                            fd = 255.0f;

                        if (wallDepthData[index] - avg_Front > m_fDepth_Distance_Min && wallDepthData[index] - avg_Front < m_fDepth_Distance_Max && fd == 0.0f)
                            fd2 = 255.0f;

                        Mat_Back.put(indexY, indexX, new double[1] { fd });
                        Mat_Front.put(indexY, indexX, new double[1] { fd2 });
                    }
                }
            }

            //! 벽스캔중이면 Mat_Back에서 잡힌 데이터를 Mat_Front에서 지워줌(구역외 인식부분 제거)
            if (wallDepthData != null)
            {
                Imgproc.dilate(Mat_Back, Mat_Back, new Mat(m_nFrontNoiseDlight, m_nFrontNoiseDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
                for (int i = 0; i < Mat_Back.cols(); i++)
                {
                    for (int j = 0; j < Mat_Back.rows(); j++)
                    {
                        if (Mat_Back.get(j, i)[0] != 0.0f)
                        {
                            Mat_Front.put(j, i, new double[1] { 0.0f });
                        }
                    }
                }
            }


            Imgproc.resize(Mat_Front, m_pMat_DepthResize, m_sSize);

            Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);
            if (m_bFilp_LeftRight == true)
            {
                Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);
            }

            if (m_bFilp_UpDown == true)
            {
                Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 0);
            }

            if (m_pLineDrawPt != null)
                m_pMat_DepthResize = Perspective(m_pLineDrawPt, m_pMat_DepthResize);

            if (m_fBlur % 2 == 0)
            {
                Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur + 1);
            }
            else
            {
                Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur);
            }

            if (m_fThreshold > 1.0f)
            {
                m_pMat_DepthResize.copyTo(m_pMat_Curr);
                if (m_bFirst == false)
                {
                    m_pMat_Curr.copyTo(m_pMat_Ago);
                    m_bFirst = true;
                }

                Core.absdiff(m_pMat_Curr, m_pMat_Ago, m_pMat_Threshold_Thread);       //! 두개의 매트릭스 비교'
                DateTime now = DateTime.Now;
                TimeSpan ts = now - oldRefreshTime;

                if (ts.TotalMilliseconds > m_fRefreshTimer)
                {
                    oldRefreshTime = now;
                    m_pMat_Curr.copyTo(m_pMat_Ago);
                }

                Imgproc.threshold(m_pMat_Threshold_Thread, m_pMat_Threshold_Thread, (double)m_fThreshold, 255, Imgproc.THRESH_BINARY);
            }
            else
            {
                m_pMat_DepthResize.copyTo(m_pMat_Threshold_Thread);
                //m_pMat_DepthResize2.copyTo(m_pMat_Threshold2);
            }

            Imgproc.erode(m_pMat_Threshold_Thread, m_pMat_Threshold_Thread, new Mat((int)m_nEroad, (int)m_nEroad, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
            Imgproc.dilate(m_pMat_Threshold_Thread, m_pMat_Threshold_Thread, new Mat((int)m_nDlight, (int)m_nDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);

            if (m_pMat_Hierarchy == null)
                continue;

            //!이부분에메트릭스 이어 주고 Contours생성후  충돌처리검사할떄 1280으로 Viewport계산
            //m_pMat_Threshold.copyTo(m_pMat_Contours);
            //Mat ele2 = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
            //Imgproc.morphologyEx(m_pMat_Contours, m_pMat_Contours, Imgproc.MORPH_CLOSE, ele2);
            //Imgproc.findContours(m_pMat_Contours, m_lContours, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기
            lock (lockObject)
            {
                m_lThreadContours.Clear();
                Imgproc.findContours(m_pMat_Threshold_Thread, m_lThreadContours, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기
            }
        }
    }

    /// <summary>
    /// 다운샘플링된 뎁스배열의 평균값 구하기
    /// 구역 내
    /// </summary>
    /// <param name="depthData"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public float GetDepthDataAvg(ushort[] depthData, int x, int y, int width, int height, ushort[] wallDepthData)
    {
        if (depthData == null)
            return 0.0f;

        int sumDepth = 0;
        int count = 0;
        for (int y1 = y; y1 < y + DownsampleSize; y1++)
        {
            for (int x1 = x; x1 < x + DownsampleSize; x1++)
            {
                int index = (y1 * width) + x1;
                if (wallDepthData != null)
                {
                    if (wallDepthData[index] - depthData[index] > m_fDepth_Distance_Min && wallDepthData[index] - depthData[index] < m_fDepth_Distance_Max)
                    {
                        ++count;
                        sumDepth += depthData[index];
                    }
                }
                else
                {
                    if (depthData[index] > m_fDepth_Distance_Min && depthData[index] < m_fDepth_Distance_Max)
                    {
                        ++count;
                        sumDepth += depthData[index];
                    }
                }
            }
        }

        return count == 0 ? 0.0f : sumDepth / (float)count;
    }

    /// <summary>
    /// 다운샘플링된 뎁스배열의 평균값 구하기
    /// 구역 외
    /// </summary>
    /// <param name="depthData"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public float GetDepthDataAvg2(ushort[] depthData, int x, int y, int width, int height, ushort[] wallDepthData)
    {
        if (depthData == null)
            return 0.0f;

        int sumDepth = 0;
        int count = 0;
        for (int y1 = y; y1 < y + DownsampleSize; y1++)
        {
            for (int x1 = x; x1 < x + DownsampleSize; x1++)
            {
                int index = (y1 * width) + x1;

                if (wallDepthData != null)
                {
                    if (wallDepthData[index] - depthData[index] > m_fDepth_Distance_Max && depthData[index] > 500.0f)
                    {
                        ++count;
                        sumDepth += depthData[index];
                    }
                }
            }
        }

        return count == 0 ? 0.0f : sumDepth / (float)count;
    }

    bool m_bFirst = false;
    /// <summary>
    /// 키넥트 연산
    /// </summary>
    /// <returns></returns>
    //    public override bool Update_Matrix()
    //    {
    //        if (m_pLineDrawPt == null || m_pLineDrawPt.Count == 0)
    //        {
    //            var fbsm = Model.First<FantaBoxSettingModel>();
    //            var data = fbsm.SenserInfo.WebCamData;

    //            m_pLineDrawPt = new List<Point>();
    //            for (int i = 0; i < data.m_pPerspectiveCornor.Length; i++)
    //                m_pLineDrawPt.Add(new Point(data.m_pPerspectiveCornor[i].x, data.m_pPerspectiveCornor[i].y));
    //        }

    //        if (m_bColor == true)      //! 컬러 값 뽑는 부분 ( m_bColor가 false면 실행하지 않음)
    //        {
    //            SetDepthToColorTexture();
    //            Utils.texture2DToMat(m_pKi_Texture, m_pMat_Color);
    //            m_pMat_Color.copyTo(m_pMat_ColorResize);
    //            //m_pMat_Color.copyTo(m_pMat_ColorResize2);

    //            Imgproc.resize(m_pMat_ColorResize, m_pMat_ColorResize, m_sSize);
    //            //Imgproc.resize(m_pMat_ColorResize2, m_pMat_ColorResize2, m_sSize);

    //            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);           
    //            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);           
    //            //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 0);
    //            //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 1);       

    //            if (m_bFilp_LeftRight == true)
    //            {
    //                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);       
    //                //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 1);   
    //            }
    //            if (m_bFilp_UpDown == true)
    //            {
    //                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);         
    //                //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 0);     
    //            }

    //            if (m_pLineDrawPt != null)
    //            {
    //                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[0], m_pLineDrawPt[1], new Scalar(255, 0, 0, 255), 2);             //라인그리기 색결정
    //                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[1], m_pLineDrawPt[2], new Scalar(255, 0, 0, 255), 2);
    //                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[2], m_pLineDrawPt[3], new Scalar(255, 0, 0, 255), 2);
    //                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[3], m_pLineDrawPt[0], new Scalar(255, 0, 0, 255), 2);
    //            }
    //        }

    //        lock (lockObject)
    //        {
    //            if (m_pMat_Depth_Thread != null)
    //                m_pMat_Depth_Thread.copyTo(m_pMat_Depth);           //! 스레드에서 연산된 뎁스Mat 받아오기
    //            else
    //                return false;
    //        }

    //        if (m_pMat_Depth == null)
    //            return false;

    //        m_pMat_Depth.copyTo(m_pMat_DepthResize);
    //        //m_pMat_Depth.copyTo(m_pMat_DepthResize2);
    //        Imgproc.resize(m_pMat_DepthResize, m_pMat_DepthResize, m_sSize);
    //        //Imgproc.resize(m_pMat_DepthResize2, m_pMat_DepthResize2, m_sSize);

    //        Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);
    //        //Core.flip(m_pMat_DepthResize2, m_pMat_DepthResize2, 1);
    //        if (m_bFilp_LeftRight == true)
    //        {
    //            Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);          
    //            //Core.flip(m_pMat_DepthResize2, m_pMat_DepthResize2, 1);
    //        }

    //        if (m_bFilp_UpDown == true)
    //        {
    //            Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 0);          
    //            //Core.flip(m_pMat_DepthResize2, m_pMat_DepthResize2, 0);
    //        }

    //        if (m_pLineDrawPt != null)
    //            m_pMat_DepthResize = Perspective(m_pLineDrawPt, m_pMat_DepthResize);

    //        if (m_fBlur % 2 == 0)
    //        {
    //            Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur + 1);
    //            //Imgproc.medianBlur(m_pMat_DepthResize2, m_pMat_DepthResize2, (int)m_fBlur + 1);
    //        }
    //        else
    //        {
    //            Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur);
    //            //Imgproc.medianBlur(m_pMat_DepthResize2, m_pMat_DepthResize2, (int)m_fBlur);
    //        }

    //        if (m_fThreshold > 1.0f)
    //        {
    //            m_pMat_DepthResize.copyTo(m_pMat_Curr);
    //            //m_pMat_DepthResize2.copyTo(m_pMat_Curr2);
    //            if (m_bFirst == false)
    //            {
    //                m_pMat_Curr.copyTo(m_pMat_Ago);
    //  //              m_pMat_Curr2.copyTo(m_pMat_Ago2);
    //                m_bFirst = true;
    //            }

    //            Core.absdiff(m_pMat_Curr, m_pMat_Ago, m_pMat_Threshold);       //! 두개의 매트릭스 비교'
    //            //Core.absdiff(m_pMat_Curr2, m_pMat_Ago2, m_pMat_Threshold2);       //! 두개의 매트릭스 비교'

    //            if (Time.time - oldRefreshTime > m_fRefreshTimer)
    //            {
    //                oldRefreshTime = Time.time;
    //                m_pMat_Curr.copyTo(m_pMat_Ago);
    ////                m_pMat_Curr2.copyTo(m_pMat_Ago2);
    //            }

    //            Imgproc.threshold(m_pMat_Threshold, m_pMat_Threshold, (double)m_fThreshold, 255, Imgproc.THRESH_BINARY);
    //            //Imgproc.threshold(m_pMat_Threshold2, m_pMat_Threshold2, (double)m_fThreshold, 255, Imgproc.THRESH_BINARY);
    //        }
    //        else
    //        {
    //            m_pMat_DepthResize.copyTo(m_pMat_Threshold);
    //            //m_pMat_DepthResize2.copyTo(m_pMat_Threshold2);
    //        }

    //        Imgproc.erode(m_pMat_Threshold, m_pMat_Threshold, new Mat((int)m_nEroad, (int)m_nEroad, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
    //        Imgproc.dilate(m_pMat_Threshold, m_pMat_Threshold, new Mat((int)m_nDlight, (int)m_nDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);

    //        //Imgproc.erode(m_pMat_Threshold2, m_pMat_Threshold2, new Mat((int)m_nEroad, (int)m_nEroad, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
    //        //Imgproc.dilate(m_pMat_Threshold2, m_pMat_Threshold2, new Mat((int)m_nDlight, (int)m_nDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);

    //        if (m_pMat_Hierarchy == null)
    //            return false;

    //        //if (ConfigMng.Instance.m_eBuildType == ConfigMng.E_Game_BuildType.BUILD_SLIDE)
    //        //{
    //        //    /// 여기서 다시 두번째 감지영역을 설정해 보자
    //        //    m_lContours2.Clear();
    //        //    m_pMat_Threshold2.copyTo(m_pMat_Contours2);
    //        //    Mat ele3 = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
    //        //    Imgproc.morphologyEx(m_pMat_Contours2, m_pMat_Contours2, Imgproc.MORPH_CLOSE, ele3);
    //        //    Imgproc.findContours(m_pMat_Contours2, m_lContours2, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기

    //        //    foreach (MatOfPoint i in m_lContours2)            //! 탐색된 박스 설정
    //        //    {
    //        //        OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(i.toArray()));      //! 박스
    //        //        Imgproc.rectangle(m_pMat_Threshold2, boundRect.tl(), boundRect.br(), new Scalar(255, 255, 255, 255), 2, 8, 0);
    //        //    }
    //        //}

    //        //!이부분에메트릭스 이어 주고 Contours생성후  충돌처리검사할떄 1280으로 Viewport계산
    //        m_lContours.Clear();         //! 박스리스트 초기화s
    //        m_pMat_Threshold.copyTo(m_pMat_Contours);
    //        Mat ele2 = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
    //        Imgproc.morphologyEx(m_pMat_Contours, m_pMat_Contours, Imgproc.MORPH_CLOSE, ele2);
    //        Imgproc.findContours(m_pMat_Contours, m_lContours, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기

    //        foreach (MatOfPoint i in m_lContours)            //! 탐색된 박스 설정
    //        {
    //            OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(i.toArray()));      //! 박스
    //            Imgproc.rectangle(m_pMat_Threshold, boundRect.tl(), boundRect.br(), new Scalar(255, 255, 255, 255), 2, 8, 0);
    //        }

    //        return base.Update_Matrix();
    //    }


    public override bool Update_Matrix()
    {

        m_pKi_ColorSourceMng.gameObject.SetActive(m_bColor);
        if (m_bColor == true)      //! 컬러 값 뽑는 부분 ( m_bColor가 false면 실행하지 않음)
        {
            if (m_pLineDrawPt == null || m_pLineDrawPt.Count == 0)
        {
            var fbsm = Model.First<FantaBoxSettingModel>();
            var data = fbsm.SenserInfo.WebCamData;

            m_pLineDrawPt = new List<Point>();
            for (int i = 0; i < data.m_pPerspectiveCornor.Length; i++)
                m_pLineDrawPt.Add(new Point(data.m_pPerspectiveCornor[i].x, data.m_pPerspectiveCornor[i].y));
        }

            SetDepthToColorTexture();
            Utils.texture2DToMat(m_pKi_Texture, m_pMat_Color);
            m_pMat_Color.copyTo(m_pMat_ColorResize);
            //m_pMat_Color.copyTo(m_pMat_ColorResize2);

            Imgproc.resize(m_pMat_ColorResize, m_pMat_ColorResize, m_sSize);
            //Imgproc.resize(m_pMat_ColorResize2, m_pMat_ColorResize2, m_sSize);

            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);
            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);
            //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 0);
            //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 1);       

            if (m_bFilp_LeftRight == true)
            {
                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);
                //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 1);   
            }
            if (m_bFilp_UpDown == true)
            {
                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);
                //Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 0);     
            }

            if (m_pLineDrawPt != null)
            {
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[0], m_pLineDrawPt[1], new Scalar(255, 0, 0, 255), 2);             //라인그리기 색결정
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[1], m_pLineDrawPt[2], new Scalar(255, 0, 0, 255), 2);
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[2], m_pLineDrawPt[3], new Scalar(255, 0, 0, 255), 2);
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[3], m_pLineDrawPt[0], new Scalar(255, 0, 0, 255), 2);
            }
        }

        m_lContours.Clear();         //! 박스리스트 초기화s

        lock (lockObject)
        {//쓰레드 더블버퍼링.
            for (int i = 0; i < m_lThreadContours.Count; i++)
            {
                m_lContours.Add(m_lThreadContours[i]);
            }
            m_pMat_Threshold_Thread.copyTo(m_pMat_Threshold);
        }

        foreach (MatOfPoint i in m_lContours)            //! 탐색된 박스 설정
        {
            OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(i.toArray()));      //! 박스
            Imgproc.rectangle(m_pMat_Threshold, boundRect.tl(), boundRect.br(), new Scalar(255, 255, 255, 255), 2, 8, 0);
        }

        return base.Update_Matrix();
    }

    public byte[] Send(Mat pMat)
    {
        int size = (int)(pMat.total() * pMat.elemSize());
        byte[] bytes = new byte[size / 2];

        int nCnt = 0;
        for (int i = 0; i < pMat.cols(); i += 2)
        {
            for (int j = 0; j < pMat.rows(); j += 2)
            {
                if (nCnt >= size) break;
                bytes[nCnt] = (byte)(pMat.get(j, i)[0] == 0 ? 0 : 1);
                nCnt++;
            }
        }
        return bytes;
    }

    /// <summary>
    /// 벽 스캔
    /// </summary>
    public void WallScan()
    {
        print("m_bSensorReady " + m_bSensorReady);
        if (m_bSensorReady == false) return;
        lock (wallLockObject)
        {
            if (m_pDepthData_Wall != null) return;
            ushort[] depthData = m_pKi_DepthSourceMng.GetData();
            m_pDepthData_Wall = new ushort[depthData.Length];
            Array.Copy(depthData, m_pDepthData_Wall, depthData.Length);
        }
        print("WallScan : " + Model.First<FantaBoxSettingModel>());
        Model.First<FantaBoxSettingModel>().SenserInfo.KinectData.m_pDepthData_Wall = m_pDepthData_Wall;
        m_bWallScan = true;
    }

    public void WallScanCancel()
    {
        print(" WallScanCancel");
        lock (wallLockObject)
        {
            m_pDepthData_Wall = null;
        }
        m_bWallScan = false;
    }

    void SafeDispose_Mat(ref Mat pMat)
    {
        if (pMat != null)
        {
            pMat.Dispose();
            pMat = null;
        }
    }

    void OnApplicationQuit()
    {
        if (m_pThread != null && m_pThread.IsAlive)
        {
            m_pThread.Abort();
            m_pThread = null;
        }
    }

    void OnDestroy()
    {
        if (m_pThread != null && m_pThread.IsAlive)
        {
            m_pThread.Abort();
            m_pThread = null;
        }

        m_pKi_ColorSourceMng = null;
        m_pKi_DepthSourceMng = null;
    }

    public override Mat Get_ColorMat()
    {
        if (m_bSensorReady == false)
            return null;

        return m_pMat_ColorResize;
    }

    //public override Mat Get_ColorMat2()
    //{
    //    if (m_bSensorReady == false)
    //        return null;

    //    return m_pMat_ColorResize2;
    //}

    public override Mat Get_ThresoldMat()
    {
        if (m_bSensorReady == false)
            return null;

        return m_pMat_Threshold;
    }

    //public override Mat Get_ThresoldMat2()
    //{
    //    if (m_bSensorReady == false)
    //        return null;

    //    return m_pMat_Threshold2;
    //}

    public override void SaveSetting()
    {
        var mPoint = GetSavePoint();
        var fbsm = Model.First<FantaBoxSettingModel>();
        fbsm.Save_KinectSetting(m_fDepth_Distance_Min, m_fDepth_Distance_Max, m_fBlur,
            m_nEroad, m_nDlight, m_nFrontNoiseDlight, m_bWallScan, m_bFilp_UpDown, m_bFilp_LeftRight,
            mPoint, mPoint, m_pDepthData_Wall, m_fThreshold, m_nCamValue, m_nServerState, m_sIP);
    }

    public Texture2D GetColorTexture()
    {
        return m_pKi_Texture;
    }

    public Vector2 GetDepthTextureSize()
    {
        return new Vector2(m_nDepthMap_Width, m_nDepthMap_Height);
    }
}

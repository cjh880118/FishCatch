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
public class OpenCV_RealSense2 : IOpenCV
{
    // 매 프레임 계산을 시간마다로 개선
    internal float m_fRefreshTimer = 0.5f;
    float oldRefreshTime = 0f;
    public RsDevice mRsDevice;

    [SerializeField]
    Texture2D m_pKi_Texture = null;             //! 컬러데이터를 받을 텍스쳐
                                                // Texture2D m_pKi_Texture_Depth = null;

    [SerializeField]
    private RsColorSourceManager m_pKi_ColorSourceMng = null;     //! 칼라값을 가져올 수있는 키넥트 기본 클래스
    [SerializeField]
    private RsDepthSourceManager m_pKi_DepthSourceMng = null;     //! 뎁스값을 가져올 수 있는 키넥트 기본 클래스

    ushort[] m_pDepthData = null;      //! 뎁스값을 저장 할 배열
    ushort[] m_pDepthData_Wall = null;      //! 벽뎁스 값을 저장할 배열

    int m_nDepthMap_Down_Width = 0;       //! 뎁스맵 다운샘플링하고나서 크기
    int m_nDepthMap_Down_Height = 0;       //
    internal const int DownsampleSize = 2;  //! 다운샘플링 수치
    float invDownsampleSize = 0;

    DepthSpacePoint[] m_pDepthSpacePointList = null;        //! 컬러화면과 뎁스화면 캘리브레이션 할 때 필요한 배열들
    ColorSpacePoint[] m_pColorSpacePointList = null;        //
    Color[] m_pDepthToColorTextureBuffer = null;        //

    Mat m_pMat_Color = null;         //! 컬러값 저장용 Mat       
    Mat m_pMat_ColorResize = null;         //! 컬러값 받아서 640x480으로 만들어 주기위한 Mat
                                           // Mat m_pMat_ColorResize2 = null;         //! 컬러값 받아서 640x480으로 만들어 주기위한 Mat
    Mat m_pMat_Depth = null;         //! 뎁스값 저장용 Mat
    Mat m_pMat_Depth_Thread = null;         //! Thread에서 연산된 뎁스값 저장용 Mat
    Mat m_pMat_DepthResize = null;         //! 뎁스값 받아서 640x480으로 만들어 주기 위한 Mat
                                           //    Mat m_pMat_DepthResize2 = null;
    Mat m_pMat_Threshold = null;         //! 뎁스값 이후 필터 처리용 Mat
    Mat m_pMat_Contours = null;         //! 감지 박스 가져오기 용 Mat
    Mat m_pMat_Hierarchy = null;         //\

    Mat m_pMat_Curr = null;
    Mat m_pMat_Ago = null;

    Thread m_pThread = null;             //! 쓰레드      
    object lockObject = new object();     //

    public float m_fDepth_Distance_Min = 500.0f;      //! 뎁스 최소거리
    public float m_fDepth_Distance_Max = 600.0f;      //! 뎁스 최대거리

    public bool m_bFilp_UpDown = false;       //! 위아래 반전
    public bool m_bFilp_LeftRight = false;       //! 좌우 반전
    public float m_fBlur = 3.0f;        //! 블러
    public int m_nEroad = 3;           //! 축소
    public int m_nDlight = 3;           //! 확장
    public int m_nFrontNoiseDlight = 13;          //! 뎁스 거리 외 인지된 부분 제거 할 크기 확장
    public bool m_bWallScan = false;       //! 벽스캔 On/Off
    public float m_fThreshold = 30.0f;

    internal bool m_bColor = false;         //! 컬러값 가져올지 안가져올지 판단

    internal int m_nCamValue = 0;
    internal int m_nServerState = 0;
    internal string m_sIP = "127.0.0.1";

    public float MAXDepthVal = 500f;

    public override void Enter(Vector2 txtSize, bool color = true)
    {
        base.Enter(txtSize, color);

        m_bColor = color;
        m_pKi_ColorSourceMng.gameObject.SetActive(true);
        m_pKi_DepthSourceMng.gameObject.SetActive(true);

        var fbsm = Model.First<FantaBoxSettingModel>();
        var data = fbsm.SenserInfo.KinectData;

        m_bWallScan = data.m_bWallScan;                   //! 저장된 데이터 가져오기
        m_fDepth_Distance_Min = data.m_fDistance_Min;     //
        m_fDepth_Distance_Max = data.m_fDistance_Max;     //
        m_bFilp_UpDown = data.m_bFilp_UpDown;             //
        m_bFilp_LeftRight = data.m_bFilp_LeftRight;       //
        Debug.LogError("블러브러러러러러러ㅓ:" + data.m_fBlur);
        m_fBlur = data.m_fBlur;                           //
        m_nEroad = data.m_nEroad;                         //
        m_nDlight = data.m_nDlight;                       //
        m_nFrontNoiseDlight = data.m_nFilterDlight;       //
        m_fThreshold = data.m_fThreshold;

        m_nCamValue = data.m_nCamValue;

        m_nServerState = data.m_nServerState;
        m_sIP = data.m_sIP;

        m_pDepthData_Wall = data.m_pDepthData_Wall;       //

        RealSenseStart();
    }

    public void RealSenseStart()
    {
        if (mRsDevice == null)
            mRsDevice = GetComponent<RsDevice>();

        Destroy_Matrix();
        StartCoroutine(InitRealSense());

        string TextFomat = string.Format("리얼센스 시작시간:{0}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        TextFileOutPut.Instance.SaveStringLine(TextFomat);
    }

    public void RealSenseStop()
    {
        if (mRsDevice == null)
            mRsDevice = GetComponent<RsDevice>();

        mRsDevice.Stop();

        string TextFomat = string.Format("리얼센스 멈춘시간:{0}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        TextFileOutPut.Instance.SaveStringLine(TextFomat);
    }

    /// <summary>
    /// 키넥트 로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitRealSense()
    {
        Destroy_Matrix();       //! 초기화 시작하기전에 깨끗하게 정리

        m_pKi_Texture = null;
        m_bSensorReady = false;
        if (m_pThread != null && m_pThread.IsAlive)     //! 쓰레드 정지하고 제거
        {
            m_pThread.Abort();
            m_pThread = null;
        }

        mRsDevice.Enter();

        while (true)
        {
            m_pKi_Texture = m_pKi_ColorSourceMng.texture;         //! 컬러 데이터 가져와서 잘 가져와질때까지 Loop돌리기
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

        m_pDepthSpacePointList = new DepthSpacePoint[(int)m_sSize.width * (int)m_sSize.height];   //! 뎁스맵 데이터로 배열 크기 초기화
        m_pColorSpacePointList = new ColorSpacePoint[(int)m_sSize.width * (int)m_sSize.height];
        m_pDepthToColorTextureBuffer = new Color[(int)m_sSize.width * (int)m_sSize.height];
        int index = 0;
        for (int y = 0; y < (int)m_sSize.height; y++)
        {
            for (int x = 0; x < (int)m_sSize.width; x++)
            {
                m_pDepthSpacePointList[index] = new DepthSpacePoint();          //! 초기화
                m_pDepthSpacePointList[index].X = x;
                m_pDepthSpacePointList[index].Y = y;
                index++;
            }
        }

        invDownsampleSize = 1.0f / (float)DownsampleSize;
        m_nDepthMap_Down_Width = (int)m_sSize.width / DownsampleSize;
        m_nDepthMap_Down_Height = (int)m_sSize.height / DownsampleSize;      //! 다운 샘플링 크기 구하기
#if UNITY_EDITOR
        Debug.Log((int)m_sSize.width + "," + (int)m_sSize.height);
#endif
        m_pMat_Color = new Mat(m_pKi_Texture.height, m_pKi_Texture.width, CvType.CV_8UC4);
        m_pMat_Depth = new Mat((int)((int)m_sSize.height * invDownsampleSize), (int)((int)m_sSize.width * invDownsampleSize), CvType.CV_8UC4);
        m_pMat_Depth_Thread = new Mat((int)((int)m_sSize.height * invDownsampleSize), (int)((int)m_sSize.width * invDownsampleSize), CvType.CV_8UC4);
        Imgproc.cvtColor(m_pMat_Depth, m_pMat_Depth, Imgproc.COLOR_BGR2GRAY);
        Imgproc.cvtColor(m_pMat_Depth_Thread, m_pMat_Depth_Thread, Imgproc.COLOR_BGR2GRAY);

        m_pMat_DepthResize = new Mat();
        m_pMat_Threshold = new Mat();
        m_pMat_Contours = new Mat();
        m_pMat_Hierarchy = new Mat();
        m_pMat_ColorResize = new Mat();

        m_lContours = new List<MatOfPoint>();
        m_pMat_Curr = new Mat();
        m_pMat_Ago = new Mat();
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
        mRsDevice = null;

        if (m_lContours != null)
        {
            m_lContours.Clear();
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

        lock (lockObject)
        {
            SafeDispose_Mat(ref m_pMat_Depth_Thread);           //! 쓰레드하고 관련된 메트릭스라 Lock걸어 놓고 제거
        }

        SafeDispose_Mat(ref m_pMat_DepthResize);
        SafeDispose_Mat(ref m_pMat_Threshold);
        SafeDispose_Mat(ref m_pMat_Contours);

        SafeDispose_Mat(ref m_pMat_Hierarchy);
        SafeDispose_Mat(ref m_pMat_ColorResize);

        SafeDispose_Mat(ref m_pPerspective);

        SafeDispose_Mat(ref m_pMat_Curr);
        SafeDispose_Mat(ref m_pMat_Ago);
    }

    /// <summary>
    /// 뎁스화면과 컬러화면을 맞춰주는 함수(캘리브레이션)
    /// </summary>
    void SetDepthToColorTexture()
    {
        Texture2D colorTexture = m_pKi_ColorSourceMng.texture;

        for (int i = 0; i < m_pDepthToColorTextureBuffer.Length; i++)
            m_pDepthToColorTextureBuffer[i] = colorTexture.GetPixel((int)m_pColorSpacePointList[i].X, (int)m_pColorSpacePointList[i].Y);

        m_pKi_Texture.SetPixels(m_pDepthToColorTextureBuffer);
        m_pKi_Texture.Apply();
    }

    /// <summary>
    /// 뎁스데이터 연산하는 함수(쓰레드로 돌아감)
    /// </summary>
    void SetNewHeightMap()
    {
        //! 뎁스맵 데이터를 Mat으로 바꿀 Mat
        Mat Mat_Front = new Mat((int)((int)m_sSize.height * invDownsampleSize), (int)((int)m_sSize.width * invDownsampleSize), CvType.CV_8UC4);   //! 설정한 구역뎁스값 넣을 mat
        Mat Mat_Back = new Mat((int)((int)m_sSize.height * invDownsampleSize), (int)((int)m_sSize.width * invDownsampleSize), CvType.CV_8UC4);   //! 설정한 구역 외 뎁스값 넣을 Mat
        Imgproc.cvtColor(Mat_Front, Mat_Front, Imgproc.COLOR_BGR2GRAY);
        Imgproc.cvtColor(Mat_Back, Mat_Back, Imgproc.COLOR_BGR2GRAY);

        float avg_Front = 0.0f;
        float avg_Back = 0.0f;

        while (true)
        {
            Thread.Sleep(45);

            m_pDepthData = m_pKi_DepthSourceMng.GetData();
            //  Debug.Log("댑스맵 스레드111");
            avg_Front = 0.0f;
            avg_Back = 0.0f;

            for (int y = 0; y < (int)m_sSize.height; y += DownsampleSize)
            {
                for (int x = 0; x < (int)m_sSize.width; x += DownsampleSize)
                {
                    int indexX = (int)(x * invDownsampleSize);
                    int indexY = (int)(y * invDownsampleSize);

                    if (indexX >= m_nDepthMap_Down_Width ||
                        indexY >= m_nDepthMap_Down_Height)
                    {
                        continue;
                    }

                    int index = (y * (int)m_sSize.width) + x;
                    avg_Front = GetDepthDataAvg(m_pDepthData, x, y, (int)m_sSize.width, (int)m_sSize.height); //! 다운 샘플링한 뎁스데이터 조사해서 평균 뎁스값 반환

                    if (m_pDepthData_Wall == null)
                    {
                        // !벽스캔중이 아니면 뎁스맵을 바로 Mat에 넣어버림
                        Mat_Front.put(indexY, indexX, new double[1] { (double)((avg_Front - m_fDepth_Distance_Min) / (m_fDepth_Distance_Max - m_fDepth_Distance_Min)) * 255.0f });
                    }
                    else
                    {
                        //! 벽스캔중이면 (벽뎁스 - 현재 거리)를 계산함 
                        //! 벽스캔 중이면 설정한 구역 뎁스와 설정 외 구역 뎁스를 두번 연산함
                        avg_Back = GetDepthDataAvg2(m_pDepthData, x, y, (int)m_sSize.width, (int)m_sSize.height); //! 설정 외 구역 뎁스 
                        double fd = 0.0f;
                        double fd2 = 0.0f;

                        //! 조금이라도 인식되면 색상값을 255로 만들어버림(더 나은 인식율을 위해)
                        if (m_pDepthData_Wall[index] - avg_Back > m_fDepth_Distance_Max && avg_Back > MAXDepthVal)
                            fd = 255.0f;

                        if (m_pDepthData_Wall[index] - avg_Front > m_fDepth_Distance_Min && m_pDepthData_Wall[index] - avg_Front < m_fDepth_Distance_Max && fd == 0.0f)
                            fd2 = 255.0f;

                        Mat_Back.put(indexY, indexX, new double[1] { fd });
                        Mat_Front.put(indexY, indexX, new double[1] { fd2 });
                    }
                }
            }

            lock (lockObject)
            {
                if (m_pMat_Depth_Thread == null)
                {
                    Debug.Log("____CurrentNull____");
                    return;
                }

                //! 벽스캔중이면 Mat_Back에서 잡힌 데이터를 Mat_Front에서 지워줌(구역외 인식부분 제거)
                if (m_pDepthData_Wall != null)
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

                Mat_Front.copyTo(m_pMat_Depth_Thread);
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
    public float GetDepthDataAvg(ushort[] depthData, int x, int y, int width, int height)
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
                if (m_pDepthData_Wall != null)
                {
                    if (m_pDepthData_Wall[index] - depthData[index] > m_fDepth_Distance_Min && m_pDepthData_Wall[index] - depthData[index] < m_fDepth_Distance_Max)
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
    public float GetDepthDataAvg2(ushort[] depthData, int x, int y, int width, int height)
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

                if (m_pDepthData_Wall != null)
                {
                    if (m_pDepthData_Wall[index] - depthData[index] > m_fDepth_Distance_Max && depthData[index] > 500.0f)
                    {
                        ++count;
                        sumDepth += depthData[index];
                    }
                }
            }
        }

        return count == 0 ? 0.0f : sumDepth / (float)count;
    }

    int dilateIter = 3;
    int erodeIter = 3;
    bool wallscanColorChange = false;
    bool m_bFirst = false;
    /// <summary>
    /// 키넥트 연산
    /// </summary>
    /// <returns></returns>
    public override bool Update_Matrix()
    {
        m_bColor = true;
        if (m_bColor == true)      //! 컬러 값 뽑는 부분 ( m_bColor가 false면 실행하지 않음)
        {
            Utils.texture2DToMat(m_pKi_Texture, m_pMat_Color);
            m_pMat_Color.copyTo(m_pMat_ColorResize);

            Imgproc.resize(m_pMat_ColorResize, m_pMat_ColorResize, m_sSize);
            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);           //! 좌우반전
            Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);           //! 좌우반전

            if (m_bFilp_LeftRight == true)
                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 1);           //! 좌우반전
                                                                                // Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 1);           //! 좌우반전
            if (m_bFilp_UpDown == true)
                Core.flip(m_pMat_ColorResize, m_pMat_ColorResize, 0);           //! 좌우반전
                                                                                // Core.flip(m_pMat_ColorResize2, m_pMat_ColorResize2, 0);           //! 좌우반전

            if (m_pLineDrawPt != null)
            {
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[0], m_pLineDrawPt[1], new Scalar(255, 0, 0, 255), 2);             //라인그리기 색결정
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[1], m_pLineDrawPt[2], new Scalar(255, 0, 0, 255), 2);
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[2], m_pLineDrawPt[3], new Scalar(255, 0, 0, 255), 2);
                Imgproc.line(m_pMat_ColorResize, m_pLineDrawPt[3], m_pLineDrawPt[0], new Scalar(255, 0, 0, 255), 2);
            }
        }

        lock (lockObject)
        {
            if (m_pMat_Depth_Thread != null)
                m_pMat_Depth_Thread.copyTo(m_pMat_Depth);           //! 스레드에서 연산된 뎁스Mat 받아오기
            else
                return false;
        }

        if (m_pMat_Depth == null)
            return false;

        m_pMat_Depth.copyTo(m_pMat_DepthResize);
        Imgproc.resize(m_pMat_DepthResize, m_pMat_DepthResize, m_sSize);

        Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);
        if (m_bFilp_LeftRight == true)
            Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 1);           //! 좌우반전
                                                                            //    Core.flip(m_pMat_DepthResize2, m_pMat_DepthResize2, 1);
        if (m_bFilp_UpDown == true)
            Core.flip(m_pMat_DepthResize, m_pMat_DepthResize, 0);           //! 좌우반전
                                                                            //     Core.flip(m_pMat_DepthResize2, m_pMat_DepthResize2, 0);

        if (m_pLineDrawPt != null)
            m_pMat_DepthResize = Perspective(m_pLineDrawPt, m_pMat_DepthResize, true);

        if (m_fBlur % 2 == 0)
            Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur + 1);
        else
            Imgproc.medianBlur(m_pMat_DepthResize, m_pMat_DepthResize, (int)m_fBlur);

        if (m_fThreshold > 1.0f)
        {
            m_pMat_DepthResize.copyTo(m_pMat_Curr);
            if (m_bFirst == false)
            {
                m_pMat_Curr.copyTo(m_pMat_Ago);
                m_bFirst = true;
            }

            Core.absdiff(m_pMat_Curr, m_pMat_Ago, m_pMat_Threshold);       //! 두개의 매트릭스 비교'

            if (Time.time - oldRefreshTime > m_fRefreshTimer)
            {
                oldRefreshTime = Time.time;
                m_pMat_Curr.copyTo(m_pMat_Ago);
            }

            Imgproc.threshold(m_pMat_Threshold, m_pMat_Threshold, (double)m_fThreshold, 255, Imgproc.THRESH_BINARY);
        }
        else
        {
            m_pMat_DepthResize.copyTo(m_pMat_Threshold);
        }

        Mat erodeKernel = Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(m_nEroad, m_nEroad));
        Mat dilateKernel = Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(m_nDlight, m_nDlight));

        if (Input.GetKeyDown(KeyCode.Alpha1)) { erodeIter = 1; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { erodeIter = 3; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { erodeIter = 5; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { erodeIter = 7; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { erodeIter = 9; }

        if (Input.GetKeyDown(KeyCode.W)) { dilateIter = 1; }
        if (Input.GetKeyDown(KeyCode.E)) { dilateIter = 3; }
        if (Input.GetKeyDown(KeyCode.R)) { dilateIter = 5; }
        if (Input.GetKeyDown(KeyCode.T)) { dilateIter = 7; }
        if (Input.GetKeyDown(KeyCode.Y)) { dilateIter = 9; }

        if (Input.GetKeyDown(KeyCode.U)) { wallscanColorChange = !wallscanColorChange; }

        for (int i = 0; i < erodeIter; i++)
            Imgproc.erode(m_pMat_Threshold, m_pMat_Threshold, erodeKernel, new Point(-1, -1));

        for (int i = 0; i < dilateIter; i++)
            Imgproc.dilate(m_pMat_Threshold, m_pMat_Threshold, dilateKernel, new Point(-1, -1));

        if (m_pMat_Hierarchy == null || m_lContours == null)
            return false;

        //!이부분에메트릭스 이어 주고 Contours생성후  충돌처리검사할떄 1280으로 Viewport계산
        m_lContours.Clear();         //! 박스리스트 초기화s
        m_pMat_Threshold.copyTo(m_pMat_Contours);
        Mat ele2 = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
        Imgproc.morphologyEx(m_pMat_Contours, m_pMat_Contours, Imgproc.MORPH_CLOSE, ele2);
        Imgproc.findContours(m_pMat_Contours, m_lContours, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기

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
        if (m_bSensorReady == false || m_pDepthData_Wall != null)
            return;

        m_bWallScan = true;
        StopCoroutine(WallScanCoroutine());
        StartCoroutine(WallScanCoroutine());
    }

    IEnumerator WallScanCoroutine()
    {
        m_pDepthData_Wall = new ushort[m_pDepthData.Length];

        m_pDepthData = m_pKi_DepthSourceMng.GetData();
        for (int j = 0; j < m_pDepthData.Length; j++)
            m_pDepthData_Wall[j] += (ushort)(m_pDepthData[j] * 0.05f);

        yield return new WaitForSeconds(0.1f);

        m_bWallScan = true;
    }

    public void WallScanCancel()
    {
        m_pDepthData_Wall = null;
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
        if (m_bSensorReady == false) return null;
        return m_pMat_ColorResize;
    }

    public override Mat Get_ThresoldMat()
    {
        if (m_bSensorReady == false) return null;
        return m_pMat_Threshold;
    }

    public List<MatOfPoint> Get_Contours()
    {
        if (m_bSensorReady == false) return null;
        return m_lContours;
    }

    public override void SaveSetting() ///
    {
        var mPoint = GetSavePoint();
        var fbsm = Model.First<FantaBoxSettingModel>();
        fbsm.Save_KinectSetting(m_fDepth_Distance_Min, m_fDepth_Distance_Max, m_fBlur, m_nEroad, 
            m_nDlight, m_nFrontNoiseDlight, m_bWallScan, m_bFilp_UpDown, m_bFilp_LeftRight,mPoint, mPoint,
            m_pDepthData_Wall, m_fThreshold, m_nCamValue, m_nServerState, m_sIP);
    }
}

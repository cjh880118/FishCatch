using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System.Collections.Generic;
using CellBig.Models;

public class OpenCV_WebCam : IOpenCV
{
    internal WebCamTexture webCamTexture = null;           //! 웹캡 텍스쳐
    internal WebCamDevice webCamDevice;                   //! 웹캠 디바이스

    Color32[] m_pColors = null;
    bool m_bFrist = false;

    Mat m_pMat_WebCam = null;             //! 웹캠 데이터 저장용 Mat
    Mat m_pMat_Resize = null;             //
    Mat m_pMat_Gray = null;             //! 흑백으로 바꾸고 이전 화면하고 비교할 Mat
    Mat m_pMat_Gray_Prev = null;             //
    Mat m_pMat_Threshold = null;             //! Threshold 연산할 Mat

    Mat m_pMat_Contours = null;
    Mat m_pMat_Hierarchy = null;

    internal bool m_bFilp_UpDown = false;
    internal bool m_bFilp_LeftRight = false;
    internal float m_fThreshold = 30.0f;
    internal float m_fBlur = 3.0f;
    internal int m_nEroad = 3;
    internal int m_nDlight = 3;

    public override void Enter(Vector2 txtSize, bool color = true)
    {
        base.Enter(txtSize, color);

        var fbsm = Model.First<FantaBoxSettingModel>();
        var data = fbsm.SenserInfo.WebCamData;

        m_bFilp_UpDown = data.m_bFilp_UpDown;         //! 저장된 데이터 가져오기
        m_bFilp_LeftRight = data.m_bFilp_LeftRight;
        m_fThreshold = data.m_fThreshold;
        m_fBlur = data.m_fBlur;
        m_nEroad = data.m_nEroad;
        m_nDlight = data.m_nDlight;

        StartCoroutine("Init_WebCam");
    }

    public void WebCamSet()
    {
        StartCoroutine("Init_WebCam");
    }

    /// <summary>
    /// 웹캠 로드
    /// </summary>
    /// <returns></returns>
    IEnumerator Init_WebCam()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            webCamTexture = null;
        }

        if (WebCamTexture.devices.Length < 1)
            yield break;

        if (webCamTexture == null)      //! 웹캠세팅
        {
            webCamDevice = WebCamTexture.devices[0];        //! 디바이스 가져오기
            webCamTexture = new WebCamTexture(webCamDevice.name, (int)m_sSize.width, (int)m_sSize.height);    //! 웹캠 텍스쳐 설정  
        }

        webCamTexture.Play();

#if UNITY_EDITOR
        Debug.Log("width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
#endif
        while (true)
        {
            if (webCamTexture.didUpdateThisFrame)
            {
#if UNITY_EDITOR
                Debug.Log(webCamDevice.isFrontFacing);
#endif
                StartCoroutine("Init_Matrix");
                break;
            }
            else
            {
                yield return 0;
            }
        }
    }

    /// <summary>
    /// 데이터 및 웹캠 로드
    /// </summary>
    /// <returns></returns>
    IEnumerator Init_Matrix()
    {
        Destroy_Matrix();
        m_bFrist = false;
        while (true)
        {
            if (webCamTexture == null)
                yield return null;

            if (webCamTexture.didUpdateThisFrame == true)
            {
                m_pColors = new Color32[webCamTexture.width * webCamTexture.height];
                m_pMat_WebCam = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
                m_pMat_Resize = new Mat();
                m_pMat_Gray = new Mat();
                m_pMat_Gray_Prev = new Mat();
                m_pMat_Threshold = new Mat();
                m_pMat_Contours = new Mat();
                m_pMat_Hierarchy = new Mat();
                m_pPerspective = new Mat();
                m_bSensorReady = true;

                break;
            }
            else
            {
                yield return null;
            }
        }
        yield return null;
    }

    public override void Destroy()
    {
        StopCoroutine("Init_WebCam");
        StopCoroutine("Init_Matrix");

        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            webCamTexture = null;
        }

        Destroy_Matrix();

        if (m_lContours != null)
        {
            m_lContours.Clear();
        }

        m_pColors = null;
        m_pLineDrawPt = null;
    }

    void Destroy_Matrix()
    {
        SafeDispose_Mat(ref m_pMat_WebCam);
        SafeDispose_Mat(ref m_pMat_Resize);
        SafeDispose_Mat(ref m_pMat_Gray);
        SafeDispose_Mat(ref m_pMat_Gray_Prev);
        SafeDispose_Mat(ref m_pMat_Threshold);
        SafeDispose_Mat(ref m_pMat_Contours);
        SafeDispose_Mat(ref m_pMat_Hierarchy);
        SafeDispose_Mat(ref m_pPerspective);
    }

    void SafeDispose_Mat(ref Mat pMat)
    {
        if (pMat != null)
        {
            pMat.Dispose();
            pMat = null;
        }
    }

    public override bool Update_Matrix()
    {
        Utils.webCamTextureToMat(webCamTexture, m_pMat_WebCam, m_pColors);        //! 웹캠텍스쳐 정보를 매트릭스로

        if (m_bFilp_LeftRight == true)
            Core.flip(m_pMat_WebCam, m_pMat_WebCam, 1);           //! 좌우반전

        if (m_bFilp_UpDown == true)
            Core.flip(m_pMat_WebCam, m_pMat_WebCam, 0);           //! 좌우반전

        Imgproc.resize(m_pMat_WebCam, m_pMat_Resize, m_sSize);
        if (m_pLineDrawPt != null)
        {
            Imgproc.line(m_pMat_WebCam, m_pLineDrawPt[0], m_pLineDrawPt[1], new Scalar(255, 0, 0, 255), 2);             //라인그리기
            Imgproc.line(m_pMat_WebCam, m_pLineDrawPt[1], m_pLineDrawPt[2], new Scalar(255, 0, 0, 255), 2);
            Imgproc.line(m_pMat_WebCam, m_pLineDrawPt[2], m_pLineDrawPt[3], new Scalar(255, 0, 0, 255), 2);
            Imgproc.line(m_pMat_WebCam, m_pLineDrawPt[3], m_pLineDrawPt[0], new Scalar(255, 0, 0, 255), 2);

            m_pMat_Resize = Perspective(m_pLineDrawPt, m_pMat_Resize, true);
        }

        Imgproc.blur(m_pMat_Resize, m_pMat_Gray, new Size((double)m_fBlur, (double)m_fBlur));
        if (m_bFrist == false)
        {
            m_pMat_Gray.copyTo(m_pMat_Gray_Prev);
            m_bFrist = true;
        }

        Core.absdiff(m_pMat_Gray, m_pMat_Gray_Prev, m_pMat_Threshold);       //! 두개의 매트릭스 비교'
        m_pMat_Gray.copyTo(m_pMat_Gray_Prev);

        Imgproc.cvtColor(m_pMat_Threshold, m_pMat_Threshold, Imgproc.COLOR_BGR2GRAY);
        Imgproc.threshold(m_pMat_Threshold, m_pMat_Threshold, (double)m_fThreshold, 255, Imgproc.THRESH_BINARY);
        Imgproc.erode(m_pMat_Threshold, m_pMat_Threshold, new Mat((int)m_nEroad, (int)m_nEroad, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
        Imgproc.dilate(m_pMat_Threshold, m_pMat_Threshold, new Mat((int)m_nDlight, (int)m_nDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);

        m_lContours.Clear();         //! 박스리스트 초기화
        m_pMat_Threshold.copyTo(m_pMat_Contours);
        Mat ele = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
        Imgproc.morphologyEx(m_pMat_Contours, m_pMat_Contours, Imgproc.MORPH_CLOSE, ele);
        Imgproc.findContours(m_pMat_Contours, m_lContours, m_pMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기

        foreach (MatOfPoint i in m_lContours)            //! 탐색된 박스 설정
        {
            OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(i.toArray()));      //! 박스
            Imgproc.rectangle(m_pMat_Threshold, boundRect.tl(), boundRect.br(), new Scalar(255, 255, 255, 255), 2, 8, 0);
        }

        return base.Update_Matrix();
    }

    public override Mat Get_ColorMat()
    {
        if (m_bSensorReady == false)
            return null;

        return m_pMat_WebCam;
    }

    public override Mat Get_ThresoldMat()
    {
        if (m_bSensorReady == false)
            return null;

        return m_pMat_Threshold;
    }

    public List<MatOfPoint> Get_Contours()
    {
        if (m_bSensorReady == false)
            return null;

        return m_lContours;
    }

    public override void SaveSetting()
    {
        var mPoint = GetSavePoint();
        var fbsm = Model.First<FantaBoxSettingModel>();
        fbsm.Save_WebCamSetting(m_fThreshold, m_fBlur,
            m_nEroad, m_nDlight, m_bFilp_UpDown, m_bFilp_LeftRight,
            mPoint);
    }
}

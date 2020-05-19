using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System.Collections.Generic;
using JHchoi.Common;
using JHchoi.Module;
using JHchoi.Constants;
using JHchoi.Models;
/// <summary>
/// OpenCV 관련 관리하는 Mng
/// WebCam이나 Kinect의 영상정보 및 데이터를 편하게 사용할 수 있게됨
/// </summary>

public class OpenCV_Mng : IModule
{
    public E_OPENCV_MOD m_eCurrentMod;

    //   [SerializeField]
    [SerializeField]
    private OpenCV_WebCam m_pWebCam = null;       //! 웹캠 클레스
    [SerializeField]
    private OpenCV_Kinect m_pKinect = null;       //! 키넥트 클래스
    [SerializeField]
    private OpenCV_RealSense2 m_pReal = null;

    //public GameObject m_pCamera = null;                 //! Mini View화면을 띄우기위한 변수들
    //public Renderer m_pQuad_Color = null;           //
    //public Renderer m_pQuad_Threshold = null;           //

    //[SerializeField]
    //Texture2D m_pTexture_Color = null;        //
    //[SerializeField]
    //Texture2D m_pTexture_Threshold = null;        //

    Vector2 m_v2TextureSize = new Vector2(640, 480);

    // Use this for initialization
    protected override void OnLoadStart()
    {
        //m_pTexture_Color = new Texture2D((int)m_v2TextureSize.x, (int)m_v2TextureSize.y, TextureFormat.RGBA32, false);
        //m_pTexture_Threshold = new Texture2D((int)m_v2TextureSize.x, (int)m_v2TextureSize.y, TextureFormat.RGBA32, false);

        //m_pCamera.SetActive(false);

        //m_pQuad_Color.material.mainTexture = m_pTexture_Color;
        //m_pQuad_Threshold.material.mainTexture = m_pTexture_Threshold;

        StartCoroutine(InitSensor());
    }

    protected override void OnUnload()
    {
        if (m_pWebCam != null)
        {
            m_pWebCam.Destroy();
            m_pWebCam = null;
        }

        if (m_pKinect != null)
        {
            m_pKinect.Destroy();
            m_pKinect = null;
        }

        //m_pQuad_Threshold = null;
        //m_pTexture_Threshold = null;
        //m_pCamera = null;
        //m_pQuad_Color = null;           //
        //m_pQuad_Threshold = null;           //
        //m_pTexture_Color = null;        //
        //m_pTexture_Threshold = null;        //
    }

    public IOpenCV GetOpenCVSensor()
    {
        IOpenCV sensor = null;
        switch (m_eCurrentMod)
        {
            case E_OPENCV_MOD.E_KINECT: sensor = m_pKinect; break;
            case E_OPENCV_MOD.E_WEBCAM: sensor = m_pWebCam; break;
            case E_OPENCV_MOD.E_REALSENSE: sensor = m_pReal; break;
        }

        return sensor;
    }

    IEnumerator InitSensor()
    {
        var sm = Model.First<SettingModel>();
        m_eCurrentMod = sm.SersonType;
        //Destroy_Current();
        var sensor = GetOpenCVSensor();
        sensor.Enter(m_v2TextureSize);

        while(true)
        {
            if (sensor.m_bSensorReady)
                break;

            yield return new WaitForSeconds(0.1f);
        }

        SetResourceLoadComplete();
    }

    /// <summary>
    /// 현재 모드 메모리 제거
    /// 초기화 및 다른 모드로 바뀔때 기존 모드 정보를 날려버림
    /// </summary>
    void Destroy_Current()
    {
        var sensor = GetOpenCVSensor();
        sensor.Destroy();
    }

    /// <summary>
    /// m_bUpdate가 True이면 OpenCV연산을 하고 false면 안함
    /// OpenCVMng는 항상 켜져 있기때문에 OpenCV를 사용 안하면 false 시켜줘야함 (예 : 게임 로딩)
    /// </summary>
    /// <param name="bUpdate"></param>
    public void ActiveSensor(bool on)
    {
        if (on)
            StartCoroutine("Update_Matrix");
        else
            StopCoroutine("Update_Matrix");
    }

    /// <summary>
    /// OpenCV 연산 Update
    /// </summary>
    /// <returns></returns>
    IEnumerator Update_Matrix()
    {
        while(true)
        {
            var sensor = GetOpenCVSensor();
            bool update = false;
            if (sensor.m_bSensorReady)
                update = sensor.Update_Matrix();
            
            if (!update)
                Debug.LogError("E_Sensor_Error");

            //! 키보드 P를 누르면 MiniView 화면이 뜸
            if (Input.GetKeyDown(KeyCode.P) == true)
            {
#if UNITY_EDITOR
                Debug.Log("켜기");
#endif
                //m_pCamera.SetActive(!m_pCamera.activeSelf);
            }

            //if (m_pCamera.activeSelf == true)
            //{
            //    if (m_pQuad_Threshold.gameObject.activeInHierarchy)
            //    {
            //        Utils.matToTexture2D(sensor.Get_ThresoldMat(), m_pTexture_Threshold);
            //    }
            //}

            yield return null;
        }
    }

    /// <summary>
    /// 현재 모드의 세팅정보를 XML저장(로컬)
    /// </summary>
    public void SaveSetting()
    {
        var sensor = GetOpenCVSensor();
        sensor.SaveSetting();
    }

    /// <summary>
    /// 감지박스의 Rect를 넣어주면 View포트 좌표로 변환해서 반환
    /// </summary>
    /// <param name="pRect"></param>
    /// <returns></returns>
    public Vector3 GetViewPortPosition(OpenCVForUnity.Rect pRect)
    {
        Vector3 pVePos;
        pVePos.x = (pRect.x + (pRect.width * 0.5f)) / m_v2TextureSize.x * 1.0f;
        pVePos.y = (pRect.y - (pRect.height * 0.5f)) / m_v2TextureSize.y * 1.0f;
        pVePos.z = 0.0f;

        return pVePos;
    }

    public UnityEngine.Rect GetViewPortRect(OpenCVForUnity.Rect pRect)
    {
        UnityEngine.Rect pMerRect = new UnityEngine.Rect(pRect.x / m_v2TextureSize.x * 1.0f,
            pRect.y / m_v2TextureSize.y * 1.0f,
            Mathf.Abs((pRect.x + (pRect.width * 0.5f)) / m_v2TextureSize.x * 1.0f -
                      (pRect.x - (pRect.width * 0.5f)) / m_v2TextureSize.x * 1.0f),
            Mathf.Abs((pRect.y + (pRect.height * 0.5f)) / m_v2TextureSize.y * 1.0f - (pRect.y - (pRect.height * 0.5f)) / m_v2TextureSize.y * 1.0f));

        pMerRect.y = pMerRect.y - pMerRect.height;

        return pMerRect;
    }
}


///// <summary>
///// 웹캠 or 키넥트 중 현재 모드에서 컬러 Mat가져옴
///// </summary>
///// <returns></returns>
//public Mat Get_ColorMat()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM: return m_pWebCam.Get_WebCamTexture();
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_KinectTexture();
//        case E_OPENCV_MOD.E_REALSENSE: return m_pReal.Get_KinectTexture();
//    }

//    return null;
//}

//public Mat Get_ColorMat2()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_KinectTexture2();
//    }

//    return null;
//}

///// <summary>
///// 웹캠 or 키넥트 중 현재 모드에서 이진 Mat가져옴
///// </summary>
///// <returns></returns>
//public Mat Get_ThresoldMat()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM: return m_pWebCam.Get_WebCamThreshold();
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_KinectThreshold();
//        case E_OPENCV_MOD.E_REALSENSE: return m_pReal.Get_KinectThreshold();
//    }

//    return null;
//}

//public Mat Get_ThresoldMat2()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_KinectThreshold2();
//    }

//    return null;
//}

///// <summary>
///// 웹캠 or 키넥트 중 현재 모드에서 현재 감지된 박스 리스트를 가져옴
///// </summary>
///// <returns></returns>
//public List<MatOfPoint> Get_Contours()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM: return m_pWebCam.Get_Contours();
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_Contours();
//        case E_OPENCV_MOD.E_REALSENSE: return m_pReal.Get_Contours();
//        default: return null;
//    }
//}

//public List<MatOfPoint> Get_Contours2()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM: return m_pWebCam.Get_Contours();
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_Contours2();
//    }

//    return null;
//}

///// <summary>
///// 현재 모드의 화면 해상도 가져옴
///// </summary>
///// <returns></returns>
//public Size Get_MatSize()
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM: return m_pWebCam.Get_WebCamSize();
//        case E_OPENCV_MOD.E_KINECT: return m_pKinect.Get_KinectSize();
//        case E_OPENCV_MOD.E_REALSENSE: return m_pReal.Get_RealSize();
//    }

//    return null;
//}

///// <summary>
///// 캠설정 화면에서 빨간점 정보를 웹캠이나 키넥트클래스로 보내서 화면 조정
///// </summary>
///// <param name="pLineDrawPt"></param>
//public void SetPerspective(List<Point> pLineDrawPt)
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_WEBCAM:
//            m_pWebCam.SetLinePoint(pLineDrawPt);
//            break;
//        case E_OPENCV_MOD.E_KINECT:
//            m_pKinect.SetLinePoint(pLineDrawPt);
//            break;
//        case E_OPENCV_MOD.E_REALSENSE:
//            m_pReal.SetLinePoint(pLineDrawPt);
//            break;
//    }
//}

//public void SetPerspective2(List<Point> pLineDrawPt)
//{
//    switch (m_eCurrentMod)
//    {
//        case E_OPENCV_MOD.E_KINECT:
//            //    m_pKinect.SetLinePoint2(pLineDrawPt);
//            break;
//    }
//}

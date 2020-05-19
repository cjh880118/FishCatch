using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Intel.RealSense;
using System.Threading;

public class OpenCV_RealSense : MonoBehaviour
{
    public RsDepthSourceManager mDepthSoureManager; // 뎁스 값을 반환하는 클래스
    public RsColorSourceManager mColorSoureManager; // 컬러값을 반환하는 클래스
    public RsDevice mRsDevice;

    public Texture2D mTex_Color;
    public Texture2D mTex_Depth;

    const int iWidth = 640;
    const int iHeight = 480;
    //int iWidth_Depth;
    //int iHeight_Depth;
    //int iWidth_Color;
    //int iHeight_Color;

    Intel.RealSense.DepthFrame mDepthFrame;

    Mat mMat_Color;
    Mat mMat_Depth;
    Mat mMat_Filter;
    Mat mMat_Contours;
    Mat mMat_Hierarchy;

    ushort[] m_pDepthData_ushort = null;      //! 뎁스값을 저장 할 배열
    Mat m_pPerspective = null;               //! 영역지정화면 만큼 Mat 자르기 용 Mat
    List<MatOfPoint> m_lContours = null;     //! 감지박스 데이터 저장용 리스트

    float fDepthDistanceMin =1.0f;
    float fDepthDistanceMax = 20.0f;

    internal bool m_bFilp_UpDown = false;       //! 위아래 반전
    internal bool m_bFilp_LeftRight = false;       //! 좌우 반전
    internal float m_fBlur = 1.0f;        //! 블러
    internal int m_nEroad = 1;           //! 축소
    internal int m_nDlight = 1;           //! 확장
    internal int m_nFrontNoiseDlight = 10;          //! 뎁스 거리 외 인지된 부분 제거 할 크기 확장

    List<Point> m_pLineDrawPt = null;       //! Perspective 영역 좌표리스트

    bool bColor = true;

    IEnumerator InitRealSense()
    {
        mRsDevice.Enter();

        mTex_Color = null;
        mTex_Depth = null;

        mMat_Color = new Mat(iHeight, iWidth, CvType.CV_8UC4);
        mMat_Depth = new Mat((int)(iHeight), (int)(iWidth), CvType.CV_8UC4);
        mMat_Filter = new Mat((int)(iHeight), (int)(iWidth), CvType.CV_8UC4);
        Imgproc.cvtColor(mMat_Depth, mMat_Depth, Imgproc.COLOR_BGR2GRAY);
        Imgproc.cvtColor(mMat_Filter, mMat_Filter, Imgproc.COLOR_BGR2GRAY);

        mMat_Contours = new Mat();
        m_pPerspective = new Mat();
        mMat_Hierarchy = new Mat();
        m_lContours = new List<MatOfPoint>();

        while (true)
        {
            m_pDepthData_ushort = mDepthSoureManager.GetData();
            if (m_pDepthData_ushort == null && m_pDepthData_ushort[0] == 0)
            {
                yield return null;
                continue;
            }
            break;
        }
        yield return null;
    }


    public void RealSenseStart()
    {
        if (mRsDevice == null)
            mRsDevice = GetComponent<RsDevice>();
        Destroy_Matrix();
        StartCoroutine(InitRealSense());

    }

    public void RealSenseStop()
    {
        if (mRsDevice == null)
            mRsDevice = GetComponent<RsDevice>();

        mRsDevice.Stop();
    }

    private void OnDestroy()
    {
        mDepthSoureManager = null;
        mColorSoureManager = null;
        mRsDevice = null;
        Destroy_Matrix();
    }

    public  bool Update_Matrix()
    {
        Debug.Log("업데이트 리얼센스");

        mTex_Color = mColorSoureManager.texture;
        mTex_Depth = mDepthSoureManager.texture2D_;

        // Color 연산
        Utils.texture2DToMat(mTex_Color, mMat_Color);
        Utils.texture2DToMat(mTex_Depth, mMat_Depth);


        Core.flip(mMat_Color, mMat_Color, 0);         
        Core.flip(mMat_Color, mMat_Color, 1);         
        Core.flip(mMat_Depth, mMat_Depth, 0);         
        Core.flip(mMat_Depth, mMat_Depth, 1);         

        if (m_bFilp_LeftRight == true)
        {
            Core.flip(mMat_Color, mMat_Color, 1);                     
            Core.flip(mMat_Depth, mMat_Depth, 1);           

        }
        if (m_bFilp_UpDown == true)
        {
            Core.flip(mMat_Color, mMat_Color, 0);          
            Core.flip(mMat_Depth, mMat_Depth, 0);          

        }
        if (m_pLineDrawPt != null)
        {
            Imgproc.line(mMat_Color, m_pLineDrawPt[0], m_pLineDrawPt[1], new Scalar(255, 0, 0, 255), 2);             //라인그리기 색결정
            Imgproc.line(mMat_Color, m_pLineDrawPt[1], m_pLineDrawPt[2], new Scalar(255, 0, 0, 255), 2);
            Imgproc.line(mMat_Color, m_pLineDrawPt[2], m_pLineDrawPt[3], new Scalar(255, 0, 0, 255), 2);
            Imgproc.line(mMat_Color, m_pLineDrawPt[3], m_pLineDrawPt[0], new Scalar(255, 0, 0, 255), 2);
        }



        // Depth 매트릭스 받아오는 부분 추가
        //
     

        // mDepthSoureManager.GetData();

        // 뎁스 이미지 연산
        //  if (m_pLineDrawPt != null) mMat_Filter = Perspective(m_pLineDrawPt, mMat_Depth);

        // blur
        //if (m_fBlur % 2 == 0)
        //{
        //    Imgproc.medianBlur(mMat_Filter, mMat_Filter, (int)m_fBlur + 1);
        //}
        //else
        //{
        //    Imgproc.medianBlur(mMat_Filter, mMat_Filter, (int)m_fBlur);
        //}

        //Imgproc.erode(mMat_Filter, mMat_Filter, new Mat((int)m_nEroad, (int)m_nEroad, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);
        //Imgproc.dilate(mMat_Filter, mMat_Filter, new Mat((int)m_nDlight, (int)m_nDlight, CvType.CV_8U, new Scalar(1)), new Point(-1, -1), 3);


        //if (mMat_Hierarchy == null || m_lContours == null) return false;

        //m_lContours.Clear();         //! 박스리스트 초기화s
        //mMat_Filter.copyTo(mMat_Contours);
        //Mat ele2 = new OpenCVForUnity.Mat(3, 3, CvType.CV_8U, new Scalar(1));
        //Imgproc.morphologyEx(mMat_Contours, mMat_Contours, Imgproc.MORPH_CLOSE, ele2);
        //Imgproc.findContours(mMat_Contours, m_lContours, mMat_Hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);    //! 박스찾기

        //foreach (MatOfPoint i in m_lContours)            //! 탐색된 박스 설정
        //{
        //    OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(i.toArray()));      //! 박스
        //    Imgproc.rectangle(mMat_Filter, boundRect.tl(), boundRect.br(), new Scalar(255, 255, 255, 255), 2, 8, 0);
        //}

        return true;
        
    }

    void SafeDispose_Mat(ref Mat pMat)
    {
        if (pMat != null)
        {
            pMat.Dispose();
            pMat = null;
        }
    }
    void Destroy_Matrix()
    {
        SafeDispose_Mat(ref mMat_Color);

        m_pPerspective = null;
        if (m_lContours != null)
        {
            m_lContours.Clear();
            m_lContours = null;
        }
        

        SafeDispose_Mat(ref mMat_Depth);
      
        SafeDispose_Mat(ref mMat_Filter);
        SafeDispose_Mat(ref mMat_Contours);
        SafeDispose_Mat(ref mMat_Hierarchy);
        
        SafeDispose_Mat(ref m_pPerspective);
        
    }





    public void SetLinePoint(List<Point> pLineDrawPt) { m_pLineDrawPt = pLineDrawPt; }


    double top = -1.0f;
    double right = -1.0f;
    double bottom = -1.0f;
    double left = -1.0f;


    /// <summary>
    /// 화면 영역잡은대로 화면 자르기
    /// </summary>
    /// <param name="corners"></param>
    /// <param name="CurrMat"></param>
    /// <returns></returns>
    public Mat Perspective(List<Point> corners, Mat CurrMat)
    {
        if (top == -1.0f)
        {
            top = Mathf.Sqrt(Mathf.Pow((float)corners[0].x - (float)corners[1].x, 2) + Mathf.Pow((float)corners[0].y - (float)corners[1].y, 2));
            right = Mathf.Sqrt(Mathf.Pow((float)corners[1].x - (float)corners[2].x, 2) + Mathf.Pow((float)corners[1].y - (float)corners[2].y, 2));
            bottom = Mathf.Sqrt(Mathf.Pow((float)corners[2].x - (float)corners[3].x, 2) + Mathf.Pow((float)corners[2].y - (float)corners[3].y, 2));
            left = Mathf.Sqrt(Mathf.Pow((float)corners[3].x - (float)corners[1].x, 2) + Mathf.Pow((float)corners[3].y - (float)corners[1].y, 2));

        }

        double max1 = ((float)top >= (float)bottom) ? top : bottom;
        double max2 = ((float)left >= (float)right) ? top : bottom;

        List<Point> result_points = new List<Point>();

        result_points.Add(new Point(0, 0));
        result_points.Add(new Point(max1, 0));
        result_points.Add(new Point(max1, max2));
        result_points.Add(new Point(0, max2));

        Mat cornerPts = Converters.vector_Point2f_to_Mat(corners);
        Mat resultPts = Converters.vector_Point2f_to_Mat(result_points);

        Mat transformation = Imgproc.getPerspectiveTransform(cornerPts, resultPts);

        Imgproc.warpPerspective(CurrMat, m_pPerspective, transformation, new Size(max1, max2));

        Imgproc.resize(m_pPerspective, CurrMat, new OpenCVForUnity.Size(640, 480));
        return CurrMat;
    }

    public Mat GetRealSenseColorMat()
    {
        return mMat_Color;
    }
    public Mat GetRealSenseThrolder()
    {
        return mMat_Depth;
    }



}

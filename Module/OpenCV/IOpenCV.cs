using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

public class OpenCVContoursMsg : Message
{
    public Vector3 TL;
    public Vector3 BR;
    public OpenCVContoursMsg(Vector3 tl, Vector3 br)
    {
        TL = tl;
        BR = br;
    }
}

public class IOpenCV : MonoBehaviour
{
    public bool m_bSensorReady = false;

    protected Mat m_pPerspective = null;               //! 영역지정화면 만큼 Mat 자르기 용 Mat
    protected List<Point> m_pLineDrawPt = null;       //! Perspective 영역 좌표리스트
    protected List<MatOfPoint> m_lContours = new List<MatOfPoint>();
    protected List<MatOfPoint> m_lContours2 = new List<MatOfPoint>();
    protected Size m_sSize;
    protected Vector3 ActiveAreaRect_BR = new Vector3(640, 480);
    protected Vector3 ActiveAreaRect_TL = new Vector3();

    double top = -1.0f;
    double right = 1.0f;
    double bottom = -1.0f;
    double left = -1.0f;

    public virtual void Enter(Vector2 txtSize, bool color = false)
    {
        m_bSensorReady = false;
        m_sSize = new Size(txtSize.x, txtSize.y);
        
    }

    /// <summary>
    /// 현재 모드 메모리 제거
    /// 초기화 및 다른 모드로 바뀔때 기존 모드 정보를 날려버림
    /// </summary>
    public virtual void Destroy()
    {
    }

    /// <summary>
    /// OpenCV 연산 Update
    /// </summary>
    /// <returns></returns>
    public virtual bool Update_Matrix()
    {
        if (0 < m_lContours.Count)
        {
            var rects = new List<UnityEngine.Rect>();
            foreach (var item in m_lContours)
            {
                OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(item.toArray()));
                var width = ActiveAreaRect_BR.x - ActiveAreaRect_TL.x;
                var height = ActiveAreaRect_BR.y - ActiveAreaRect_TL.y;
                Vector3 tl = new Vector3((float)((boundRect.tl().x - ActiveAreaRect_TL.x) / width), (float)((height - boundRect.tl().y + ActiveAreaRect_TL.y) / height), 0);
                Vector3 br = new Vector3((float)((boundRect.br().x - ActiveAreaRect_TL.x) / width), (float)((height - boundRect.br().y + ActiveAreaRect_TL.y) / height), 0);
                var rect = new UnityEngine.Rect();
                rect.min = new Vector2( tl.x , br.y );
                rect.max = new Vector2(br.x, tl.y );
                
                rects.Add(rect);
                //Message.Send<OpenCVContoursMsg>(new OpenCVContoursMsg(tl, br));
            }
            //foreach (var item in rects)
            //{
            //    Debug.LogWarning("============ rect : " + item + "\n--------------------------------------min :  " + item.min + "_max : " + item.max);
            //}
            Message.Send<JHchoi.Contents.Event.TouchRectMsg>(new JHchoi.Contents.Event.TouchRectMsg(rects));
        }

        return true;
    }

    public void SetActiveAreaRect(Vector3 br, Vector3 tl)
    {
        ActiveAreaRect_BR = br;
        ActiveAreaRect_TL = tl;
    }

    public void GetActiveAreaRect(out Vector3 br, out Vector3 tl)
    {
        tl = new Vector3( ActiveAreaRect_TL.x, ActiveAreaRect_TL.y);
        br = new Vector3( ActiveAreaRect_BR.x, ActiveAreaRect_BR.y);
    }

    ///// <summary>
    ///// 웹캠 or 키넥트 중 현재 모드에서 컬러 Mat가져옴
    ///// </summary>
    ///// <returns></returns>
    public virtual Mat Get_ColorMat()
    {
        return null;
    }

    public virtual Mat Get_ColorMat2()
    {
        return null;
    }

    ///// <summary>
    ///// 웹캠 or 키넥트 중 현재 모드에서 이진 Mat가져옴
    ///// </summary>
    ///// <returns></returns>
    public virtual Mat Get_ThresoldMat()
    {
        return null;
    }

    public virtual Mat Get_ThresoldMat2()
    {
        return null;
    }

    ///// <summary>
    ///// 캠설정 화면에서 빨간점 정보를 웹캠이나 키넥트클래스로 보내서 화면 조정
    ///// </summary>
    ///// <param name="pLineDrawPt"></param>
    public void SetLinePoint(List<Point> pLineDrawPt)
    {
        m_pLineDrawPt = pLineDrawPt;
    }

    public virtual void SaveSetting()
    {
    }

    protected FantaSenserInfo.MPoint[] GetSavePoint()
    {
        FantaSenserInfo.MPoint[] mPoint = new FantaSenserInfo.MPoint[4];
        for (int i = 0; i < 4; i++)
            mPoint[i] = new FantaSenserInfo.MPoint((float)m_pLineDrawPt[i].x, (float)m_pLineDrawPt[i].y);

        return mPoint;
    }

    /// <summary>
    /// 화면 영역잡은대로 화면 자르기
    /// </summary>
    /// <param name="corners"></param>
    /// <param name="CurrMat"></param>
    /// <returns></returns>
    protected Mat Perspective(List<Point> corners, Mat CurrMat, bool color = true)
    {
        //  Debug.Log("DD:" + corners.Count);
        if (color == true || top == -1.0f)
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
        Imgproc.resize(m_pPerspective, CurrMat, m_sSize);

        return CurrMat;
    }

    public Vector2 GetSensorSize()
    {
        return new Vector2((int)m_sSize.width, (int)m_sSize.height);
    }
}

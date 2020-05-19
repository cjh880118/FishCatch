using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using JHchoi.Models;
using JHchoi.Contents.Event;

public class GamePictureBubble : MonoBehaviour
{
    public bool m_bClear = false;

    float m_fNoBlockCheckTime = 0.0f;

    Touch tempTouch;
    Vector2 touchPos;

    public List<GameObject> BubbleList = new List<GameObject>();
    public List<GameObject> BubbleList1 = new List<GameObject>();
    public bool isCheck = false;

    public int pictureNum;
    public GameObject ClearFx;

    public void Enter()
    {
        m_bClear = false;
        ClearFx.SetActive(false);

        //if (isCheck)
        {
            //Message.AddListener<CellBig.Contents.Event.PictureBubbleCreateMsg>(Create);
            StartCoroutine(PlayBubble());
        }

        
    }

    public void Destroy()
    {
        StopAllCoroutines();
        //Message.RemoveListener<CellBig.Contents.Event.PictureBubbleCreateMsg>(Create);
    }

    IEnumerator PlayBubble()
    {
        while (true)
        {
            #region // opencv
            // 옮기기
            //if (OpenCV_Mng.I != null)
            //{
            //    List<MatOfPoint> m_pContours = OpenCV_Mng.I.Get_Contours();
            //    if (m_pContours == null) return false;
            //    for (int i = 0; i < m_pContours.Count; i++)
            //    {
            //        OpenCVForUnity.Rect boundRect = Imgproc.boundingRect(new MatOfPoint(m_pContours[i].toArray()));
            //        boundRect.y = 480 - boundRect.y;

            //        TouchObject(OpenCV_Mng.I.GetViewPortRect(boundRect));
            //    }
            //}
            #endregion

            yield return null;
        }
    }

    public void Next()
    {
        StartCoroutine(Cor_Next());
    }

    IEnumerator Cor_Next()
    {
        //m_bClear = true;
        // 옮기기
        //Scene_Game_Picture.I.m_pGame.m_pParticleMng.Create("Clear");
        FxCreate(ClearFx);
        JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Picture_Clear);
        Message.Send<PictureBubbleTagIn>(new PictureBubbleTagIn());
        yield return new WaitForSeconds(4.0f);
        Message.Send<PictureBubbleTagOut>(new PictureBubbleTagOut());
        //  Scene_Game.I.m_pGameRoot.ScoreUp_Bonus(5000);

        yield return new WaitForSeconds(1.0f);
        BubbleCount = 0;
        //m_bClear = false;
        Message.Send<PictureBubbleCreate>(new PictureBubbleCreate());
        //yield return new WaitForSeconds(1.0f);
       
        
        yield return null;
    }

    void FxCreate(GameObject fx)
    {
        GameObject createObj;
        Vector3 pos;
        createObj = Instantiate(fx);
        createObj.SetActive(false);
        createObj.transform.SetParent(this.transform.parent);
        pos = new Vector3(5.55f, 3.69f, 0);
        createObj.SetActive(true);
        createObj.transform.position = pos;
        DestroyFx(createObj);
    }

    void DestroyFx(GameObject fx)
    {
        Destroy(fx, 5f);
    }

    int BubbleCount = 0;
    public void Create(Vector3 pos, int nX, int nY, bool bBoom)
    {
        if (BubbleList1.Count < BubbleList.Count)
        {
            BubbleList[BubbleCount].SetActive(true);
            BubbleList[BubbleCount].GetComponent<GamePictureBubbleObj>().Active(pos, bBoom);
            BubbleList[BubbleCount].GetComponent<GamePictureBubbleObj>().SetMng(this);
            BubbleList[BubbleCount].GetComponent<GamePictureBubbleObj>().SetXY(nX, nY);

            BubbleList1.Add(BubbleList[BubbleCount]);
            BubbleCount++;
        }

        Message.Send<CurrentPictureNumMsg>(new CurrentPictureNumMsg(pictureNum));
    }

    public bool GetObjectNoneCheck()
    {
        for (int i = 0; i < BubbleList.Count; i++)
        {
            if (BubbleList[i].activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public int GetCurrBlockNum()
    {
        return BubbleList.Count;
    }

    public int GetCurrBubble1List()
    {
        return BubbleList1.Count;
    }

    public void Freeze(int nX, int nY)
    {
        for (int i = 0; i < BubbleList.Count; i++)
        {
            GamePictureBubbleObj pSrc = BubbleList[i].GetComponent<GamePictureBubbleObj>();
            if (pSrc == null || pSrc.bActive == false || pSrc.m_bLife == false || pSrc.m_pCollider.enabled == false) continue;
            if (Mathf.Abs(nX - pSrc.m_nX) <= 1 && Mathf.Abs(nY - pSrc.m_nY) <= 1)
            {
                pSrc.Freeze();
            }
        }
    }

    public void AllDie()
    {
        for (int i = 0; i < BubbleList.Count; i++)
        {
            GamePictureBubbleObj pSrc = BubbleList[i].GetComponent<GamePictureBubbleObj>();
            if (pSrc == null || pSrc.bActive == false || pSrc.m_bLife == false)
            {
                Debug.LogError(pSrc.bActive + " " + pSrc.m_bLife);
                return;
            }
            pSrc.Die();
        }
    }
}

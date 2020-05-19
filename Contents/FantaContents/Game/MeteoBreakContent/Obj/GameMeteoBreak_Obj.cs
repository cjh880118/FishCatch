using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameMeteoBreak_Obj : MonoBehaviour
{
    public Collider colObj = null; //충돌체
    public GameObject m_pObj1 = null; //오브젝트
    public GameObject m_pObj2 = null; //이펙트 //없을 수도 있다.

    public ScoreTextControl scoreTextControl;

    Coroutine mCor_Life = null;
    Coroutine mCor_Update = null;

    internal int m_nObjType = 0;
    internal float m_fMoveTime = 0.0f;
    internal int m_nMoveType = 1;
    internal float m_fIntervalTime = 1.2f;
    internal float m_fIntervalTimeHalf = 0.6f;
    internal int m_nDirection = 1;

    int score = 100;

    private void OnEnable()
    {
        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    public void Active(Vector3 pPos, int nObjType)
    {
        colObj.enabled = true;

        m_pObj1.SetActive(true);
         
        if (m_pObj2 != null)
            m_pObj1.SetActive(true);

        m_nDirection = Random.Range(0, 100) > 50 ? -1 : 1;
        //m_nDirection = -1;
        if (m_nDirection == -1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);

            if (m_pObj2 != null)
                m_pObj2.transform.rotation = Quaternion.Euler(0, 180, 0);

            transform.position = new Vector3(20.0f, pPos.y, pPos.z);
        }
        else
            transform.position = pPos;

        m_nObjType = nObjType;
        m_fMoveTime = 0.0f;

        mCor_Update = StartCoroutine(Cor_Update());
        mCor_Life = StartCoroutine(Cor_Life());
    }

    public void Hit()
    {
        colObj.enabled = false;

        m_pObj1.transform.gameObject.SetActive(false);

        if (m_pObj2 != null)
            m_pObj2.transform.gameObject.SetActive(false);

        StopCoroutine(mCor_Update);
        StopCoroutine(mCor_Life);

        StartCoroutine(HitEvent());

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.MeteoBreak_Explosion);

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

    }

    IEnumerator HitEvent()
    {
        yield return new WaitForSeconds(1.0f);

        Deactive();
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            switch (m_nObjType)
            {
                case 0://좌에서 우로 진행되는 우주선( 진자 운동을 한다.)
                case 1://좌에서 우로 진행되는 우주선( 진자 운동을 한다.)
                case 2://좌에서 우로 진행되는 우주선( 진자 운동을 한다.)
                case 3://좌에서 우로 진행되는 우주선( 진자 운동을 한다.)
                case 4:
                case 5:
                case 6:
                    {
                        //transform.position += (Vector3.right * +15.0f) * Time.deltaTime;
                        m_fMoveTime += Time.deltaTime;

                        if (m_fMoveTime > m_fIntervalTime)
                        {
                            m_fMoveTime = 0;
                            m_nMoveType *= -1;
                        }

                        transform.Translate(
                            Time.deltaTime * 3.0f,
                            Time.deltaTime * 2.0f * (m_fMoveTime < m_fIntervalTimeHalf ? m_fMoveTime : m_fIntervalTime - m_fMoveTime) * m_nMoveType,
                            0);
                    }
                    break;
                default:
                    {
                        transform.position += (m_nDirection == 1 ? Vector3.right : Vector3.left) * 15.0f * Time.deltaTime;
                        transform.Rotate(Vector3.right * 10.0f * Time.deltaTime);
                    }
                    break;
            }

            if (transform.position.x > 22.0f || transform.position.x < -22.0f)
                Deactive();

            yield return null;
        }
    }

    IEnumerator Cor_Life()
    {
        yield return new WaitForSeconds(0.5f);
        colObj.enabled = true;
    }

    void OnPoolObjectMsg(PoolObjectMsg msg)
    {
        Deactive();
    }

    public void Deactive()
    {
        StopAllCoroutines();
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(m_nObjType, this.transform.gameObject));
    }
}

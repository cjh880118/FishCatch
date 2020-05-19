using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFireFighterTruckCam : MonoBehaviour
{
    GameObject m_pFollowObj = null;

    float fStartTime;
    float fZoomTime;
    float fDuration;

    bool m_bActive = false;

    public void Enter()
    {
        m_pFollowObj = GameObject.Find("FireTruck01");
        if (m_pFollowObj == null) return;

        m_bActive = true;
        fStartTime = Time.time;
        fZoomTime = 0.0f;
        fDuration = 10.0f;
        bZoom = false;
    }

    public void Destroy()
    {
        m_pFollowObj = null;
    }

    public void DeActive()
    {
        m_bActive = false;

        fStartTime = Time.time;
        StartCoroutine("Cor_Start");
    }

    IEnumerator Cor_Start()
    {
        yield return new WaitForSeconds(1.0f);
        Enter();
        yield return new WaitForSeconds(2.0f);
        StartCoroutine("Cor_Zoom");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bActive == false)
        {
            float fT2 = (Time.time - fStartTime) / 0.6f;
            Vector3 pPos2 = transform.position;
            pPos2.z = Mathf.SmoothStep(pPos2.z, 60.0f, fT2);
            transform.position = pPos2;
            return;
        }
        //    if (Scene_Game.I.m_pClear.gameObject.activeSelf == true) return;

        float fT = (Time.time - fStartTime) / fDuration;
        Vector3 pPos = transform.position;
        pPos.x = Mathf.SmoothStep(pPos.x, m_pFollowObj.transform.position.x - 197.0f, fT);

        transform.position = pPos;
    }

    bool bZoom = false;
    IEnumerator Cor_Zoom()
    {

        bZoom = true;
        fZoomTime = Time.time;

        Vector3 pPos2 = transform.position;
        for (float i = 0.0f; i < 4.0f; i += Time.deltaTime)
        {
            float fT2 = (Time.time - fZoomTime) / 0.6f;
            pPos2 = transform.position;
            pPos2.z = Mathf.SmoothStep(pPos2.z, -1.53f, fT2);
            transform.position = pPos2;
            if (Mathf.Abs(pPos2.z - -0.53f) < 0.1f)
            {
                pPos2.z = -0.53f;
                transform.position = pPos2;
                break;
            }
            yield return null;
        }
        pPos2.z = -0.53f;
        transform.position = pPos2;
        bZoom = false;

        yield return null;
    }
}

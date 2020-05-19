using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.Contents.Event;

using DG.Tweening;

public class GameSpaceJump_ObjectControl : MonoBehaviour
{
    float m_fScreen_Left = 0.0f;
    float m_fScreen_Right = 0.0f;
    float m_fScreen_Top = 0.0f;
    float m_fScreen_Bottom = 0.0f;

    float m_fEdgeDis = 3.0f;
    float m_fAirBorneDis = 7.0f;

    public Transform universeCamera;
    public Camera spawnCamera;

    public Animator earthAnim;

    public float startPointX = 1.15f;
    public float restartPointX = 1.65f;
    public float betweenXGap = 0.7f;

    public float currentPointX;

    public float waitTime;

    Vector3 startCameraPos;
    Vector3 restartCameraPos;

    public bool isReady = false;
    public bool isMove = false;

    Vector3 spawnAirBornePos;

    Coroutine waitCor;

    private void Awake()
    {
        startCameraPos = new Vector3(startPointX, universeCamera.localPosition.y, universeCamera.localPosition.z);
        restartCameraPos = new Vector3(restartPointX, universeCamera.localPosition.y, universeCamera.localPosition.z);

        SetSpawnCamera();
    }


    private void OnEnable()
    {
        StopAllSpaceSound();

        SetStartCameraPos();
        SetEarthAnim();
    }

    void SetStartCameraPos()
    {
        universeCamera.localPosition = startCameraPos;
    }

    void SetEarthAnim()
    {
        earthAnim.Rebind();
        earthAnim.SetTrigger("Move");
    }

    void SetSpawnCamera()
    {
        m_fScreen_Left = 87f;
        m_fScreen_Top = 7f;
        m_fScreen_Right = 113.6f;
        m_fScreen_Bottom = -7f;
    }

    public void GameStart()
    {
        isReady = true;
        currentPointX = universeCamera.localPosition.x;
        spawnAirBornePos = new Vector3(10000f, 10000f, 10000f);
        StopAllSpaceSound();

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.SpaceJump_Chatter);
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.SpaceJump_Wind);
    }

    public IEnumerator SetSpawn(GameSpaceJump_AirBorne airBorne)
    {
        yield return StartCoroutine(SetSpawnPoint(airBorne));
    }

    IEnumerator SetSpawnPoint(GameSpaceJump_AirBorne airBorne)
    {
        Vector3 tempPos = Vector3.zero;
        bool isPass = false;

        while (!isPass)
        {
            tempPos = new Vector3(
                                Random.Range(m_fScreen_Left + m_fEdgeDis, m_fScreen_Right - m_fEdgeDis),
                                0,
                                Random.Range(m_fScreen_Top - m_fEdgeDis, m_fScreen_Bottom + m_fEdgeDis));

            if (Vector3.Distance(tempPos, spawnAirBornePos) >= m_fAirBorneDis)
                isPass = true;

            yield return null;
        }

        airBorne.SetSpawn(tempPos);
        spawnAirBornePos = tempPos;

        yield return null;
    }

    
    public void MoveUp()
    {
        if (currentPointX < 2.0f)
            currentPointX += betweenXGap * 0.2f;
        else
        {
            currentPointX += betweenXGap;
            StopAllSpaceSound();
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.SpaceJump_Space);
        }

        universeCamera.DOMoveX(currentPointX, 0.2f);

        if (waitCor != null)
            StopCoroutine(waitCor);

        if(currentPointX > 30.0f)
            StartCoroutine(Cor_Down());
        else if (currentPointX > 3.3f)
            waitCor = StartCoroutine(WaitTime());
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(Cor_Down());
    }

    IEnumerator Cor_Down()
    {
        isReady = false;

        Message.Send<PoolObjectMsg>(new PoolObjectMsg());
        Message.Send<AirBorneInitMsg>(new AirBorneInitMsg());

        universeCamera.DOMove(restartCameraPos, 3.0f);
        
        yield return new WaitForSeconds(3.0f);

        StopAllSpaceSound();

        GameStart();
    }

    public void StopAllSpaceSound()
    {
        SoundManager.Instance.StopSound((int)SoundType_GameFX.SpaceJump_Chatter);
        SoundManager.Instance.StopSound((int)SoundType_GameFX.SpaceJump_Wind);
        SoundManager.Instance.StopSound((int)SoundType_GameFX.SpaceJump_Space);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;

public class GameRoadDestruction_ObjectControl : MonoBehaviour
{
    public FracturedObject fracturedObject;

    float maxPercent = 100f;
    public float resetPercent = 50.0f;
    public float currentPercent;

    public bool isReady = false;
    bool isReset = false;

    Coroutine timer_Cor = null;
    Coroutine percent_Cor = null;

    public void GameStart()
    {
        isReady = true;

        GetPercentage();

        if (timer_Cor != null)
            StopCoroutine(timer_Cor);

        timer_Cor = null;
        timer_Cor = StartCoroutine(Cor_Timer());

        percent_Cor = StartCoroutine(Cor_Percent());
    }

    public void ResetGame()
    {
        isReady = false;
        StopCoroutine(timer_Cor);
        StopCoroutine(percent_Cor);
    }

    public void GetPercentage()
    {
        currentPercent = GetLivePercent();
    }

    IEnumerator Cor_Timer()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (isReady == false)
                yield break;

            yield return new WaitForSeconds(5.0f);

            if (!isReset)
                StartCoroutine(Cor_Reset());

            yield return null;
        }
    }

    IEnumerator Cor_Percent()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (isReady == false)
                yield break;

            if (currentPercent < resetPercent)
            {
                if (!isReset)
                    StartCoroutine(Cor_Reset());
            }
            yield return null;
        }
    }

    IEnumerator Cor_Reset()
    {
        GetPercentage();

        if (currentPercent == maxPercent)
            yield break;

        isReady = false;

        fracturedObject.ResetAnimation(); // 되돌아가는 애니매이션

        SoundManager.Instance.PauseSound((int)SoundType_GameBGM.RoadDestruction);
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.RoadDestruction_TapleRe);

        while (true)
        {
            if (fracturedObject.IsReturning() == false) break;
            yield return new WaitForFixedUpdate();
        }
        fracturedObject.ResetChunks();  // 되돌리기

        SoundManager.Instance.PausePlaySound((int)SoundType_GameBGM.RoadDestruction);
        SoundManager.Instance.StopSound((int)SoundType_GameFX.RoadDestruction_TapleRe);

        yield return new WaitForSeconds(0.1f);

        GameStart();

        yield return null;
    }

    float GetLivePercent()
    {
        float nLiveNum = (float)GetCurrLiveNum();
        float nMaxNum = (float)GetMaxNum();
        float a = nLiveNum / nMaxNum * 100.0f;
        return a;
    }

    int GetCurrLiveNum()
    {
        return fracturedObject.GetLiveNum();
    }

    int GetMaxNum()
    {
        return fracturedObject.GenerateNumChunks;
    }
}

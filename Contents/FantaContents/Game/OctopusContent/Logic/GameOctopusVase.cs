using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

public class GameOctopusVase : MonoBehaviour
{
    [Header("Object SpawnTime")]
    public AnimationCurve mCurveSpawnTime = null;

    public Camera mCamera = null;
    float mCreateCurrTime = 0.0f;

    public float CurrGameTime;
    public float MaxGameTime;

    public List<GameObject> VaseList = new List<GameObject>();
    public int VaseCount = 0;

    public void Enter()
    {
        SetupList();
        StartCoroutine(PlayVaseContent());
    }

    public void Destroy()
    {
        VaseCount = 0;
        for (int i = 0; i < VaseList.Count; i++)
        {
            //VaseList[i].GetComponent<GameOctopusVaseObj>().Destroy();
            VaseList[i].SetActive(false);
        }

        mCreateCurrTime = 0;
        StopCoroutine(PlayVaseContent());
    }

    void SetupList()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            VaseList.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    IEnumerator PlayVaseContent()
    {
        while(true)
        {
            mCreateCurrTime += Time.deltaTime;

            if (mCreateCurrTime > GetSpawnTimeInfo(mCurveSpawnTime, CurrGameTime, MaxGameTime)+1)
            {
                CreateObject();
                mCreateCurrTime = 0.0f;
            }
            yield return null;
        }
    }

    void CreateObject()
    {
        if (VaseCount < VaseList.Count)
        {
            VaseList[VaseCount].SetActive(true);
            VaseList[VaseCount].GetComponent<GameOctopusVaseObj>().Active();
            VaseCount++;
        }

    }

    float GetSpawnTimeInfo(AnimationCurve pCurve, float fCurrTime, float fMaxTime)
    {
        float fPercent = 1.0f - (fCurrTime / fMaxTime) * 1.0f;
        return pCurve.Evaluate(fPercent);
    }
}

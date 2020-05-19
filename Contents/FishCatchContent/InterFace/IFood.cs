using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.UI.Event;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using JHchoi;
using JHchoi.Constants;
using JHchoi.Constants.FishCatch;

public abstract class IFood : MonoBehaviour
{
    public GameObject sturnEffect;
    public float resetSpeed = 3;
    //public Rigidbody rigidbody;
    public MeshRenderer[] meshRenderer;
    protected int index;
    protected float viewPosZ;
    protected FoodType foodType;
    protected float catchDelay;

    public bool isCatchInput;
    public bool isCapturePossible;
    public bool isMissObj;
    protected float timeSpan = 0f;

    protected Coroutine corCatch;
    protected Coroutine corRectDelay;
    private int plateNum;
    DateTime firstTime;
    DateTime nowTime;
    Transform parentTransform;

    public virtual void InitFood(int index, FoodType foodType, float catchDelay, float viewPosZ, Transform parentTransform)
    {
        this.index = index;
        this.foodType = foodType;
        this.catchDelay = catchDelay;
        this.viewPosZ = viewPosZ;
        this.isCapturePossible = true;
        isMissObj = false;
        isCatchInput = false;
        if (sturnEffect != null)
            sturnEffect.SetActive(false);

        this.parentTransform = parentTransform;
        this.gameObject.transform.parent = parentTransform;

        foreach (var o in meshRenderer)
            o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }


    public virtual void Catch()
    {
        if (!isCapturePossible)
            return;

        if (timeSpan <= 0)
        {
            firstTime = DateTime.Now.AddSeconds(catchDelay);
        }

        nowTime = DateTime.Now;

        Log.Instance.log(this.gameObject.name + " : 잡히는 중 : " + DateTime.Now.ToString());
        CoroutineCheckStop(corCatch);
        timeSpan += Time.deltaTime;
        corCatch = StartCoroutine(CatchDelay());
        Debug.Log("잡는중");

        if (firstTime <= nowTime) //(timeSpan > catchDelay)
        {
            Log.Instance.log("잡힌 시간 : " + DateTime.Now.ToString());
            Debug.Log("잡앗다");
            timeSpan = 0;
            isCapturePossible = false;
            this.gameObject.transform.parent = parentTransform;
            this.gameObject.transform.localScale = Vector3.one;
            Vector3 vecPosition = this.gameObject.transform.position;
            Vector3 viewPosition = Camera.main.WorldToViewportPoint(vecPosition);
            Vector3 newWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(viewPosition.x, viewPosition.y, viewPosZ));
            this.gameObject.transform.position = newWorldPos;
            StopCoroutine(corCatch);
            corCatch = null;

            if (sturnEffect != null)
                sturnEffect.SetActive(true);

            CatchSoundPlay();
            Vector3 m_Center = this.gameObject.transform.position;
            //if (rigidbody != null)
            //{
            //    rigidbody.useGravity = false;
            //    rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            //}
            foreach (var o in meshRenderer)
                o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            //if (this.gameObject.GetComponent<BoxCollider>() != null)
            //    this.gameObject.GetComponent<BoxCollider>().enabled = false;

            Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Catch, m_Center));
            Message.Send<CatchFoodPlateNumMsg>(new CatchFoodPlateNumMsg(plateNum));
        }
    }

    protected abstract void CatchSoundPlay();

    protected IEnumerator CatchDelay()
    {
        while (DateTime.Now < DateTime.Now.AddSeconds(1.0f))
        {
            yield return null;
        }
        Debug.Log("리셋");
        timeSpan = 0;
    }

    public void RectInputDelay(DateTime time)
    {
        CoroutineCheckStop(corRectDelay);

        if (this.gameObject.activeSelf)
            corRectDelay = StartCoroutine(RectInputTimeCheck(time));
    }

    IEnumerator RectInputTimeCheck(DateTime time)
    {
        while (DateTime.Now < time.AddSeconds(0.5f))
        {
            yield return null;
        }

        MissionFood();
    }

    public void MissionFood()
    {
        Vector3 vecPosition = this.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(vecPosition.x, vecPosition.y, vecPosition.z);
        Debug.Log("놓쳤다");

        if (sturnEffect != null)
            sturnEffect.SetActive(false);

        isMissObj = true;

//        Vector3 vec3 = this.gameObject.transform.position - new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 1, this.gameObject.transform.position.z);

        this.gameObject.transform.DOMove(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 8, this.gameObject.transform.position.z), 1);
        //if (rigidbody != null)
        //{
        //    rigidbody.useGravity = true;
        //    rigidbody.constraints = RigidbodyConstraints.None;
        //    rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //}
        //if (this.gameObject.GetComponent<BoxCollider>() != null)
        //    this.gameObject.GetComponent<BoxCollider>().enabled = true;

        StartCoroutine(ActiveFalseObject());
    }

    IEnumerator ActiveFalseObject()
    {
        yield return new WaitForSeconds(1.0f);
        Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Miss, this.gameObject.transform.position));
        Message.Send<MissFoodMsg>(new MissFoodMsg(index));
        //if (rigidbody != null)
        //    rigidbody.useGravity = false;

        //if (this.gameObject.GetComponent<BoxCollider>() != null)
        //    this.gameObject.GetComponent<BoxCollider>().enabled = false;

        this.gameObject.SetActive(false);
    }

    protected void CoroutineCheckStop(Coroutine cor)
    {
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    public bool GetIsCatchInput()
    {
        return isCatchInput;
    }

    public void SetIsCatchInput(bool isCatchInput)
    {
        this.isCatchInput = isCatchInput;
    }

    public int GetFoodIndex()
    {
        return index;
    }

    public FoodType GetFoodType()
    {
        return foodType;
    }

    public bool GetIsCatch()
    {
        return !isCapturePossible;
    }

    public void SetPlateNum(int plateNum)
    {
        this.plateNum = plateNum;
    }

    public int GetPlateNum()
    {
        return plateNum;
    }

    public void StopRectDelayCheck()
    {
        CoroutineCheckStop(corRectDelay);
    }

    public void ResetFood()
    {
        this.isCapturePossible = true;
        isMissObj = false;
        isCatchInput = false;
        //if (rigidbody != null)
        //{
        //    rigidbody.useGravity = true;
        //    rigidbody.constraints = RigidbodyConstraints.None;
        //    rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //}
        //if (this.gameObject.GetComponent<BoxCollider>() != null)
        //    this.gameObject.GetComponent<BoxCollider>().enabled = true;

        Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Respawn, this.gameObject.transform.position));
        foreach (var o in meshRenderer)
            o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        CoroutineCheckStop(corRectDelay);
    }
}

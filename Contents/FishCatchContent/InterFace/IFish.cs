using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using CellBig;
using CellBig.Constants;
using CellBig.Constants.FishCatch;

public abstract class IFish : MonoBehaviour
{
    //public GameObject testmat;
    public GameObject sturnEffect;
    public float resetSpeed = 3;
    public SkinnedMeshRenderer[] meshRenderer;
    protected int index;
    protected float maxSize;
    protected float minSize;
    protected float maxSpeed;
    protected float minSpeed;
    protected float viewPosZ;
    protected FishType fishType;
    protected float catchDelay;

    protected bool isTargetPossible;
    protected bool isCatchInput;    //잡아다 들어왔니?
    public bool isCapturePossible;
    public float rayDistance = 100;
    public Animator ani;
    protected float timeSpan = 0f;
    DateTime firstTime;
    DateTime nowTime;


    protected Coroutine corCatch;
    protected Coroutine corRectDelay;

    /// <summary>
    /// 물고기 초기화 사이즈 속도
    /// </summary>
    public virtual void InitFish(int index, float maxSize, float minSize, float maxSpeed, float minSpeed, float catchDelay, float viewPosZ)
    {
        this.index = index;
        this.maxSize = maxSize;
        this.minSize = minSize;
        float rndSize = Random.Range(minSize, maxSize);
        this.gameObject.transform.DOScale(new Vector3(rndSize, rndSize, rndSize), 3f);
        this.maxSpeed = maxSpeed;
        this.minSpeed = minSpeed;
        this.catchDelay = catchDelay;
        this.viewPosZ = viewPosZ;
        this.isCapturePossible = true;
        isCatchInput = false;
        isTargetPossible = true;
        if (sturnEffect != null)
            sturnEffect.SetActive(false);

        foreach (var o in meshRenderer)
            o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    /// <summary>
    // 물고기 이동 목표물 정해주기
    /// <summary>
    public virtual IEnumerator MoveTarget(Vector3 target, int targetIndex)
    {
        //int layerMask = 1 << 20;
        while (Vector3.Distance(this.gameObject.transform.position, target) > 1f && isCapturePossible)
        {
            yield return null;
            Vector3 dir = target - this.gameObject.transform.position;
            float rndSpeed = Random.Range(minSpeed, maxSpeed);
            if (!isTargetPossible)
                rndSpeed = resetSpeed;

            float rotateTime = Time.deltaTime * 5;
            float moveTime = Time.deltaTime * 0.3f * rndSpeed;

            this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, Quaternion.LookRotation(dir), rotateTime);
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, target, moveTime);
        }

        if (isCapturePossible)
            Message.Send<FishArriveMsg>(new FishArriveMsg(fishType, index, targetIndex));
    }

    public virtual void Catch()
    {
        if (!isCapturePossible)
            return;

        if (!isTargetPossible)
            return;

        CoroutineCheckStop(corCatch);
        //int a = CatchPossibleCheck();
        if (!CatchPossibleCheck())
        {
            Debug.Log("못잡어");
            return;
        }


        if (timeSpan <= 0)
        {
            firstTime = DateTime.Now.AddSeconds(catchDelay);
        }

        nowTime = DateTime.Now;

        timeSpan += Time.deltaTime;
        corCatch = StartCoroutine(CatchDelay());
        Debug.Log("잡는중");

        if (firstTime <= nowTime)
        {
            Debug.Log("잡앗다");
            if (ani != null)
                ani.SetTrigger("Catch");
            timeSpan = 0;
            isCapturePossible = false;
            Vector3 vecPosition = this.gameObject.transform.position;
            Vector3 viewPosition = Camera.main.WorldToViewportPoint(vecPosition);
            Vector3 newWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(viewPosition.x, viewPosition.y, viewPosZ));
            this.gameObject.transform.position = newWorldPos;
            StopCoroutine(corCatch);
            corCatch = null;

            if (sturnEffect != null)
                sturnEffect.SetActive(true);

            foreach (var o in meshRenderer)
                o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            CatchSoundPlay();
            Vector3 m_Center;
            if (this.gameObject.transform.GetComponent<BoxCollider>() != null)
            {
                Collider m_Collider = this.gameObject.transform.GetComponent<BoxCollider>();
                m_Center = m_Collider.bounds.center;
            }
            else
                m_Center = this.gameObject.transform.position;
            Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Catch, m_Center));
        }
    }

    protected abstract void CatchSoundPlay();

    protected bool CatchPossibleCheck()
    {
        int layerMask = 1 << 22;
        RaycastHit hit;
        BoxCollider boxCollider = this.gameObject.GetComponent<BoxCollider>();
        Debug.DrawRay(this.gameObject.transform.position, Vector3.up * rayDistance, Color.red);
        if (Physics.Raycast(this.gameObject.transform.position, Vector3.up, out hit, Mathf.Infinity, layerMask))
            return false;
        else
            return true;

    }

    protected IEnumerator CatchDelay()
    {
        while (DateTime.Now < DateTime.Now.AddSeconds(1.0f))
        {
            yield return null;
        }
        Debug.Log("리셋");
        timeSpan = 0;
    }

    public bool GetIsCatch()
    {
        return !isCapturePossible;
    }

    public virtual void Respawn(Vector3 position)
    {
        Debug.Log("리스폰");
        isCapturePossible = true;
        this.gameObject.transform.position = position;
        Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Respawn, position));
        timeSpan = 0;
        CoroutineCheckStop(corRectDelay);
    }

    public virtual void CatchPlate()
    {
        CoroutineCheckStop(corRectDelay);
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

        MissingFish();
    }

    public void StopInputDelay()
    {
        CoroutineCheckStop(corRectDelay);
    }

    public void MissingFish()
    {
        Vector3 vecPosition = this.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(vecPosition.x, vecPosition.y - 15, vecPosition.z);
        Debug.Log("놓쳤다");
        if (sturnEffect != null)
            sturnEffect.SetActive(false);
        if (ani != null)
            ani.SetTrigger("Miss");
        isTargetPossible = false;
        isCapturePossible = true;
        Message.Send<MissFishMsg>(new MissFishMsg(fishType, index, vecPosition));
        StartCoroutine(CatchPossbileDelay());
        StartCoroutine(LayerCheck(true));
    }

    public IEnumerator LayerCheck(bool isMiss)
    {
        int layerMask = (1 << 4) | (1 << 21) | (1 << 24);
        RaycastHit raycastHit;
        while (true)
        {
            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward * 1, Color.red);
            Debug.DrawRay(this.gameObject.transform.position, -this.gameObject.transform.up * 1, Color.red);
            if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out raycastHit, 1, layerMask)
                || Physics.Raycast(this.gameObject.transform.position, -this.gameObject.transform.up, out raycastHit, 1, layerMask)
                )
            {
                //Debug.Log(layerMask.ToString());
                LayerCheckSound(raycastHit.collider.gameObject.layer);
                if (isMiss)
                    Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.Miss, this.gameObject.transform.position));
                else
                    Message.Send<PlayEffectMsg>(new PlayEffectMsg(FishEffectType.CatchAfterEffect, this.gameObject.transform.position));

                foreach (var o in meshRenderer)
                    o.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                Debug.Log("물에 첨벙");
                break;
            }
            yield return null;
        }
    }

    protected abstract void LayerCheckSound(int layer);

    IEnumerator CatchPossbileDelay()
    {
        yield return new WaitForSeconds(2.0f);
        isTargetPossible = true;
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

    public int GetFishIndex()
    {
        return index;
    }

    public FishType GetFishType()
    {
        return fishType;
    }
}

using CellBig;
using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RinoBeetle_Controller : IFish
{
    public Material[] matColor;
    public GameObject objMaterial;

    public override void InitFish(int index, float maxSize, float minSize, float maxSpeed, float minSpeed, float catchDelay, float viewPosZ)
    {
        fishType = FishType.RinoBeetle;
        if (matColor.Length > 0)
        {
            int rndMaterialColor = Random.Range(0, matColor.Length);
            objMaterial.GetComponent<SkinnedMeshRenderer>().material = matColor[rndMaterialColor];
        }
        base.InitFish(index, maxSize, minSize, maxSpeed, minSpeed, catchDelay, viewPosZ);
    }

    public override IEnumerator MoveTarget(Vector3 target, int targetIndex)
    {
        bool isIdleCheck = false;
        bool isWayFind = false;
        bool isRotate = false;
        float checkTime = Random.Range(1.5f, 3);
        float rndWaitTime = Random.Range(1.5f, 3);
        int rndIdle = Random.Range(0, 10);
        float spanTime = 0;
        RaycastHit raycastHit;
        int layerMask = 1 << 20;
        int layerMaskWay = 1 << 23;

        while (Vector3.Distance(this.gameObject.transform.position, target) > 1f && isCapturePossible)
        {
            yield return null;
            spanTime += Time.deltaTime;
            Vector3 dir = target - this.gameObject.transform.position;
            float rndSpeed = Random.Range(minSpeed, maxSpeed);
            if (!isTargetPossible)
                rndSpeed = resetSpeed;

            float rotateTime = Time.deltaTime * 5;
            float moveTime = Time.deltaTime * 0.3f * rndSpeed;

            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward * Mathf.Infinity, Color.red);
            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward + this.gameObject.transform.forward + this.gameObject.transform.right * 2, Color.red);
            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward + this.gameObject.transform.forward - this.gameObject.transform.right * 2, Color.red);

            if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out raycastHit, 2f, layerMask) && !isRotate)
            {
                isRotate = true;
                while (!isWayFind)
                {
                    yield return null;
                    this.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * -15);
                    this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime);
                    if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out raycastHit, Mathf.Infinity, layerMaskWay) && isRotate)
                    {
                        isWayFind = true;
                        isRotate = false;
                    }
                }
                break;
            }

            if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward + this.gameObject.transform.forward + this.gameObject.transform.right, out raycastHit, 2f, layerMask) && !isRotate)
            {
                isRotate = true;
                while (!isWayFind)
                {
                    yield return null;
                    this.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * -15);
                    this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime);
                    if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out raycastHit, Mathf.Infinity, layerMaskWay) && isRotate)
                    {
                        isWayFind = true;
                        isRotate = false;
                    }
                }
                break;
            }

            if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward + this.gameObject.transform.forward - this.gameObject.transform.right, out raycastHit, 2f, layerMask) && !isRotate)
            {
                isRotate = true;
                while (!isWayFind)
                {
                    yield return null;
                    this.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 15);
                    this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime);
                    if (Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, out raycastHit, Mathf.Infinity, layerMaskWay) && isRotate)
                    {
                        isWayFind = true;
                        isRotate = false;
                    }
                }
                break;
            }

            if (!isIdleCheck && spanTime > checkTime && rndIdle < 2 && isTargetPossible)
            {
                isIdleCheck = true;
                ani.SetTrigger("IdleIn");
                yield return new WaitForSeconds(rndWaitTime);
                ani.SetTrigger("IdleOut");
            }

            this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, Quaternion.LookRotation(dir), rotateTime);
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, target, moveTime);
        }

        if (isCapturePossible)
            Message.Send<FishArriveMsg>(new FishArriveMsg(fishType, index, targetIndex));
    }

    protected override void CatchSoundPlay()
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_PopUp);
    }

    protected override void LayerCheckSound(int layer)
    {
        if (layer == 24)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_Fail);
        else if (layer == 21)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_CatchAfter);

    }
}

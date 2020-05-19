using CellBig;
using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discussfish_Controller : IFish
{
    public Material[] matColor;
    public GameObject objMaterial;

    public override void InitFish(int index, float maxSize, float minSize, float maxSpeed, float minSpeed, float catchDelay, float viewPosZ)
    {
        fishType = FishType.Discussfish;
        if (matColor.Length > 0)
        {
            int rndMaterialColor = Random.Range(0, matColor.Length);
            objMaterial.GetComponent<SkinnedMeshRenderer>().material = matColor[rndMaterialColor];
        }
        base.InitFish(index, maxSize, minSize, maxSpeed, minSpeed, catchDelay, viewPosZ);
    }

    public override IEnumerator MoveTarget(Vector3 target, int targetIndex)
    {
        while (Vector3.Distance(this.gameObject.transform.position, target) > 5f && isCapturePossible)
        {
            yield return null;
            Vector3 dir = target - this.gameObject.transform.position;
            float rndSpeed = Random.Range(minSpeed, maxSpeed);
            if (!isTargetPossible)
                rndSpeed = resetSpeed;

            float rotateTime = Time.deltaTime * 5;
            float moveTime = Time.deltaTime * 0.3f * rndSpeed;
            //테스트
            //Debug.DrawRay(this.gameObject.transform.position, (this.gameObject.transform.forward) * 0.1f, Color.red);
            //if(!Physics.Raycast(this.gameObject.transform.position, this.gameObject.transform.forward, 0.1f, layerMask))
            //{
            //    //float rotateTime = Time.deltaTime * 5;
            //    //float moveTime = Time.deltaTime * 0.3f * rndSpeed;
            //    this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, Quaternion.LookRotation(dir), rotateTime);
            //    this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, target, moveTime);
            //}
            //else
            //{
            //    this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, Quaternion.LookRotation(dir), rotateTime);
            //}
            //
            this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, Quaternion.LookRotation(dir), rotateTime);
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, target, moveTime);
        }

        if (isCapturePossible)
            Message.Send<FishArriveMsg>(new FishArriveMsg(fishType, index, targetIndex));
    }

    protected override void CatchSoundPlay()
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Sfx_CatchIng);
    }

    protected override void LayerCheckSound(int layer)
    {
        if (layer == 4)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sfx_Fail);
        else if (layer == 21)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_CatchAfter);
    }
}

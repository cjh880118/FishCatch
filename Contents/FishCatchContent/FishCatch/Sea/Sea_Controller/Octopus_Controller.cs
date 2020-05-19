using JHchoi;
using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus_Controller : IFish
{
    public Material[] matColor;
    public GameObject objMaterial;

    public override void InitFish(int index, float maxSize, float minSize, float maxSpeed, float minSpeed, float catchDelay, float viewPosZ)
    {
        fishType = FishType.Octopus;
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
        float checkTime = Random.Range(1.5f, 3);
        float rndWaitTime = Random.Range(1.5f, 3);
        int rndIdle = Random.Range(0, 10);
        float spanTime = 0;

        while (Vector3.Distance(this.gameObject.transform.position, target) > 5f && isCapturePossible)
        {
            yield return null;
            spanTime += Time.deltaTime;
            Vector3 dir = target - this.gameObject.transform.position;
            float rndSpeed = Random.Range(minSpeed, maxSpeed);
            if (!isTargetPossible)
                rndSpeed = resetSpeed;

            float rotateTime = Time.deltaTime * 5;
            float moveTime = Time.deltaTime * 0.3f * rndSpeed;

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
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Sea_CatchWater);
    }

    protected override void LayerCheckSound(int layer)
    {
        if (layer == 4)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Sfx_Fail);
        else if (layer == 21)
            SoundManager.Instance.PlaySound((int)SoundFishCatch.Bug_CatchAfter);
    }
}

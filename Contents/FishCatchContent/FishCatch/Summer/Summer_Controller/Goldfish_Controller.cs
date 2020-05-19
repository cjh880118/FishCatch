using JHchoi;
using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goldfish_Controller : IFish
{
    public Material[] matColor;
    public GameObject objMaterial;

    public override void InitFish(int index, float maxSize, float minSize, float maxSpeed, float minSpeed, float catchDelay, float viewPosZ)
    {
        int rndMaterialColor = Random.Range(0, matColor.Length);
        fishType = (FishType)(int)FishType.Orange_Goldfish + rndMaterialColor;
        objMaterial.GetComponent<SkinnedMeshRenderer>().material = matColor[rndMaterialColor];
        base.InitFish(index, maxSize, minSize, maxSpeed, minSpeed, catchDelay, viewPosZ);
    }

    protected override void CatchSoundPlay()
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Sfx_CatchIng);
    }
    protected override void LayerCheckSound(int layer)
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Sfx_Fail);
    }
}

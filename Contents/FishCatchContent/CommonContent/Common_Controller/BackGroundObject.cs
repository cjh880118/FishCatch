using JHchoi;
using JHchoi.Common;
using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackGroundObject : MonoBehaviour
{
    [Header("Effect")]
    public GameObject catchEffect;
    public GameObject missEffect;
    public GameObject respawnEffect;
    public GameObject catchAfterEffect;
    public GameObject[] CatchInput;

    ObjectPool ObjPoolCatch;
    ObjectPool ObjPoolMiss;
    ObjectPool ObjPoolRespawn;
    ObjectPool ObjPoolCatchAfter;

    //이펙트 메모리풀 //잡았다(Catching_fish_Effect) 놓쳤다(Fail_fish_Effect) 리스폰(Summon_fish_Effect)

    [Header("Way Target")]
    public GameObject[] arrayTarget;

    [Header("Catch Target")]
    public GameObject[] arrayPlate;

    float maxPosition;
    float minPosition;

    public virtual void InitBackGround(float minPosition, float maxPosition)
    {
        this.minPosition = minPosition;
        this.maxPosition = maxPosition;
        AddMessage();

        if (catchEffect != null)
            ObjPoolCatch = CreateObjectPool(catchEffect, 10);

        if (missEffect != null)
            ObjPoolMiss = CreateObjectPool(missEffect, 10);

        if (respawnEffect != null)
            ObjPoolRespawn = CreateObjectPool(respawnEffect, 10);

        if (catchAfterEffect != null)
            ObjPoolCatchAfter = CreateObjectPool(catchAfterEffect, 10);

    }

    ObjectPool CreateObjectPool(GameObject o, int count)
    {
        var PoolObject = new GameObject();
        PoolObject.name = o.name + "Pool";
        PoolObject.transform.SetParent(transform);
        ObjectPool p = PoolObject.AddComponent<ObjectPool>();
        p.PreloadObject(count, o as GameObject);
        return p;
    }

    private void AddMessage()
    {
        Message.AddListener<CatchPlayerMsg>(CatchPlayer);
        Message.AddListener<PlayEffectMsg>(PlayEffect);
    }

    private void CatchPlayer(CatchPlayerMsg msg)
    {
        if (CatchInput.Length > 0 && !CatchInput[msg.playerIndex].GetComponent<ParticleSystem>().isPlaying)
        {
            StartCoroutine(EffectOff(msg.playerIndex));
        }
    }

    IEnumerator EffectOff(int index)
    {
        CatchInput[index].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        ParticleSystem particleSystem = CatchInput[index].GetComponent<ParticleSystem>();

        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        CatchInput[index].SetActive(false);
    }

    public Vector3 GetTargetPosition(int index)
    {
        float positionX = Random.Range(minPosition, maxPosition);
        float positionY = Random.Range(minPosition, maxPosition);
        float positionZ = Random.Range(minPosition, maxPosition);
        return arrayTarget[index].transform.position - new Vector3(positionX, positionY, positionZ);
    }

    public int GetTargetCount()
    {
        return arrayTarget.Length;
    }

    public int GetMostNearPosition(Vector3 pos)
    {
        float mostNear = 0;
        int mostNearIndex = 0;
        for (int i = 0; i < arrayTarget.Length; i++)
        {
            float distance = Vector3.Distance(pos, arrayTarget[i].transform.position);
            if (i == 0)
            {
                mostNearIndex = 0;
                mostNear = distance;
            }
            else if (mostNear > distance)
            {
                mostNear = distance;
                mostNearIndex = i;
            }

        }
        return mostNearIndex;
    }

    public Vector2 GetPlateViewPortPosition(int index)
    {
        return Camera.main.WorldToViewportPoint(arrayPlate[index].transform.position);
    }

    public Vector3 GetPlateWorldPosition(int index)
    {
        return arrayPlate[index].transform.position;
    }

    private void PlayEffect(PlayEffectMsg msg)
    {
        GameObject particle = null;
        ObjectPool objectPool = null;
        GameObject baseParticle = null;

        if (msg.effectType == FishEffectType.Catch)
        {
            objectPool = ObjPoolCatch;
            baseParticle = catchEffect;
        }
        else if (msg.effectType == FishEffectType.Miss)
        {
            objectPool = ObjPoolMiss;
            baseParticle = missEffect;
        }
        else if (msg.effectType == FishEffectType.Respawn)
        {
            objectPool = ObjPoolRespawn;
            baseParticle = respawnEffect;
        }
        else if (msg.effectType == FishEffectType.CatchAfterEffect)
        {
            objectPool = ObjPoolCatchAfter;
            baseParticle = catchAfterEffect;
        }

        if (objectPool != null)
        {
            particle = objectPool.GetObject(objectPool.transform);

            if (msg.effectType != FishEffectType.CatchAfterEffect)
                particle.transform.localScale = new Vector3(baseParticle.transform.localScale.x / 2, baseParticle.transform.localScale.y / 2, baseParticle.transform.localScale.z / 2);

            particle.gameObject.transform.position = msg.position;
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            StartCoroutine(ParticlePlay(particleSystem, objectPool, particle));
        }
    }

    IEnumerator ParticlePlay(ParticleSystem particleSystem, ObjectPool objPool, GameObject particle)
    {
        yield return new WaitForSeconds(0.2f);

        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        objPool.PoolObject(particle);
    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    private void RemoveMessage()
    {
        Message.RemoveListener<CatchPlayerMsg>(CatchPlayer);
        Message.RemoveListener<PlayEffectMsg>(PlayEffect);
    }
}

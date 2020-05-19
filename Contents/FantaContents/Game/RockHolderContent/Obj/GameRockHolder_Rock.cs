using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

using DG.Tweening;

public class GameRockHolder_Rock : MonoBehaviour
{
    public RockType rockType;

    public FracturedObject fracturedObject;
    public ExplosionSource explosionSource;

    public Animator anim;

    public GameObject hitParticle_1;
    public GameObject hitParticle_2;

    public int rockPhase;

    Vector3 explosionOriginPos;

    Coroutine hitEventCor;

    bool isTouch;

    private void Awake()
    {
        explosionOriginPos = explosionSource.transform.localPosition;
    }

    private void OnEnable()
    {
        AddMessage();
        Active();    
    }

    private void OnDisable()
    {
        RemoveMessage();

        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnPoolObjectMsg);
    }

    void OnPoolObjectMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void Active()
    {
        isTouch = false;

        explosionSource.InfluenceRadius = 2.0f;
        explosionSource.Force = 10.0f;
        explosionSource.transform.localPosition = explosionOriginPos;

        transform.localRotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));

        anim.SetTrigger("Start");

        rockPhase = 0;
    }

    public void SetSpawn(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Hit()
    {
        isTouch = true;

        if (hitEventCor != null)
            return;

        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        float timer = 0.0f;

        while(true)
        {
            Debug.LogError(timer);

            if (isTouch)
            {
                hitParticle_1.SetActive(false);

                timer += Time.deltaTime * 3f;
                isTouch = false;

                hitParticle_1.SetActive(true);

                RandomSound((int)SoundType_GameFX.RockHolder_Hit1, (int)SoundType_GameFX.RockHolder_Hit4);
                Message.Send<ADDScore>(new ADDScore(100));

            }

            if (rockPhase == 0 && timer > 0.05f)
                NextRockPhase();
            else if (rockPhase == 1 && timer > 0.5f)
                NextRockPhase("Hit");
            else if (rockPhase == 2 && timer > 1.0f)
                NextRockPhase();
            else if (rockPhase == 3 && timer > 1.3f)
            {
                NextRockPhase("Die");
                StartCoroutine(LastPhase());
                SoundManager.Instance.PlaySound((int)SoundType_GameFX.RockHolder_Die1);
                SoundManager.Instance.PlaySound((int)SoundType_GameFX.RockHolder_Die2);
                break;
            }

            yield return new WaitForSeconds(1.0f);
            yield return null;
        }
    }

    void NextRockPhase(string animStr = null)
    {
        rockPhase++;
        RockBreak(rockPhase);

        if(animStr != string.Empty)
            anim.SetTrigger(animStr);
    }

    IEnumerator LastPhase()
    {
        explosionSource.Force = 30.0f;

        hitParticle_2.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        fracturedObject.ScaleDisapear();
        yield return new WaitForSeconds(1.0f);

        DeActive();
    }

    void RockBreak(int rocPhase)
    {
        Vector3 currentExplosionPos = explosionSource.transform.localPosition;
        float targetExplosionPosY;

        anim.ResetTrigger("HH");
        anim.SetTrigger("HH");

        if (rockPhase == 1)
            targetExplosionPosY = 3.3f;
        else if (rockPhase == 2)
            targetExplosionPosY = 3.1f;
        else if (rockPhase == 3)
            targetExplosionPosY = 2.8f;
        else
            targetExplosionPosY = 1.5f;

        explosionSource.transform.DOLocalMoveY(targetExplosionPosY, 0.3f);

        anim.SetTrigger("Idle");

        RandomSound((int)SoundType_GameFX.RockHolder_Break1, (int)SoundType_GameFX.RockHolder_Break3);
    }

    void RandomSound(int min, int max)
    {
        int randomFootSnd = Random.Range(min, max + 1);
        SoundManager.Instance.PlaySound(randomFootSnd);
    }

    public void DeActive()
    {
        explosionSource.transform.localPosition = explosionOriginPos;
        fracturedObject.ResetChunks();

        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)rockType, gameObject));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameSlideLAVA_Foot : MonoBehaviour
{
    public GameObject hitParticle;
    public GameObject explosionSource;

    Collider collider;

    Coroutine hitEventCor;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();
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

    public void Hit()
    {
        if (hitEventCor != null)
            return;

        hitEventCor = StartCoroutine(HitEvent());

        FootSound();
    }

    IEnumerator HitEvent()
    {
        collider.enabled = true;

        Message.Send<ADDScore>(new ADDScore(100));
        yield return new WaitForSeconds(1.0f);

        collider.enabled = false;

        DeActive();
    }

    void DeActive()
    {
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }

    void FootSound()
    {
        int randomFootSnd = Random.Range((int)SoundType_GameFX.SlideLAVA_Step1, ((int)SoundType_GameFX.SlideLAVA_Step2 + 1));

        SoundManager.Instance.PlaySound(randomFootSnd);

        randomFootSnd = Random.Range((int)SoundType_GameFX.SlideLAVA_Flame1, ((int)SoundType_GameFX.SlideLAVA_Flame3 + 1));

        SoundManager.Instance.PlaySound(randomFootSnd);
    }
}

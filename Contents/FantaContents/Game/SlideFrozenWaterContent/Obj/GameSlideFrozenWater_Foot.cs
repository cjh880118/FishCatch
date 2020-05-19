using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameSlideFrozenWater_Foot : MonoBehaviour
{
    public GameObject hitParticle;
    public GameObject explosionSource;

    Collider collider = null;

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
    }

    IEnumerator HitEvent()
    {
        hitParticle.SetActive(true);
        collider.enabled = true;

        FootSound();
        Message.Send<ADDScore>(new ADDScore(100));
        yield return new WaitForSeconds(0.1f);

        collider.enabled = false;
        yield return new WaitForSeconds(1.0f);

        hitParticle.SetActive(false);

        DeActive();
    }

    void FootSound()
    {
        int randomFootSnd = Random.Range((int)SoundType_GameFX.SlideFrozenWater_BreakIce1, ((int)SoundType_GameFX.SlideFrozenWater_BreakIce2 + 1));
        SoundManager.Instance.PlaySound(randomFootSnd);
    }

    void DeActive()
    {
        if(hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameWaterFish_Foot : MonoBehaviour
{
    public GameObject hitParticle;

    public Collider collider = null;

    Coroutine hitEventCor;

    private void OnEnable()
    {
        AddMessage();
        Active();
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

    void Active()
    {
        collider.enabled = true;
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

        FootSound();

        Message.Send<ADDScore>(new ADDScore(100));

        yield return new WaitForSeconds(0.1f);

        collider.enabled = false;

        yield return new WaitForSeconds(0.5f);

        hitParticle.SetActive(false);

        DeActive();
    }

    void FootSound()
    {
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.WaterFish_Step);
    }

    void DeActive()
    {
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }
}

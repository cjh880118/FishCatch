using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameFallen_Foot : MonoBehaviour
{
    Collider collider = null;

    Coroutine hitEventCor;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

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
        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        int randomStepSnd = Random.Range((int)SoundType_GameFX.Fallen_Step1, ((int)SoundType_GameFX.Fallen_Step3 + 1));

        SoundManager.Instance.PlaySound(randomStepSnd);

        Message.Send<ADDScore>(new ADDScore(100));
        yield return new WaitForSeconds(0.1f);

        collider.enabled = false;

        DeActive();
    }

    void DeActive()
    {
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)FallenObjectType.Foot, gameObject));
    }
}

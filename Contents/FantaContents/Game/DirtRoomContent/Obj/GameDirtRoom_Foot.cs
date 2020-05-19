using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameDirtRoom_Foot : MonoBehaviour
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
        Message.Send<ADDScore>(new ADDScore(100));
        yield return new WaitForSeconds(0.1f);
        collider.enabled = false;
        DeActive();
    }

    void FootSound()
    {
        int randomStepSnd = Random.Range((int)SoundType_GameFX.Fallen_Step1, ((int)SoundType_GameFX.Fallen_Step3 + 1));
        SoundManager.Instance.PlaySound(randomStepSnd);
    }

    void DeActive()
    {
        if (hitEventCor != null)
            hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)DirtRoom_FootType.Foot, gameObject));
    }
}

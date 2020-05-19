using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameRoadDestruction_Foot : MonoBehaviour
{
    public GameObject hitParticle;
    public GameObject explosionSource;

    Collider collider = null;

    Coroutine hitEventCor;
    Coroutine soundCor;

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

    public void Hit(bool isSound)
    {
        if (hitEventCor != null)
            return;

        hitEventCor = StartCoroutine(HitEvent());
        TouchSound();
    }

    IEnumerator HitEvent()
    {
        hitParticle.SetActive(true);
        collider.enabled = true;
        Message.Send<ADDScore>(new ADDScore(100));

        yield return new WaitForSeconds(0.2f);

        collider.enabled = false;

        yield return new WaitForSeconds(1.0f);

        hitParticle.SetActive(false);

        DeActive();
    }

    void DeActive()
    {
        if(hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }

    void TouchSound()
    {
        int randomRockIndex = Random.Range(0, 4);

        SoundManager.Instance.PlaySound((((int)SoundType_GameFX.RoadDestruction_Rock0) + randomRockIndex));
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.RoadDestruction_Impact);
    }
}

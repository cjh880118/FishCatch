using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;

using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameSlideSnow_Foot : MonoBehaviour
{
    public GameObject[] hitParticle;

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
        int randomStepSnd = Random.Range((int)SoundType_GameFX.SlideSnow_Flake1, ((int)SoundType_GameFX.SlideSnow_Flake2 + 1));

        SoundManager.Instance.PlaySound(randomStepSnd);
        Message.Send<ADDScore>(new ADDScore(100));
        yield return new WaitForSeconds(0.1f);

        collider.enabled = false;

        HitParticle(true);
        yield return new WaitForSeconds(1.0f);

        HitParticle(false);

        DeActive();
    }

    void HitParticle(bool active)
    {
        for (int index = 0; index < hitParticle.Length; index++)
            hitParticle[index].SetActive(active);
    }

    void DeActive()
    {
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)SnowObjectType.Foot, gameObject));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

using DG.Tweening;

public class GameGrass_Foot : MonoBehaviour
{
    public Transform hitPlane;

    public ParticleSystem hitParticleIn;
    public ParticleSystem hitParticleOut;

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
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        hitPlane.transform.localScale = Vector3.one * 0.1f;
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
        FootSound();

        Message.Send<ADDScore>(new ADDScore(100));

        hitPlane.DOScale(Vector3.zero, 1.0f);

        yield return StartCoroutine(Particle());

        yield return new WaitForSeconds(1.0f);

        DeActive();
    }

    IEnumerator Particle()
    {
        hitParticleOut.gameObject.SetActive(true);
        while (hitParticleOut.isPlaying)
            yield return null;
        hitParticleOut.gameObject.SetActive(false);

        hitParticleIn.gameObject.SetActive(true);
        while (hitParticleIn.isPlaying)
            yield return null;
        hitParticleIn.gameObject.SetActive(false);
    }

    void FootSound()
    {
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Grass_Step);
    }

    void DeActive()
    {
        if(hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }
}

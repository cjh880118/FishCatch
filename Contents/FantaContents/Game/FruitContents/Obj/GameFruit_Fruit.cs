using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

public class GameFruit_Fruit : MonoBehaviour
{
    public FruitType fruitType;

    public GameObject meshObj;

    [SerializeField]
    public Rigidbody rigid = null;
    public Collider collider = null;
    //public GameObject[] fruitObjs;
    public GameObject hitParticle;

    public ScoreTextControl scoreTextControl;

    float bottomDiePosY = -460f;

    protected int addScore = 100;

    Coroutine hitEventCor = null;
    Coroutine delayDeActiveCor = null;

    private void Awake()
    {
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();

        if (collider == null)
            collider = GetComponent<Collider>();

        rigid.mass = 5;
    }

    private void OnEnable()
    {
        Active();
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

    protected virtual void Active()
    {
        collider.enabled = true;
    }

    public void SetSpawn(Vector3 pos, Quaternion rot, float powerX, float powerY)
    {
        transform.localPosition = pos;
        transform.localRotation = rot;

        rigid.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        rigid.angularVelocity = Vector3.zero;

        rigid.AddForce(powerX, powerY, 0.0f);
        rigid.AddRelativeTorque(300.0f, 2000.0f, 0.0f);
    }

    public virtual void Hit()
    {
        if (hitEventCor != null)
            return;

        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        collider.enabled = false;
        meshObj.SetActive(false);
        hitParticle.SetActive(true);
        
        scoreTextControl.SetScore(addScore);

        HitSound();

        Message.Send<ADDScore>(new ADDScore(addScore));

        yield return new WaitForSeconds(1.0f);
    }

    protected virtual void HitSound()
    {
        int randomHitSoundIndex = UnityEngine.Random.Range((int)SoundType_GameFX.Fruit_Hit1, ((int)SoundType_GameFX.Fruit_Hit2 + 1));
        SoundManager.Instance.PlaySound(randomHitSoundIndex);
    }

    IEnumerator DelayDeActive()
    {
        while (transform.localPosition.y > bottomDiePosY)
            yield return null;

        DeActive();
    }

    protected virtual void DeActive()
    {
        hitParticle.SetActive(false);

        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        if(delayDeActiveCor != null)
            StopCoroutine(delayDeActiveCor);

        hitEventCor = null;
        delayDeActiveCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)fruitType, gameObject));
    }
}

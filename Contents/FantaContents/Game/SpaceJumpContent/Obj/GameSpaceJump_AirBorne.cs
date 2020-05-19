using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

using DG.Tweening;

public class GameSpaceJump_AirBorne : MonoBehaviour
{
    public AirBorneType airBorneType;

    public SpriteRenderer sprite;
    public MeshRenderer meshRen;
    Animator anim;

    public Collider collider = null;

    public GameObject[] hitParticle;

    public ScoreTextControl scoreTextControl;

    Coroutine hitEventCor;

    int score = 100;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
        Message.AddListener<PoolObjectMsg>(OnDeActiveAirBorneMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnDeActiveAirBorneMsg);
    }

    void Active()
    {
        hitEventCor = null;

        transform.position = new Vector3(10000f, 10000, 10000);

        ObjectColor(Color.white, 0.0f);

        anim.Rebind();
        anim.SetTrigger("Start");
    }

    public void SetSpawn(Vector3 spawnPoint)
    {
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.SpaceJump_Cloud1);

        transform.position = spawnPoint;
        collider.enabled = true;
    }

    public void Hit()
    {
        if (hitEventCor != null)
            return;

        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        collider.enabled = false;

        Message.Send<AirBorneInitMsg>(new AirBorneInitMsg());

        anim.SetTrigger("End");

        ObjectColor(Color.red, 0.5f);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.SpaceJump_Cloud2);

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        yield return StartCoroutine(HitParticle(true));
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(HitParticle(false));

        hitEventCor = null;

        DeActive();
    }

    void ObjectColor(Color color, float during)
    {
        sprite.DOColor(color, during);

        if (meshRen != null)
            meshRen.material.DOColor(color, during);
    }

    IEnumerator HitParticle(bool active)
    {
        for(int index = 0; index < hitParticle.Length; index++)
        {
            hitParticle[index].SetActive(active);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnDeActiveAirBorneMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void DeActive()
    {
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)airBorneType, gameObject));
    }
}

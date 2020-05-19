using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig.Contents.Event;

using DG.Tweening;

public class GameSlideSnow_Snow : MonoBehaviour
{
    Rigidbody rigid = null;
    Collider collider = null;

    public SpriteRenderer spriteRen = null;
    public Sprite[] sprite = null;

    Vector3 originPos;

    float resetTime = 1.5f;

    Coroutine Cor_ColFoot;

    private void OnEnable()
    {
        AddMessage();
        Active();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void Active()
    {
        SetSnow();
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

    void SetSnow()
    {
        spriteRen.sprite = sprite[Random.Range(0, sprite.Length)];
    }

    public void SetSpawn(Vector3 pos)
    {
        transform.position = pos;
        originPos = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Cor_ColFoot != null)
            StopCoroutine(Cor_ColFoot);

        Cor_ColFoot = StartCoroutine(ColFoot());
    }

    IEnumerator ColFoot()
    {
        yield return new WaitForSeconds(resetTime);
        transform.DOMove(originPos, 0.5f);
    }

    public void DeActive()
    {
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)SnowObjectType.Snow, gameObject));
    }
}

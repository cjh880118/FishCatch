using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

using DG.Tweening;

public class GamePaints_Paint : MonoBehaviour
{
    public PaintType paintType;

    public GameObject drop;
    public GameObject[] splatPaint;

    public ScoreTextControl scoreTextControl;

    MeshRenderer[] splatMeshRen;

    Color[] splatOriginColor;

    public GameObject hitParticle;

    public Collider collider;

    Coroutine hitEventCor;

    int score = 100;

    private void Awake()
    {
        splatMeshRen = new MeshRenderer[splatPaint.Length];
        splatOriginColor = new Color[splatPaint.Length];

        for(int index = 0; index < splatPaint.Length; index++)
        {
            splatMeshRen[index] = splatPaint[index].GetComponent<MeshRenderer>();
            splatOriginColor[index] = splatMeshRen[index].material.color;
        }
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
        Message.AddListener<PoolObjectMsg>(OnDeActiveVertexMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnDeActiveVertexMsg);
    }

    void Active()
    {
        collider.enabled = true;
        transform.localPosition = new Vector3(10000, 10000, 0);

        drop.SetActive(true);

        for(int index = 0; index < splatPaint.Length; index++)
        {
            splatPaint[index].SetActive(false);
            splatMeshRen[index].material.color = splatOriginColor[index];
        }
    }

    public void SetSpawn(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
    }

    public void Hit()
    {
        if (hitEventCor != null)
            return;

        collider.enabled = false;

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Paints_Hit);

        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        int randomSplat = Random.Range(0, splatPaint.Length);

        drop.SetActive(false);

        splatPaint[randomSplat].SetActive(true);

        hitParticle.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        splatMeshRen[randomSplat].material.DOFade(0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        hitParticle.SetActive(false);

        hitEventCor = null;

        DeActive();
    }

    void OnDeActiveVertexMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void DeActive()
    {
        collider.enabled = false;
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)paintType, gameObject));
    }
}

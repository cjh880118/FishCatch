using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;
using JHchoi.UI.Event;
using JHchoi.Contents.Event;

using DG.Tweening;

public class GameVertex_Vertex : MonoBehaviour
{
    public GameObject[] figures;
    public GameObject hitParticle;

    public Collider collider;

    public int spawnIndex;

    public int maxHP = 4;
    public int currentHP = 0;
    public float scaleSpeed = 3;

    public Vector3 originScale;

    public ScoreTextControl scoreTextControl;

    Coroutine hitEventCor;

    int score = 100;

    private void Awake()
    {
        originScale = transform.localScale;
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
        currentHP = maxHP;

        transform.localScale = originScale;

        collider.enabled = true;

        figures[currentHP].SetActive(true);
    }

    public void SetSpawn(int index, Transform spawnPoint)
    {
        spawnIndex = index;
        transform.position = spawnPoint.transform.position;
    }

    public void Hit()
    {
        if (hitEventCor != null)
            return;

        if (currentHP > 0)
        {
            figures[currentHP].SetActive(false);

            currentHP--;

            figures[currentHP].SetActive(true);
            hitEventCor = StartCoroutine(HitEvent());
        }
        else
        {
            currentHP--;
            hitEventCor = StartCoroutine(HitEvent());
        }

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Vertex_Hit);
    }

    IEnumerator HitEvent()
    {
        hitParticle.SetActive(true);

        if (currentHP < 0)
        {
            transform.DOScale(0, 1.0f);
            yield return new WaitForSeconds(1.0f);

            DeActive();
        }
        else
            yield return new WaitForSeconds(0.5f);

        hitParticle.SetActive(false);

        hitEventCor = null;
    }

    void OnDeActiveVertexMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void DeActive()
    {
        collider.enabled = false;

        for (int index = 0; index < figures.Length; index++)
            figures[index].SetActive(false);

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(spawnIndex, gameObject));
    }
}

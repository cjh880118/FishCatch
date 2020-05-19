using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameFirework_Firework : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer SpriteRenderer;

    float Speed;
    public Sprite[] mFirworkSprites;
    public Collider mCollider;
    public GameObject mEffect_FollowFire;
    public GameObject mEffect_explosion;

    public ScoreTextControl scoreTextControl;

    int score = 100;

    Coroutine mCor_Life = null;
    Coroutine mCor_Update = null;
    private void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

    public void Active()
    {
        if (SpriteRenderer == null)
        {
            Debug.LogError("NULL SpriteRenerer");
            return;
        }

        Speed = Random.Range(80, 200);
        mCollider.enabled = false;
        SpriteRenderer.enabled = true;
        mEffect_explosion.SetActive(false);
        mEffect_FollowFire.SetActive(true);
        SpriteRenderer.sprite = mFirworkSprites[Random.Range(0, mFirworkSprites.Length)];
        mCor_Update = StartCoroutine(Cor_Update());
        mCor_Life = StartCoroutine(Cor_Life());
        // n초후 삭제 // 도착 후 삭제
    }

    public void Hit()
    {
        SpriteRenderer.enabled = false;
        mCollider.enabled = false;
        mEffect_explosion.SetActive(true);
        mEffect_FollowFire.SetActive(false);
        StopCoroutine(mCor_Update);
        StopCoroutine(mCor_Life);
        StartCoroutine(Cor_DealyDeactive());

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.FireWork_Explosion);
    }

    public void Deactive()
    {
        StopAllCoroutines();
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(this.transform.gameObject));
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            transform.position += transform.up * Time.deltaTime * Speed;
            if (transform.position.y > 300) Deactive();
            yield return null;
        }
    }

    IEnumerator Cor_Life()
    {
        yield return new WaitForSeconds(0.5f);
        mCollider.enabled = true;
    }

    IEnumerator Cor_DealyDeactive()
    {
        yield return new WaitForSeconds(2.0f);
        Deactive();
    }

    void OnPoolObjectMsg(PoolObjectMsg msg)
    {
        Deactive();
    }
}

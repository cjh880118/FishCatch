using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Contents.Event;

public class GameBlockBreak_Block : MonoBehaviour
{
    public BlockType blockType;

    [Header("[ Common ]")]
    public Animation m_pAni = null;
    public BoxCollider m_pCollider = null;
    public GameObject m_pSprite = null;
    public SpriteRenderer mainSprite;

    [Header("[ Normal ]")]
    public GameObject m_pSprite_Freeze = null;
    public Sprite[] candySprite;
    public Sprite freezeSprite;

    [Header("[ Effect ]")]
    public GameObject pangEff;
    public GameObject iceEff;

    public ScoreTextControl scoreTextControl;

    protected int score = 100;

    int m_nBlockClassNum = 0;

    internal bool m_bLife = false;
    internal int m_nX = 0;
    internal int m_nY = 0;
    public int m_nHp = 0;

    protected virtual void OnEnable()
    {
        m_pSprite.SetActive(true);
        m_pCollider.enabled = false;
        m_bLife = false;
        m_nHp = 1;
        StartCoroutine(Active());

        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnDeActiveBlockMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnDeActiveBlockMsg);
    }

    IEnumerator Active()
    {
        m_pAni.Play("Block_Create");
        yield return new WaitForSeconds(1.0f);
        m_pAni.Play("Block_None");
        m_bLife = true;
        m_pCollider.enabled = true;
        yield return null;
    }

    public void Hit()
    {
        m_nHp--;
        if (m_nHp <= 0)
        {
            StartCoroutine(Die());
            pangEff.SetActive(true);
        }
        else
        {
            if (m_nHp == 2)
                m_pAni.Play("Block_Freeze");
            else if (m_nHp == 1)
            {
                m_pSprite.gameObject.SetActive(true);
                m_pSprite_Freeze.gameObject.SetActive(false);
                m_pAni.Play("Block_None");
            }
            iceEff.SetActive(true);

            StartCoroutine(Cor_ColliderTime());
        }

        HitSound();
    }

    IEnumerator Cor_ColliderTime()
    {
        m_pCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        m_pCollider.enabled = true;
    }

    public virtual void Freeze()
    {
        m_nHp = 3;
        m_pSprite.gameObject.SetActive(false);
    }

    protected IEnumerator Die()
    {
        m_bLife = false;
        m_pCollider.enabled = false;
        m_pAni.Play("Block_Die");
        HitSound();
        ScoreUpdate();
        yield return new WaitForSeconds(1.0f);
        DeActive();
        yield return null;
    }

    protected virtual void ScoreUpdate()
    {

    }

    protected virtual void HitSound()
    {

    }

    public void DeActive()
    {
        m_pCollider.enabled = false;
        m_bLife = false;
        pangEff.SetActive(false);
        iceEff.SetActive(false);
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)blockType, this.gameObject));
    }

    void OnDeActiveBlockMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Hit();
        }
    }
}

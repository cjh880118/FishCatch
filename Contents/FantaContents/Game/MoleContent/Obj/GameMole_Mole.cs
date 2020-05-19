using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameMole_Mole : MonoBehaviour
{
    public MoleType moleType;
    public MoleState moleState;

    public GameObject meshObj;
    public GameObject hitParticle;

    public ScoreTextControl scoreTextControl;

    public Animator anim;
    Collider collider;

    GameMole_Hole tempHole;

    float waitTime = 2.0f;

    bool isFake;

    protected int addScore = 100;

    Coroutine stateCor;
    Coroutine hitEventCor;

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

    public void SetSpawn(GameMole_Hole hole, Vector3 pos)
    {
        tempHole = hole;
        transform.position = new Vector3(pos.x, -1.75f, pos.z);

        Active();
        SetFake();
    }

    protected virtual void Active()
    {
        meshObj.SetActive(true);
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mole_Init);
    }

    void SetFake()
    {
        isFake = Random.Range(0, 100) < 15 ? true : false;

        if(isFake)
            ChangeState(MoleState.Fake);
        else
            ChangeState(MoleState.Show);
    }

    protected void ChangeState(MoleState eState)
    {
        moleState = eState;
        if (stateCor != null)
        {
            StopCoroutine(stateCor);
            stateCor = null;
        }
        switch (moleState)
        {
            case MoleState.Show:
                stateCor = StartCoroutine(Cor_State_Show());
                break;
            case MoleState.Fake:
                stateCor = StartCoroutine(Cor_State_Fake());
                break;
            case MoleState.Hide:
                stateCor = StartCoroutine(Cor_State_Hide());
                break;
        }
    }

    IEnumerator Cor_State_Show()
    {
        anim.SetTrigger("Show");
        collider.enabled = false;
        yield return new WaitForSeconds(0.3f);

        collider.enabled = true;
        yield return new WaitForSeconds(waitTime);

        collider.enabled = false;
        ChangeState(MoleState.Hide);
        yield return null;
    }

    IEnumerator Cor_State_Hide()
    {
        anim.SetTrigger("Hide");
        collider.enabled = false;
        yield return new WaitForSeconds(1.0f);

        DeActive();
        yield return null;
    }

    IEnumerator Cor_State_Fake()
    {
        anim.SetTrigger("Fake");
        collider.enabled = false;
        yield return new WaitForSeconds(2.0f);

        DeActive();
        yield return null;
    }

    public virtual void Hit()
    {
        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        collider.enabled = false;
        meshObj.SetActive(false);
        hitParticle.SetActive(true);

        Message.Send<ADDScore>(new ADDScore(addScore));
        scoreTextControl.SetScore(addScore);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mole_Hit);

        yield return new WaitForSeconds(1.0f);

        hitParticle.SetActive(false);
        DeActive();

        yield return null;
    }

    protected virtual void DeActive()
    {
        tempHole.DeActive();

        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)moleType, gameObject));
    }
}

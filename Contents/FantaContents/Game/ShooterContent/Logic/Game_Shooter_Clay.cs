using UnityEngine;
using System.Collections;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class Game_Shooter_Clay : MonoBehaviour
{
    public Rigidbody m_pRigi = null;
    public Collider m_pCollider = null;
    public GameObject m_pVase = null;
    public Game_Shooter_Clay_Fragment m_pFragment = null;
    public GameObject ParticleObj = null;

    public ScoreTextControl scoreTextControl;

    Coroutine Cor_active = null;

    int score = 100;

    protected void OnDestroy()
    {
        m_pRigi = null;
        m_pCollider = null;
        m_pVase = null;
        m_pFragment.Destroy();
        m_pFragment = null;
    }

    private void Awake()
    {
        m_pFragment.Enter();
    }

    public void Active()
    {
        m_pFragment.Reset_Fragment();
        m_pFragment.gameObject.SetActive(false);

        ParticleObj.SetActive(false);

        transform.localRotation = Quaternion.Euler(new Vector3(-28.0f, Random.Range(-20.0f, 20.0f), 0.0f));
        m_pRigi.isKinematic = false;
        m_pRigi.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        m_pRigi.AddRelativeForce(0.0f, 300.0f, 350.0f);
        m_pRigi.AddRelativeTorque(0.0f, 100.0f, 0.0f);

        m_pVase.SetActive(true);
        
        m_pCollider.enabled = false;
        Cor_active = StartCoroutine(Cor_Active());
    }

    IEnumerator Cor_Active()
    {
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Shot);
        yield return new WaitForSeconds(0.5f);
        // 터치 가능
        m_pCollider.enabled = true;
        yield return new WaitForSeconds(5.0f);

        DeActive();
    }

    public void Hit()
    {
        StartCoroutine(Cor_Death());
        StopCoroutine(Cor_active);

        ParticleObj.SetActive(true);
        m_pVase.SetActive(false);
        m_pCollider.enabled = false;
        m_pFragment.gameObject.SetActive(true);
        m_pFragment.AddForce(300.0f);
        m_pRigi.isKinematic = true;

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);
    }

    IEnumerator Cor_Death()
    {
        yield return new WaitForSeconds(2.0f);
        // 죽음
        DeActive();
        yield return null;
    }

    void DeActive()
    {
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(this.transform.gameObject));
    }
}
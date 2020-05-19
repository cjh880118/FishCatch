using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameMeteoShower_Meteo : MonoBehaviour
{
    public MeteoType meteoType;

    public GameObject meshObj;
    public GameObject hitParticle;

    public Animator anim;
    public Collider collider = null;

    public ScoreTextControl scoreTextControl;

    int score = 100;

    float m_fLeft_X = -10.0f;
    float m_fRight_X = 10.0f;
    float m_fBottom_Y = -8.0f;
    float m_fTop_Y = 8.0f;
    float m_Pos_Z = 250f;
    float moveSpeed = 60f;

    Coroutine moveCor;

    private void OnEnable()
    {
        Active();
    }

    private void OnDisable()
    {
        if (moveCor != null)
            StopCoroutine(moveCor);

        moveCor = null;
    }

    void Active()
    {
        collider.enabled = true;
        meshObj.SetActive(true);

        float fPos_x = Random.Range(m_fLeft_X, m_fRight_X);
        float fPos_y = Random.Range(m_fBottom_Y, m_fTop_Y);

        transform.localPosition = new Vector3(fPos_x, fPos_y, m_Pos_Z);

        moveCor = StartCoroutine(MoveMeteo());
    }

    IEnumerator MoveMeteo()
    {
        while(transform.localPosition.z > -5f)
        {
            transform.localPosition -= (Vector3.forward * moveSpeed) * Time.deltaTime;
            yield return null;
        }

        DeActive();
    }

    public void Hit()
    {
        collider.enabled = false;
        meshObj.SetActive(false);
        hitParticle.SetActive(true);
        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.MeteoShower_Explosion);

        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(1.0f);

        DeActive();
    }

    void DeActive()
    {
        hitParticle.SetActive(false);

        if (moveCor != null)
            StopCoroutine(moveCor);

        moveCor = null;

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage((int)meteoType, gameObject));
    }
}

using System.Collections;
using UnityEngine;

public class GameFireFighterTruck : MonoBehaviour
{
    public enum E_FireTruckState
    {
        E_IDLE = 0,
        E_MOVE,

        E_MAX,
    }

    internal E_FireTruckState m_eState = E_FireTruckState.E_IDLE;
    float m_fMoveSpeed = 9.0f;

    public Animation m_pFighters = null;

    public void Enter()
    {
        m_eState = E_FireTruckState.E_IDLE;
        //m_pFighters.gameObject.SetActive(false);
    }

    public void Destroy()
    {
        StopCoroutine("Cor_MoveNext");
        //m_pFighters = null;
    }

    public void MoveNext(float fX)
    {
        StartCoroutine(Cor_MoveNext(fX));
    }

    IEnumerator Cor_MoveNext(float fX)
    {
        //Scene_Game.I.m_pGameRoot.m_pSound.Play("Siren");
        m_pFighters.Play("Fighter_Close");
        yield return new WaitForSeconds(0.5f);

        m_eState = E_FireTruckState.E_MOVE;
        float fStartTime = Time.time;
        float fDuration = 5.0f;
        Vector3 pPos = Vector3.zero;
        while (Mathf.Abs(transform.position.x - fX) > 3.0f)
        {
            // if (Scene_Game.I.m_pClear.gameObject.activeSelf == true) yield break;
            float fT = (Time.time - fStartTime) / fDuration;
            pPos = transform.position;
            pPos.x = Mathf.SmoothStep(pPos.x, fX, fT);
            transform.position = pPos;
            yield return null;
        }
        pPos.x = fX;
        transform.position = pPos;
        m_pFighters.Play("Fighter_Open");
        yield return new WaitForSeconds(0.5f);
        m_pFighters.Play("Fighter");
        m_eState = E_FireTruckState.E_IDLE;


        yield return null;
    }
}

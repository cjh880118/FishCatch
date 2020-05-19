using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents.Event;

public class GameSlimeSlimeObj : MonoBehaviour
{
    public GameSlime_SpriteAni m_pSprite = null;
    public Collider m_pCollider = null;
    public float m_fLifeTime = 0.0f;
    public int m_nVirusType = 0;
    public bool m_bVirusLive = false;
    public bool m_bMove = false;
    public float m_fRandZ = 0.0f;

    const float m_fEdge_L = -2.2f;
    const float m_fEdge_R = 2.2f;
    const float m_fEdge_T = 1.2f;
    const float m_fEdge_B = -1.2f;
    const float m_fEdge_Spawn_L = -2.2f;
    const float m_fEdge_Spawn_R = -2.2f;

    Coroutine m_pCor = null;
    public GameObject DieFx;
    public GameSlimeSlime Slime;
    public Transform myPool;
    public ScoreTextControl TextControl;

    public void SetMng(GameSlimeSlime slime)
    {
        Slime = slime;
        myPool = transform.parent;
    }

    public void Destroy()
    {
        StopAllCoroutines();
        this.transform.SetParent(myPool);
    }
    public void Active()
    {
        this.gameObject.SetActive(true);
        m_fRandZ = Random.Range(-0.1f, -0.2f);
        m_fLifeTime = 0.0f;
        m_bVirusLive = true;
        Vector3 pSpawnPoint = Vector3.zero;
        pSpawnPoint.x = Random.Range(0.0f, 100.0f) < 50.0f ? m_fEdge_L : m_fEdge_R;
        pSpawnPoint.y = Random.Range(m_fEdge_T, m_fEdge_B);

        pSpawnPoint.z = -3.5f + m_fRandZ;
        transform.position = pSpawnPoint;


        m_pCollider.enabled = true;

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
        }
        m_pCor = null;
        m_pCor = StartCoroutine(Cor_Idle());
    }

    public void Active(int vType, Vector3 pPos)
    {
        this.gameObject.SetActive(true);
        m_fRandZ = Random.Range(-0.1f, -0.2f);
        m_fLifeTime = 0.0f;
        m_nVirusType = vType;
        m_bVirusLive = true;
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        pPos.z = pPos.z + m_fRandZ;
        transform.position = pPos;

        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
        }
        Vector3 pSpawnPoint = pPos;

        m_pCor = null;
        m_pCor = StartCoroutine(Cor_Create(pSpawnPoint));
    }

    public void Active(bool bFallDown, int vType)
    {
        this.gameObject.SetActive(true);
        // Debug.Log("Spawn");
        m_fRandZ = Random.Range(-0.1f, -0.2f);
        m_fLifeTime = 0.0f;
        m_nVirusType = vType;
        m_bVirusLive = true;
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        if (bFallDown == false)
        {
            return;
        }
        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
        }
        Vector3 pSpawnPoint = Vector3.zero;
        pSpawnPoint.x = Random.Range(0.0f, 100.0f) < 50.0f ? m_fEdge_L : m_fEdge_R;
        pSpawnPoint.y = Random.Range(m_fEdge_T, m_fEdge_B);
        pSpawnPoint.z = -4f + m_fRandZ;
        transform.position = pSpawnPoint;
        m_pCor = null;
        //m_pCor = StartCoroutine(Cor_FallDown(pSpawnPoint));
        m_pCor = StartCoroutine(Cor_Create(pSpawnPoint));
    }

    public void DeActive()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator PlaySlimeObj()
    {
        while(true)
        {
            m_fLifeTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Cor_Idle()
    {

        float fWaitTime = Random.Range(0.1f, 3.0f);
        float fTurnTime = 0.0f;

        m_pSprite.ChangePlaying("Idle");

        for (float i = 0.0f; i < fWaitTime; i += Time.deltaTime)
        {
            fTurnTime += Time.deltaTime;
            if (fTurnTime > 0.8f)
            {
                if (Random.Range(0.0f, 100.0f) < 50.0f)
                {
                    Vector3 pScale = transform.localScale;
                    pScale.x *= -1.0f;
                    transform.localScale = pScale;
                }
                fTurnTime = 0.0f;
            }
            yield return null;
        }

        Vector3 pNextMovePoint = Vector3.zero;
        pNextMovePoint.x = Random.Range(m_fEdge_L, m_fEdge_R);
        pNextMovePoint.y = Random.Range(m_fEdge_T, m_fEdge_B);

        float fSpeed = Random.Range(0.5f, 3.0f);
        m_pCor = StartCoroutine(Cor_Move(pNextMovePoint, fSpeed));
        yield return null;
    }

    IEnumerator Cor_Move(Vector3 pMovePoint, float fSpeed)
    {
        Vector3 pPos = transform.position;
        Vector3 pStartPoint = pPos;
        Vector3 pDirection = GetDistance(pPos, pMovePoint);
        float fInversValue = 0.0f;

        m_pSprite.ChangePlaying("Move");
        //yield return null;
        if (pDirection.x > 0.0f)
        {
            Vector3 pScale = transform.localScale;
            if (pScale.x > 0.0f) pScale.x *= -1.0f;
            transform.localScale = pScale;
            TextControl.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            Vector3 pScale = transform.localScale;
            if (pScale.x < 0.0f) pScale.x *= -1.0f;
            transform.localScale = pScale;
            TextControl.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        bool bJumpSound = false;
        m_bMove = false;
        while (true)
        {
            if (m_pSprite.GetCurrFrame() == 0) bJumpSound = false;
            if (m_pSprite.GetCurrFrame() >= 2 && m_pSprite.GetCurrFrame() <= 5)
            {
                if (bJumpSound == false && m_pSprite.GetCurrFrame() == 2)
                {
                    //m_pSound.Play("Jump");

                    bJumpSound = true;
                    m_bMove = true;
                }
                pPos.x += pDirection.x / pDirection.z * fSpeed * Time.deltaTime;
                pPos.y += pDirection.y / pDirection.z * fSpeed * Time.deltaTime;
                transform.position = pPos;
            }

            fInversValue = Mathf.InverseLerp(pStartPoint.x, pMovePoint.x, pPos.x);
            if (fInversValue >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        m_pCor = StartCoroutine(Cor_Idle());
        yield return null;
    }

    public void Die()
    {
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Slime_Die);
        if (m_pCor != null)
        {
            StopCoroutine(m_pCor);
        }
        m_pCor = null;
        m_pCor = StartCoroutine(Cor_Die());
        //Scene_Game_Slime.I.m_pGame.m_pParticle_Mng.Create("Effect2", transform.position);
    }

    public void EndDie()
    {
        m_pCollider.enabled = false;
        DeActive();

        //Scene_Game_Slime.I.m_pGame.m_pParticle_Mng.Create("Effect2", transform.position);
    }

    IEnumerator Cor_Die()
    {
        m_pSprite.ChangePlaying("Die");
        m_pCollider.enabled = false;
        CellBig.SoundManager.Instance.PlaySound((int)CellBig.SoundType_GameFX.Slime_Die);

        Slime.CheckCreate();
        
        while (true)
        {
            if (m_pSprite.GetMaxFrameOnce() == true)
            {
                int nScore = 0;
                switch (m_nVirusType)
                {
                    case 0:
                        nScore = 50;
                        break;
                    case 1:
                        nScore = 150;
                        break;
                    case 2:
                        nScore = 200;
                        break;
                    case 3:
                        nScore = 250;
                        break;
                    case 4:
                        nScore = 300;
                        break;
                    default:
                        nScore = 300;
                        break;
                }
                Message.Send<CellBig.UI.Event.ADDScore>(new CellBig.UI.Event.ADDScore(100));
                FXCreate(DieFx);
                break;
            }
            yield return null;
        }

        DeActive();
        yield return null;
    }

    public void HitPoint(Vector3 hitpoint)
    {
        TextControl.transform.parent.position = hitpoint;
        TextControl.SetScore(100);
    }

    IEnumerator Cor_FallDown(Vector3 pMovePoint)
    {
        float fGravity = 13.8f;
        float fDT = 0.0f;
        float fJumpDT = 0.0f;
        float fMove = 0.0f;
        m_pCollider.enabled = false;
        Vector3 pPos = transform.position;
        pPos.x = pMovePoint.x;
        pPos.y = 7.0f;
        m_pSprite.ChangePlaying("Idle");
        yield return null;
        while (true)
        {
            fDT += Time.deltaTime;
            fMove = (fGravity * ((fDT - fJumpDT)));
            pPos.y -= fMove * Time.deltaTime;
            transform.position = pPos;
            if (pPos.y <= pMovePoint.y)
            {
                //m_pSound.Play("Down");
                //Scene_Game_Slime.I.m_pGame.m_pParticle_Mng.Create("Dust", transform.position + Vector3.down * 0.2f);
                break;
            }
            yield return null;
        }
        m_pCollider.enabled = true;
        m_pCor = StartCoroutine(Cor_Idle());
    }

    IEnumerator Cor_Create(Vector3 pMovePoint)
    {
        float fCurScale = 0.0f;
        float fRandScale = Random.Range(0.6f, 1.0f);
        m_pSprite.ChangePlaying("Idle");
        yield return null;
        for (float i = 0.001f; i < fRandScale; i += Time.deltaTime)
        {
            transform.localScale = new Vector3(i, i, i);
            yield return null;
        }
        transform.localScale = new Vector3(fRandScale, fRandScale, fRandScale);
        m_pCollider.enabled = true;
        m_pCor = StartCoroutine(Cor_Idle());
    }

    void FXCreate(GameObject fx)
    {
        GameObject createObj;
        Vector3 pos;
        createObj = Instantiate(fx);
        createObj.SetActive(false);
        createObj.transform.SetParent(this.transform.parent);
        pos = this.transform.position;
        createObj.SetActive(true);
        createObj.transform.position = pos;
        DestroyFx(createObj);
    }

    void DestroyFx(GameObject fx)
    {
        Destroy(fx, 0.5f);
    }

    Vector3 GetDistance(Vector3 stPos1, Vector3 stPos2)
    {
        Vector3 vPos;
        vPos.x = stPos2.x - stPos1.x;
        vPos.y = stPos2.y - stPos1.y;

        vPos.z = Mathf.Sqrt((vPos.x * vPos.x) + (vPos.y * vPos.y));

        return vPos;
    }
}

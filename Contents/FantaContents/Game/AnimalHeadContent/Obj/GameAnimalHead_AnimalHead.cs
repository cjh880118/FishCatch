using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameAnimalHead_AnimalHead : MonoBehaviour
{
    public MeshFilter m_pMeshFilter = null;
    public MeshRenderer m_pMeshRender = null;

    public List<Mesh> m_pMeshList = null;
    public List<Material> m_pMaterialList = null;

    public Animation anim;
    public BoxCollider m_pCollider = null;

    public GameObject m_bBoom;

    public GameObject explosionParicle;
    public GameObject pangParticle;

    public ScoreTextControl scoreTextControl;

    int score = 100;

    int m_nAnimal_ClassNum = 0;
    float m_fMinSpeed = 20.0f;
    float m_fMaxSpeed = 40.0f;
    float m_fBoomPercent = 10.0f;

    float m_fStartPosY = -65.0f;
    float m_fExitPosY = 65.0f;
    float m_fMinPosX = -70.0f;
    float m_fMaxPosX = 70.0f;
    float m_fBetweenZ = 20.0f;

    float m_fSpeed = 0.0f;
    internal bool m_bLife = false;
    internal bool m_bBomb = false;
    Vector3 m_pPos;

    private void OnEnable()
    {
        m_bLife = true;
        m_bBomb = false;

        m_fSpeed = Random.Range(m_fMinSpeed, m_fMaxSpeed);
        m_pPos = transform.localPosition;
        m_pPos.x = Random.Range(m_fMinPosX, m_fMaxPosX);
        m_pPos.y = m_fStartPosY;
        transform.localPosition = m_pPos;

        m_nAnimal_ClassNum = Random.Range(0, m_pMeshList.Count);
        m_pMeshFilter.mesh = m_pMeshList[m_nAnimal_ClassNum];
        m_pMeshRender.material = m_pMaterialList[m_nAnimal_ClassNum];

        m_pCollider.enabled = true;

        if (Random.Range(0.0f, 100.0f) < m_fBoomPercent)
        {
            m_bBomb = true;
            m_bBoom.SetActive(true);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.AnimalHead_Bomb);
            Debug.Log("Animal" + gameObject.name);
        }
    }

    private void Update()
    {
        if (m_bLife == true)
        {
            m_pPos.y += m_fSpeed * Time.deltaTime;
            transform.localPosition = m_pPos;
            if (m_pPos.y > m_fExitPosY)
                DeActive();
        }
    }

    void DeActive()
    {
        m_bLife = false;

        if(m_bBomb)
            SoundManager.Instance.StopSound((int)SoundType_GameFX.AnimalHead_Bomb);

        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(this.transform.gameObject));
    }

    public void SetZ(int nNum_Z)
    {
        m_pPos.z = nNum_Z * m_fBetweenZ;
    }

    public void Set_Boom()
    {
        m_bBoom.SetActive(true);
    }

    public void Die()
    {
        StartCoroutine(Cor_Die());
    }

    IEnumerator Cor_Die()
    {
        anim.Play("AnimalHead_Die");
        m_pCollider.enabled = false;

        if (m_bBoom.activeInHierarchy)
        {
            explosionParicle.SetActive(true);
            Message.Send<ShakeCameraMsg>(new ShakeCameraMsg(0.5f, 1.0f));
            Message.Send<ColorCameraMsg>(new ColorCameraMsg(0.1f, new Color(0.7f, 0.7f, 0.7f, 0.7f)));
            Message.Send<ADDScore>(new ADDScore(-score));
            scoreTextControl.SetScore(-score);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.AnimalHead_Explosion);
        }
        else
        {
            pangParticle.SetActive(true);
            Message.Send<ADDScore>(new ADDScore(score));
            scoreTextControl.SetScore(score);
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.AnimalHead_Balloon);
        }

        //m_pSound.Play(m_nAnimal_ClassNum.ToString());
        SoundManager.Instance.PlaySound(m_nAnimal_ClassNum + 4004);
        m_bBoom.SetActive(false);

        yield return new WaitForSeconds(1.1f);

        explosionParicle.SetActive(false);
        pangParticle.SetActive(false);

        DeActive();
    }
}

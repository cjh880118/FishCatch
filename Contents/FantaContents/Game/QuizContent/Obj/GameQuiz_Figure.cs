using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

using DG.Tweening;

public class GameQuiz_Figure : MonoBehaviour
{
    public MeshType meshType;
    public PatternType patternType;

    public GameObject mainMesh;
    MeshFilter meshFilter;
    MeshRenderer meshRen;

    public Mesh[] figureMesh;
    public Material[] figureMat;

    public GameObject correctParticle;
    public GameObject wrongParticle;
    GameObject tempParticle;

    public ScoreTextControl scoreTextControl;

    Collider collider = null;

    int randomMeshType;
    int randomPatternType;
    public int spawnIndex;
    int score = 100;

    Coroutine hitEventCor;

    private void Awake()
    {
        meshFilter = mainMesh.GetComponent<MeshFilter>();
        meshRen = mainMesh.GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        Active();
        AddMessage();
    }

    private void OnDisable()
    {
        RemoveMessage();
    }

    void AddMessage()
    {
        Message.AddListener<PoolObjectMsg>(OnDeActiveHatchDragonMsg);
    }

    void RemoveMessage()
    {
        Message.RemoveListener<PoolObjectMsg>(OnDeActiveHatchDragonMsg);
    }

    void Active()
    {
        randomMeshType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MeshType)).Length);
        randomPatternType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(PatternType)).Length);

        meshType = (MeshType)randomMeshType;
        patternType = (PatternType)randomPatternType;

        meshFilter.mesh = figureMesh[randomMeshType];
        meshRen.material = figureMat[randomPatternType];

        transform.localPosition = new Vector3(10000,10000,0);
        mainMesh.transform.localScale = Vector3.zero;

        mainMesh.transform.DOScale(1.0f, 1.0f);
        meshRen.material.DOFade(1.0f, 0.0f);

        collider.enabled = true;
    }

    void OnDeActiveHatchDragonMsg(PoolObjectMsg msg)
    {
        DeActive();
    }

    void DeActive()
    {
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(gameObject));
    }

    public void SetSpawn(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
    }

    public void Hit(MeshType _meshType, PatternType _patternType)
    {
        if (hitEventCor != null)
            return;

        if (meshType == _meshType && patternType == _patternType)
        {
            tempParticle = correctParticle;
            tempParticle.SetActive(true);

            Message.Send<ADDScore>(new ADDScore(score));
            scoreTextControl.SetScore(score);

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Quiz_Correct);
        }
        else
        {
            tempParticle = wrongParticle;
            tempParticle.SetActive(true);

            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Quiz_Wrong);
        }

        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        mainMesh.transform.DOScale(0.0f, 1.0f);
        meshRen.material.DOFade(0.0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        tempParticle.SetActive(false);
        hitEventCor = null;

        DeActive();
    }
}

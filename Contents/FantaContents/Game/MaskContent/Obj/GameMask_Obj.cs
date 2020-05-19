using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using CellBig;
using CellBig.UI.Event;
using CellBig.Contents.Event;

public class GameMask_Obj : MonoBehaviour
{
    public GameObject Mask = null;
    public GameObject MaskEffect = null;

    public GameObject Parts = null;
    public GameObject PartsEffect = null;

    public Animation Ani = null;
    public Collider Collider = null;

    public MeshFilter Mesh = null;
    public MeshRenderer MeshRender = null;

    public List<Mesh> Meshs = null;
    public List<Material> Materials = null;

    public ScoreTextControl scoreTextControl;

    Coroutine _CorUpdate = null;

    static float _BigTime_Min = 5.0f;
    static float _BigTime_Max = 10.0f;
    float _BigTime = 0.0f;

    static int _MaxHp = 3;
    int _Hp = 0;

    int _HitCount = 0;

    int score = 100;

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

    public void Active(Vector3 pPos)
    {
        ObjActive(true);

        _BigTime = Random.Range(_BigTime_Min, _BigTime_Max);
        _Hp = 1;
        _HitCount = 0;

        int temp = Random.Range(1, 3);
        Mesh.mesh = Meshs[temp];
        MeshRender.material = Materials[temp];

        transform.position = pPos;
        Ani.Play("Mask_Spawn");

        _CorUpdate = StartCoroutine(Cor_Update());

        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mask_Init);
    }

    void ObjActive(bool active)
    {
        Collider.enabled = active;
        Mask.SetActive(active);
        MaskEffect.SetActive(active);
        Parts.SetActive(!active);
        PartsEffect.SetActive(!active);
    }

    IEnumerator Cor_Update()
    {
        while (Ani.IsPlaying("Mask_Spawn") == true)
            yield return null;

        Ani.Play("Mask_Idle");

        float time_Before = _BigTime * 0.7f;
        yield return new WaitForSeconds(time_Before);
        
        Ani.Play("Mask_Shake2");
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mask_Alarm);
        float time_After = _BigTime - time_Before;
        yield return new WaitForSeconds(time_After);

        _Hp = _MaxHp;
        Ani.Play("Mask_Idle");
        transform.localScale = Vector3.one * 1.2f;
        SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mask_Monster);
    }

    public void Hit()
    {
        _HitCount++;
        if (_HitCount == _Hp)
        {
            ObjActive(false);

            StopCoroutine(_CorUpdate);
            StartCoroutine(Cor_Deactive());
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mask_Break);
        }
        else
        {
            StartCoroutine(Cor_Hit());
            SoundManager.Instance.PlaySound((int)SoundType_GameFX.Mask_Hit);
        }

        Message.Send<ADDScore>(new ADDScore(score));
        scoreTextControl.SetScore(score);

    }

    IEnumerator Cor_Hit()
    {
        if (Ani.IsPlaying("Mask_Hit") == true)
            Ani.Stop("Mask_Hit");

        Ani.Play("Mask_Hit");
        while (Ani.IsPlaying("Mask_Hit") == true)
            yield return null;

        Ani.Play("Mask_Idle");
    }

    IEnumerator Cor_Deactive()
    {
        yield return new WaitForSeconds(2f);
        Deactive();
    }
    
    void OnPoolObjectMsg(PoolObjectMsg msg)
    {
        Deactive();
    }

    void Deactive()
    {
        StopAllCoroutines();
        Message.Send<GameObjectDeActiveMessage>(new GameObjectDeActiveMessage(this.transform.gameObject));
    }
}

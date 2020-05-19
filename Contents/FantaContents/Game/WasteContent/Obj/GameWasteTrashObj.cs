using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameWasteTrashObj : MonoBehaviour
{
    public GameObject CurrentGameObject;
    public GameObject[] GameObjectArray;

    public Vector3 pos = Vector3.zero;
    public GameObject HitFx;

    public float lifeTime = 5f;
    public ScoreTextControl TextControl;
    private void OnEnable()
    {
        lifeTime = 5f;
        StartCoroutine(PlayTrashObj());
    }

    public void RadomObject(Vector3 newPos)
    {
        int RandomIndex = Random.Range(0, 21);

        for (int i = 0; i < GameObjectArray.Length; i++)
        {
            if (RandomIndex == i)
                GameObjectArray[i].SetActive(true);
            else
                GameObjectArray[i].SetActive(false);
        }
        this.transform.position = newPos;
    }

    
    IEnumerator PlayTrashObj()
    {
        while(true)
        {
            lifeTime -= 0.1f * Time.deltaTime;
            if (lifeTime <= 0)
                this.gameObject.SetActive(false);
            yield return null;
        }
    }


    public void Hit()
    {
        int RandomHitIndex = Random.Range(21, 30);
        for (int i = 0; i < GameObjectArray.Length; i++)
        {
            if (RandomHitIndex == i)
                GameObjectArray[i].SetActive(true);
            else
                GameObjectArray[i].SetActive(false);
        }
        JHchoi.SoundManager.Instance.PlaySound((int)JHchoi.SoundType_GameFX.Waste_Hit);
        
    }

    public void AfterHit(Vector3 pos)
    {
        this.transform.position = pos;
        TextControl.transform.parent.position = pos;
        TextControl.SetScore(100);

        StartCoroutine(Cor_DeActiveDelay(pos));
        FXCreate(HitFx);
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

    IEnumerator Cor_DeActiveDelay(Vector3 pos)
    {
        float _Time = 5f;
        this.transform.position = pos;

        while(_Time >= 0)
        {
            _Time -= 0.1f * Time.deltaTime;
            pos.y += 2f * Time.deltaTime;
            if (pos.x >= 0)
                pos.x += 1f * Time.deltaTime;
            else
                pos.x -= 1f * Time.deltaTime;
            this.transform.position = pos;
            yield return null;
        }

        this.gameObject.SetActive(false);
        yield return null;
    }

}

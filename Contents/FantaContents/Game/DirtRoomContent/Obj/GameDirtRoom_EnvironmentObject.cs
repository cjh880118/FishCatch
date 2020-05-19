using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JHchoi;

public class GameDirtRoom_EnvironmentObject : MonoBehaviour
{
    public DirtRoom_EnvironmentObjectType objectType;

    public GameObject[] activeObject;

    Coroutine hitEventCor;

    public bool isHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Foot"))
            return;

        switch (objectType)
        {
            case DirtRoom_EnvironmentObjectType.Doll:
                RandomSound((int)SoundType_GameFX.DirtRoom_Doll1, (int)SoundType_GameFX.DirtRoom_Doll3);
                break;
            case DirtRoom_EnvironmentObjectType.Plastic:
                PlaySound((int)SoundType_GameFX.DirtRoom_Plastic);
                break;
            case DirtRoom_EnvironmentObjectType.Paint_Movable:
                if (!isHit)
                    hitEventCor = StartCoroutine(HitEvent_Paint());

                PlaySound((int)SoundType_GameFX.DirtRoom_Bucket);
                break;
            case DirtRoom_EnvironmentObjectType.Paint_Moved:
                PlaySound((int)SoundType_GameFX.DirtRoom_Bucket);
                break;
        }

        isHit = true;
    }

    IEnumerator HitEvent_Paint()
    {
        if (hitEventCor != null)
            yield break;

        transform.parent.GetComponent<Animator>().SetTrigger("Hit");

        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);
        ActiveObject();

        hitEventCor = null;
    }

    void ActiveObject()
    {
        for (int index = 0; index < activeObject.Length; index++)
            activeObject[index].SetActive(true);
    }

    void PlaySound(int index)
    {
        SoundManager.Instance.PlaySound(index);
    }

    void RandomSound(int min, int max)
    {
        SoundManager.Instance.PlaySound(Random.Range(min, max + 1));
    }
}

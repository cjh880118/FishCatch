using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOctopusShitObj : MonoBehaviour
{
    public Animator mAni = null;

    public void Destroy()
    {
        mAni.ResetTrigger("Shit");
    }

    public void Acitve(Vector3 pos)
    {
        pos.z = -250.0f;
        transform.position = pos;
        transform.localScale = Vector3.one * Random.Range(0.3f, 0.5f);
        transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.Range(0.0f, 350.0f)));

        ActiveObject();
    }

    public void ActiveObject()
    {
        mAni.ResetTrigger("Shit");
        mAni.SetTrigger("Shit");
        StartCoroutine(Cor_Timer());
    }

    IEnumerator Cor_Timer()
    {
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);   // 확인해야됨
    }
}

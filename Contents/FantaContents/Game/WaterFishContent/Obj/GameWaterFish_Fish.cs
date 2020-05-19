using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWaterFish_Fish : MonoBehaviour
{
    Animation anim = null;

    Coroutine hitEventCor;

    private void Awake()
    {
        anim = transform.parent.GetComponent<Animation>();
    }

    void OnEnable()
    {
        Active();
    }

    private void OnDisable()
    {
        if (hitEventCor != null)
            StopCoroutine(hitEventCor);

        hitEventCor = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Foot"))
            Hit();
    }

    void Active()
    {
        transform.localScale = Vector3.one * 1.5f;

        anim.Play("Take_SI");
        anim["Take_SI"].speed = 1.0f;
    }

    void Hit()
    {
        if (hitEventCor != null)
            return;

        anim["Take_SI"].speed = 10.0f;
        hitEventCor = StartCoroutine(HitEvent());
    }

    IEnumerator HitEvent()
    {
        yield return new WaitForSeconds(2.0f);

        anim["Take_SI"].speed = 1.0f;
        hitEventCor = null;
    }
}

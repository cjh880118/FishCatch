using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkeleton_Tomb : MonoBehaviour
{
    public GameSkeleton_Skeleton gameSkeleton_Skeleton;

    public Animator anim;

    public bool isActive;

    private void Awake()
    {
        if (gameSkeleton_Skeleton == null)
            gameSkeleton_Skeleton = GetComponentInChildren<GameSkeleton_Skeleton>(true);

        gameSkeleton_Skeleton.SetHole(this);
    }

    private void OnEnable()
    {
        anim.Rebind();

        gameSkeleton_Skeleton.DeActive();

        isActive = false;

        anim.SetTrigger("E_Close");
    }

    public void Active()
    {
        gameSkeleton_Skeleton.gameObject.SetActive(true);
        gameSkeleton_Skeleton.Active();

        Open();

        isActive = true;
    }

    IEnumerator DeActive()
    {
        yield return new WaitForSeconds(1.0f);
        isActive = false;  
    }

    public void Close()
    {
        anim.SetTrigger("E_Close");
        StartCoroutine(DeActive());
    }

    public void Open()
    {
        anim.SetTrigger("E_Open");
    }

    public void Idle()
    {
        anim.SetTrigger("E_Idle");
    }

    public bool GetEmpty()
    {
        return isActive;
    }
}

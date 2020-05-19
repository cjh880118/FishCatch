using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkeleton_ObjectControl : MonoBehaviour
{
    public GameSkeleton_Tomb[] gameSkeleton_Tomb;

    public Animator anim;

    public List<int> isPossibleCreateList = new List<int>();

    bool isEndAnimation;

    private void OnEnable()
    {
        isEndAnimation = false;
        anim.enabled = true;
        anim.Rebind();

        StartCoroutine(EndAnimation());
    }

    private void Awake()
    {
        gameSkeleton_Tomb = GetComponentsInChildren<GameSkeleton_Tomb>(true);
    }

    IEnumerator EndAnimation()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.999f)
            yield return null;

        isEndAnimation = true;
        anim.enabled = false;
    }

    public bool IsEnAnimation()
    {
        return isEndAnimation;
    }

    public IEnumerator Cor_PlayContent_Skeleton()
    {
        while (true)
        {
            isPossibleCreateList.Clear();

            yield return null;

            for (int index = 0; index < gameSkeleton_Tomb.Length; index++)
            {
                if (!gameSkeleton_Tomb[index].isActive)
                    isPossibleCreateList.Add(index);
            }

            if (isPossibleCreateList.Count <= 0)
                continue;

            int createRandomIndex = Random.Range(0, isPossibleCreateList.Count);

            gameSkeleton_Tomb[isPossibleCreateList[createRandomIndex]].Active();

            float Random_Dealy = Random.Range(1f, 3f);

            yield return new WaitForSeconds(Random_Dealy);
            yield return null;
        }
    }
}

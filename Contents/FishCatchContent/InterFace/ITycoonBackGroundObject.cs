using CellBig.Constants.FishCatch;
using CellBig.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITycoonBackGroundObject : BackGroundObject
{
    public List<Vector3> ListPlatePos = new List<Vector3>();
    public Sprite[] spriteFood;
    public GameObject SuccessEffect;
    public GameObject FailEffect;
    Animator[] animators;

    Coroutine[] corShuffle;

    public override void InitBackGround(float maxPosition, float minPosition)
    {
        base.InitBackGround(0, 0);
        corShuffle = new Coroutine[arrayTarget.Length];
        animators = new Animator[arrayTarget.Length];
        for (int i = 0; i < arrayTarget.Length; i++)
            animators[i] = arrayTarget[i].GetComponent<Animator>();

        ListPlatePos.Clear();
        foreach (var o in arrayTarget)
            ListPlatePos.Add(o.transform.position);

        AddMessage();
    }

    void AddMessage()
    {
        Message.AddListener<MissionShuffleMsg>(MissionShuffle);
        Message.AddListener<SetTycoonMissionMsg<FoodType>>(SetTycoonMission);
        Message.AddListener<CatchFoodPlayerMsg>(CatchFoodPlayer);
    }

    private void MissionShuffle(MissionShuffleMsg msg)
    {
        if (corShuffle[msg.playerIndex] != null)
        {
            StopCoroutine(corShuffle[msg.playerIndex]);
            corShuffle[msg.playerIndex] = null;
        }

        corShuffle[msg.playerIndex] = StartCoroutine(MissionShuffle(msg.playerIndex));
    }

    IEnumerator MissionShuffle(int playerIndex)
    {
        int spriteNum = 0;
        while (true)
        {
            int rnd;//= Random.RandomRange(0, spriteFood.Length);
            do
            {
                rnd = Random.RandomRange(0, spriteFood.Length);
            } while (spriteNum == rnd);
            spriteNum = rnd;

            yield return null;
            arrayPlate[playerIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteFood[spriteNum];
        }
    }

    protected void SetTycoonMission(SetTycoonMissionMsg<FoodType> msg)
    {
        if (corShuffle[msg.playerIndex] != null)
        {
            StopCoroutine(corShuffle[msg.playerIndex]);
            corShuffle[msg.playerIndex] = null;
        }

        SetMissionSprite(msg.playerIndex, (int)msg.food);
    }

    protected abstract void SetMissionSprite(int playerIndex, int foodIndex);

    //리스폰 접시
    public void RespawnPlate(int index)
    {
        arrayTarget[index].transform.position = ListPlatePos[index];
    }

    private void CatchFoodPlayer(CatchFoodPlayerMsg msg)
    {
        ParticleSystem particleSystem;

        if (msg.isRight)
        {
            particleSystem = SuccessEffect.transform.GetChild(msg.playerIndex).gameObject.GetComponent<ParticleSystem>();
            particleSystem.gameObject.SetActive(true);
        }
        else
        {
            particleSystem = FailEffect.transform.GetChild(msg.playerIndex).gameObject.GetComponent<ParticleSystem>();
            particleSystem.gameObject.SetActive(true);
        }

        MissionShuffle(new MissionShuffleMsg(msg.playerIndex));
        StartCoroutine(EffectOff(particleSystem, msg.isRight, msg.playerIndex));
    }

    //맞고 이펙트 끝나고 미션 셋팅 이미 
    IEnumerator EffectOff(ParticleSystem particle, bool isRight, int playerIndex)
    {
        yield return new WaitForSeconds(1.0f);
        while (particle.isPlaying)
        {
            yield return null;
        }

        //if (isRight)
        SetNewMission(playerIndex);
        particle.gameObject.SetActive(false);
    }

    protected abstract void SetNewMission(int playerIndex);

    //음식 리스폰 지역
    public GameObject GetRespawnObject(int index)
    {
        return arrayTarget[index].transform.GetChild(0).gameObject;
    }

    public void SetPlate(int plateIndex)
    {
        arrayTarget[plateIndex].SetActive(true);
    }

    public IEnumerator ResetPlate()
    {
        foreach (var o in animators)
            o.SetTrigger("Out");

        yield return new WaitForSeconds(0.5f);

        foreach (var o in animators)
            o.gameObject.SetActive(false);

    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    void RemoveMessage()
    {
        Message.RemoveListener<MissionShuffleMsg>(MissionShuffle);
        Message.RemoveListener<SetTycoonMissionMsg<FoodType>>(SetTycoonMission);
        Message.RemoveListener<CatchFoodPlayerMsg>(CatchFoodPlayer);
    }
}
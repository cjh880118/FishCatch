using JHchoi.Constants.FishCatch;
using JHchoi.UI.Event;
using UnityEngine;

public class SushibackGroundObject : ITycoonBackGroundObject
{
    protected override void SetNewMission(int playerIndex)
    {
        int rndFood = Random.RandomRange((int)FoodType.Sushi_01, (int)FoodType.Sushi_13 + 1);
        Message.Send<SetTycoonMissionMsg<FoodType>>(new SetTycoonMissionMsg<FoodType>((FoodType)rndFood, playerIndex));
    }

    protected override void SetMissionSprite(int playerIndex, int foodIndex)
    {
        arrayPlate[playerIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteFood[foodIndex - (int)FoodType.Sushi_01];
    }
}

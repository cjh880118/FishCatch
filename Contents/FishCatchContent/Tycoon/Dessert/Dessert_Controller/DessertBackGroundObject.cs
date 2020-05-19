using JHchoi.Constants.FishCatch;
using UnityEngine;
using JHchoi.UI.Event;

public class DessertBackGroundObject : ITycoonBackGroundObject
{
    protected override void SetNewMission(int playerIndex)
    {
        int rndFood = Random.RandomRange((int)FoodType.Cake1, (int)FoodType.Roll3 + 1);
        Message.Send<SetTycoonMissionMsg<FoodType>>(new SetTycoonMissionMsg<FoodType>((FoodType)rndFood, playerIndex));
    }

    protected override void SetMissionSprite(int playerIndex, int foodIndex)
    {
        arrayPlate[playerIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteFood[foodIndex - (int)FoodType.Cake1];
    }
}

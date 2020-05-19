using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Constants;
using JHchoi.Constants.FishCatch;

public class TycoonPlayer : IPlayer
{
    public FoodType foodType;
    bool isMissSet = false;

    public bool IsMissSet { get => isMissSet; set => isMissSet = value; }

    public void SetFoodMission(FoodType foodType)
    {
        this.foodType = foodType;
    }

    public FoodType GetFoodMission()
    {
        return foodType;
    }

}

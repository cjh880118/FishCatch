using UnityEngine;
using System.Collections.Generic;
using CellBig.Contents;
using CellBig.Constants;
using CellBig.Models;
using CellBig.Constants.FishCatch;

namespace CellBig.UI.Event
{
    public class GotoTitleMsg : Message
    {
    }

    public class FishArriveMsg : Message
    {
        public FishType fishType;
        public int index;
        public int targetIndex;
        public FishArriveMsg(FishType fishType, int index, int targetIndex)
        {
            this.fishType = fishType;
            this.index = index;
            this.targetIndex = targetIndex;
        }
    }

    //렉트 포지션
    public class RectPositionMsg : Message
    {
        public Vector2 vec2RectViewPortPosition;
        public RectPositionMsg(Vector2 rectPosition)
        {
            vec2RectViewPortPosition = rectPosition;
        }
    }

    public class CatchPlateSuccessMsg : Message
    {
        public FishType fishType;
        public int fishIndex;
        public int playerIndex;
        public string fishName;
        public CatchPlateSuccessMsg(FishType fishType, int fishindex, int playerIndex, string fishName)
        {
            this.fishType = fishType;
            this.fishIndex = fishindex;
            this.playerIndex = playerIndex;
            this.fishName = fishName;
        }
    }

    public class CatchFoodSuccessMsg : Message
    {
        public FoodType foodType;
        public int fishIndex;
        public int playerIndex;
        public CatchFoodSuccessMsg(FoodType foodType, int fishindex, int playerIndex)
        {
            this.foodType = foodType;
            this.fishIndex = fishindex;
            this.playerIndex = playerIndex;
        }
    }

    public class MissFishMsg : Message
    {
        public FishType fishType;
        public int index;
        public Vector3 position;
        public MissFishMsg(FishType fishType, int index, Vector3 position)
        {
            this.fishType = fishType;
            this.index = index;
            this.position = position;
        }
    }

    public class MissFoodMsg : Message
    {
        public int index;
        public MissFoodMsg(int index)
        {
            this.index = index;
        }
    }

    public class CatchPlayerMsg : Message
    {
        public int playerIndex;
        public CatchPlayerMsg(int index)
        {
            playerIndex = index;
        }
    }

    public class CatchFoodPlayerMsg : Message
    {
        public bool isRight;
        public int playerIndex;
        public CatchFoodPlayerMsg(bool isRight, int index)
        {
            this.isRight = isRight;
            playerIndex = index;
        }
    }

    public class CatchInfoMsg : Message
    {
        public bool isCatch;
        public CatchInfoMsg(bool isCatch)
        {
            this.isCatch = isCatch;
        }
    }

    public class PlayEffectMsg : Message
    {
        public FishEffectType effectType;
        public Vector3 position;

        public PlayEffectMsg(FishEffectType fishEffect, Vector3 position)
        {
            this.effectType = fishEffect;
            this.position = position;
        }
    }

    public class ArrayListObjMsg<T> : Message
    {
        public T[] arrayObj;
        public ArrayListObjMsg(T[] arrayObj)
        {
            this.arrayObj = arrayObj;
        }
    }

    public class SetTycoonMissionMsg<T> : Message
    {
        public T food;
        public int playerIndex;
        public SetTycoonMissionMsg(T food, int playerIndex)
        {
            this.food = food;
            this.playerIndex = playerIndex;
        }
    }

    public class InputPlateEffectMsg : Message
    {
        public int playerIndex;
        public bool isRight;

        public InputPlateEffectMsg(int playerIndex, bool isRight)
        {
            this.playerIndex = playerIndex;
            this.isRight = isRight;
        }
    }


    public class CatchFoodPlateNumMsg : Message
    {
        public int plateNum;
        public CatchFoodPlateNumMsg(int plateNum)
        {
            this.plateNum = plateNum;
        }

    }

    public class MissionShuffleMsg : Message
    {
        public int playerIndex;
        public MissionShuffleMsg(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }
}
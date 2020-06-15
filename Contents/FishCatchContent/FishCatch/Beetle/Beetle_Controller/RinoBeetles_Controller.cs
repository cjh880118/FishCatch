using JHchoi.Constants;
using JHchoi.Constants.FishCatch;
using JHchoi.Models;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public class RinoBeetles_Controller : IFish_Controller
    {
        protected override void MissSoundPlay()
        {
            SoundManager.Instance.StopSound((int)SoundFishCatch.Bug_Fail);
        }
        protected override void FishArrive(FishArriveMsg msg)
        {
            if (msg.fishType == (FishType)fishIndexNum)
                base.FishArrive(msg);
        }

        protected override void MissFish(MissFishMsg msg)
        {
            if (msg.fishType == (FishType)fishIndexNum)
                base.MissFish(msg);
        }

        protected override void CatchPlateSuccess(CatchPlateSuccessMsg msg)
        {
            if (msg.fishType == (FishType)fishIndexNum)
                base.CatchPlateSuccess(msg);
        }
    }
}

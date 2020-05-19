using JHchoi;
using JHchoi.Constants;
using JHchoi.Constants.FishCatch;
using JHchoi.Models;
using JHchoi.Module.Detection.CV.Output;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JHchoi.Contents
{
    public class Goldfishs_Controller : IFish_Controller
    {
        protected override void MissSoundPlay()
        {
            SoundManager.Instance.StopSound((int)SoundFishCatch.Sfx_CatchIng);
        }

        protected override void FishArrive(FishArriveMsg msg)
        {
            //if (msg.fishType == (FishType)fishIndexNum)
            base.FishArrive(msg);
        }

        protected override void MissFish(MissFishMsg msg)
        {
            //if (msg.fishType == (FishType)fishIndexNum)
            base.MissFish(msg);
        }

        protected override void CatchPlateSuccess(CatchPlateSuccessMsg msg)
        {
            //if (msg.fishType == (FishType)fishIndexNum)
            base.CatchPlateSuccess(msg);
        }
    }
}

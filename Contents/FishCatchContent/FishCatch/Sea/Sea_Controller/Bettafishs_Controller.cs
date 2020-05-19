using CellBig.Constants.FishCatch;
using CellBig.UI.Event;

namespace CellBig.Contents
{
    public class Bettafishs_Controller : IFish_Controller
    {
        protected override void MissSoundPlay()
        {
            SoundManager.Instance.StopSound((int)SoundFishCatch.Sfx_CatchIng);
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

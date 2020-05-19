using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;

namespace JHchoi.Scene
{
	public class UangelInGameScene : IScene
	{
        protected override void OnLoadComplete()
        {
            var gm = Model.First<GameModel>();
            if (gm.nameType == GameModel.UangelNameType.LittleStar)
                IContent.RequestContentEnter("UangelLittleStarContent");
            else if (gm.nameType == GameModel.UangelNameType.MoonLight)
                IContent.RequestContentEnter("UangelMoonLightContent");
        }
    }
}

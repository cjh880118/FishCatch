using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;

namespace CellBig.Scene
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

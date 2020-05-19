using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;

namespace CellBig.Scene
{
    public class UangelTitleScene : IScene
    {
        protected override void OnLoadComplete()
        {
            IContent.RequestContentEnter("UangelTitleContent");
        }
        
    }
}

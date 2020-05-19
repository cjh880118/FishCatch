using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;

namespace JHchoi.Scene
{
    public class UangelTitleScene : IScene
    {
        protected override void OnLoadComplete()
        {
            IContent.RequestContentEnter("UangelTitleContent");
        }
        
    }
}

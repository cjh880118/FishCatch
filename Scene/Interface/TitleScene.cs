using JHchoi.Models;
using JHchoi.UI;
using UnityEngine;

namespace JHchoi.Scene
{
    public class TitleScene : IScene
    {
        protected override void OnLoadStart()
        {
            SetResourceLoadComplete();
        }

        protected override void OnLoadComplete()
        {
        }
    }
}

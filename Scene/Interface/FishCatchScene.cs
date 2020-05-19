using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Scene
{
    public class FishCatchScene : IScene
    {
        protected override void OnLoadStart()
        {
            Debug.Log("씬 로드");
        }

        protected override void OnLoadComplete()
        {
            Debug.Log("씬 로드 끝");
        }
    }
}

﻿

using UnityEngine;

namespace CellBig.Scene
{
	public class LobbyScene : IScene
	{
        protected override void OnLoadComplete()
        {
            SceneManager.Instance.Load(Constants.SceneName.Spring);
        }
    }
}



using UnityEngine;

namespace JHchoi.Scene
{
	public class LobbyScene : IScene
	{
        protected override void OnLoadComplete()
        {
            SceneManager.Instance.Load(Constants.SceneName.Spring);
        }
    }
}

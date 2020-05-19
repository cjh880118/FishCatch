using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;

namespace CellBig.Scene
{
	public class InGameScene : IScene
	{
        protected override void OnLoadComplete()
        {
          StartCoroutine(ChangeContent());

        }

        IEnumerator ChangeContent()
        {
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(2.0f);
            var pcm = Model.First<PlayContentModel>();
            string nextContent = pcm.GetCurrentContent().ContentName;
            IContent.RequestContentEnter(nextContent);
        }

    }
}

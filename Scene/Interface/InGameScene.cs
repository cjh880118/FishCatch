using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi.Constants;
using JHchoi.Contents;
using JHchoi.UI.Event;

namespace JHchoi.Scene
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

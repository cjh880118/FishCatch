using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi;
using JHchoi.Contents.Event;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class ResultContent : IContent
	{
        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.ResultDialog>();

            Message.Send<IsScoreContentMsg>(new IsScoreContentMsg(pcm.GetCurrentContent().isScore));
        }

        protected override void OnExit()
        {

            UI.IDialog.RequestDialogExit<UI.ResultDialog>();
        }

        protected override void AddMessage()
        {
          
        }
        protected override void RemoveMessage()
        {
        }
    }
}
